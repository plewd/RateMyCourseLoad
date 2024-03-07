open System.IO
open System.Threading
open Amazon
open Amazon.DynamoDBv2
open CourseScraper
open FSharp.Json
open CourseScraper.Scraper
open CourseScraper.CommandLine

let invokedProgramName = System.AppDomain.CurrentDomain.FriendlyName

let config =
    JsonConfig.create (unformatted = false, jsonFieldNaming = Json.snakeCase)

let scrapeToJson siteMapUrl outputPath =
    let scraper = Scraper(siteMapUrl)
    printfn "Starting to scrape..."
    let result = scraper.FetchAll()
    printfn "Done scraping"

    match result with
    | Ok v ->
        printfn "Writing data to JSON output file"
        Json.serialize v |> fun v -> File.WriteAllText(outputPath, v)
        0
    | Result.Error e ->
        failwith (e.ToString())
        1

let isStatusCodeSuccessful (code: System.Net.HttpStatusCode) =
    let n = int code
    n >= 200 && n <= 299

let writeJsonDataToDynamoDB (inputPath: string) (dynamoDBTableName: string) =
    let client = new AmazonDynamoDBClient(region = RegionEndpoint.USEast1)

    let writeToDynamoDB =
        (DynamodbClient.writeCourseToDynamoDbWithBatching client dynamoDBTableName)

    printfn "Loading data from JSON file..."

    let data =
        File.ReadAllText inputPath |> Json.deserialize<Domain.ParsedCourseList list>

    printfn "Starting to write to DynamoDB table"

    data
    |> List.iter (fun courseList ->
        let batchResponse = writeToDynamoDB courseList

        let failed =
            batchResponse
            |> List.filter (_.HttpStatusCode >> isStatusCodeSuccessful >> not)
            |> Array.ofList

        if failed.Length = 0 then
            printfn $"Finished with {courseList.CoursePrefix}"
        else
            failwith $"failed on {courseList.CoursePrefix}")

    printfn "Done!"
    0

let execute (cliArgs: Argu.ParseResults<Commands>) =
    match cliArgs.GetSubCommand() with
    | Scrape args -> scrapeToJson (args.GetResult <@ SiteMapUrl @>) (args.GetResult <@ OutputPath @>)
    | Write args -> writeJsonDataToDynamoDB (args.GetResult <@ InputPath @>) (args.GetResult <@ DynamoDBTableName @>)

[<EntryPoint>]
let main argv =
    try
        let parser =
            Argu.ArgumentParser.Create<Commands>(programName = "RateMyCourseLoadScraper")

        let cliArgs = parser.Parse(argv)
        execute cliArgs
    with :? Argu.ArguParseException as e ->
        eprintfn $"%s{e.Message}"
        1

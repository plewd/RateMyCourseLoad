open System.IO
open CourseScraper.Domain
open FSharp.Json
open CourseScraper.Scraper

let invokedProgramName = System.AppDomain.CurrentDomain.FriendlyName

let config =
    JsonConfig.create (unformatted = false, jsonFieldNaming = Json.snakeCase)

[<EntryPoint>]
let main args =
    if args.Length <> 2 then
        printfn $"Usage: %s{invokedProgramName} [siteMapUrl] [outputPath]"
        1
    else
        let siteMapUrl = args[0]
        let outputPath = args[1]
        let scraper = Scraper(siteMapUrl)
        let result = scraper.FetchAll()

        match result with
        | Ok v ->
            v
            // |> List.map (_.Courses)
            // |> List.concat
            // // drop the descriptions, can make this optional
            // |> List.map (fun v -> {| Name = v.Name; Title = v.Title; CreditHour = v.CreditHours |})
            |> Json.serializeEx config
            |> (fun v -> File.WriteAllText(outputPath, v))

            0
        | Result.Error e ->
            failwith (e.ToString())
            1

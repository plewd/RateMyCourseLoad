module CourseScraper.CommandLine

open Argu

type ScrapeArgs =
    | [<Mandatory; AltCommandLine("-s")>] SiteMapUrl of string
    | [<Mandatory; AltCommandLine("-o")>] OutputPath of string

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | SiteMapUrl _ -> "site map to find all the relevant pages"
            | OutputPath _ -> "where to save to"

type WriteArgs =
    | [<Mandatory; AltCommandLine("-i")>] InputPath of string
    | [<Mandatory; AltCommandLine("-t")>] DynamoDBTableName of string

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | InputPath _ -> "the path to the json file to read"
            | DynamoDBTableName _ -> "the DynamoDB table to write to"

// Define the top-level commands
type Commands =
    | [<First; Unique; CliPrefix(CliPrefix.None)>] Scrape of ParseResults<ScrapeArgs>
    | [<First; Unique; CliPrefix(CliPrefix.None)>] Write of ParseResults<WriteArgs>

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Scrape _ -> "scrape data from a website and save it to json"
            | Write _ -> "read the json file and write the contents to dynamodb"

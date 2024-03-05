module CourseScraper.Scraper

open System
open FSharp.Data
open FsToolkit.ErrorHandling

open CourseScraper.Domain

type SiteMap = XmlProvider<"./TypeSamples/site-map.xml">

type Scraper(siteMapUrl: string) =
    let siteMap = SiteMap.Load siteMapUrl

    let relevantPages =
        siteMap.Urls
        |> List.ofArray
        |> List.map (_.Loc >> Uri)
        |> List.choose (fun uri ->
            match uri.Segments with
            | [| _; "course-descriptions/"; coursePrefix |] ->
                {| Uri = uri
                   CoursePrefix = coursePrefix.Remove(coursePrefix.Length - 1).ToUpper() |}
                |> Some
            | _ -> None)

    member x.FetchAll() =
        relevantPages
        |> List.map (fun page ->
            let result =
                page.Uri.ToString()
                |> HtmlDocument.Load
                |> CourseListParser.getCourses
                |> Result.map (fun v ->
                    { CoursePrefix = page.CoursePrefix
                      Courses = v })
                |> Result.mapError (fun v ->
                    { ParseError = v
                      CoursePage = page.CoursePrefix })

            printfn $"Finished %s{page.CoursePrefix}"
            result)
        |> List.sequenceResultM

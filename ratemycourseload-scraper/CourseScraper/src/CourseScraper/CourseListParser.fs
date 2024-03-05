module CourseScraper.CourseListParser

open System
open System.Text.RegularExpressions
open FSharp.Data
open FsToolkit.ErrorHandling

open CourseScraper.Utility
open CourseScraper.Domain

(*
Annoying chars to handle:
<br>
&nbsp;
 *)

(*
Should look like this:
<p class="courseblocktitle noindent">
 <strong>
  CS&nbsp;4400.  Programming Languages.  (4 Hours)
 </strong>
</p>
*)
let getTitle (nodes: HtmlNode list) =
    match nodes with
    | [ n ] ->
        match n.InnerText() with
        | "" -> Error(ParseError.FindElement "title")
        | t -> Ok t
    | _ -> Error(ParseError.FindElement "title")

let creditHourParseRegex = Regex(@"^(\d*\.?\d*) Hours?$", RegexOptions.Compiled)

let creditHourRangeParseRegex =
    Regex(@"^(\d*\.?\d*)-(\d*\.?\d*) Hours?$", RegexOptions.Compiled)

let creditHourCommaParseRegex =
    Regex(@"^(\d*\.?\d*),(\d*\.?\d*) Hours?$", RegexOptions.Compiled)


let getCreditHoursFromText t =
    match t with
    | Regex creditHourParseRegex [ hoursAsString ] -> Some(CreditHour.Exact(double hoursAsString))
    | Regex creditHourRangeParseRegex [ minHour; maxHour ] -> Some(CreditHour.Range(double minHour, double maxHour))
    | Regex creditHourCommaParseRegex [ n1; n2 ] -> Some(CreditHour.Consecutive(double n1, double n2))
    | _ -> None

let titleRegex = Regex("""^(\w+ \d+)\. (.*)\. \((.*)\)$""")

let parseTitle (title: string) =
    match title with
    | Regex titleRegex [ courseName; courseTitle; creditHourText ] ->
        (match getCreditHoursFromText creditHourText with
         | None -> Error(ParseError.FindElement "Credit Hours")
         | Some v ->
             Ok
                 {| Name = courseName
                    Title = courseTitle
                    CreditHours = v |})
    | _ -> Error(ParseError.ParseElement title)


(*
Should look like this:
<p class="cb_desc">
 Blah blah
</p>
 *)
let getDescription (nodes: HtmlNode list) =
    let errResult = Error(ParseError.FindElement "description")

    match nodes with
    | [ n ] ->
        match n.InnerText() with
        | "" -> None
        | t -> Some t
    | _ -> None


(*
parsing this is annoying, going to skip this for now, example:
Prerequisite(s): (GE 1111 with a minimum grade of D- or GE 1502 with a minimum grade of D- ); MATH 2341 (may be taken concurrently) with a minimum grade of D- ; (PHYS 1155 (may be taken concurrently) with a minimum grade of D- or PHYS 1165 (may be taken concurrently) with a minimum grade of D- or PHYS 1175 with a minimum grade of D- ); EECE 2140 (may be taken concurrently) with a minimum grade of D-
Attribute(s):  NUpath Analyzing/Using Data
*)
let getExtras (nodes: HtmlNode list) : Result<Extra list, ParseError> = Ok []
// match nodes with
// | [] -> Ok []
// | lst ->
//     let texts =
//         lst
//         |> List.map _.InnerText()
//
//     Ok []
// lst
// |> List.map _.InnerText()
// Error(ParseError.FindElement "extras")

let getCourses (page: HtmlDocument) : Result<Course list, ParseError> =
    page.CssSelect ".courseblock"
    |> List.map (fun elem ->
        result {
            let! title = elem.CssSelect ".courseblocktitle" |> getTitle
            let! titleParts = parseTitle title
            let! extras = elem.CssSelect ".courseblockextra" |> getExtras
            let description = elem.CssSelect ".cb_desc" |> getDescription

            return
                { Name = titleParts.Name
                  Title = titleParts.Title
                  CreditHours = titleParts.CreditHours
                  Description = description
                  Extras = extras }
        }
        |> Result.mapError (fun v ->
            printfn "here"
            v))
    |> List.ofSeq
    |> List.sequenceResultM

module CourseScraper.Utility

open System.Text.RegularExpressions

let (|Regex|_|) (regexp: Regex) input =
    let m = regexp.Match(input)

    match m.Success, (m.Groups |> List.ofSeq) with
    | true, _wholePattern :: t -> Some [ for x in t -> x.Value ]
    | _, _ -> None

module CourseScraper.Domain

[<RequireQualifiedAccess>]
type ParseError =
    | FindElement of string
    | ParseElement of string

type ParseErrorOnPage =
    { ParseError: ParseError
      CoursePage: string }

type Extra =
    | CoreRequisite of string list
    | PreRequisite of string list

[<RequireQualifiedAccess>]
type CreditHour =
    | Range of min: double * max: double
    | Exact of double
    | Consecutive of first: double * second: double

type Course =
    { Name: string
      Title: string
      CreditHours: CreditHour
      Description: string option
      Extras: Extra list }

type ParsedCourseList =
    { CoursePrefix: string
      Courses: Course list }

type ParserResult = Result<ParsedCourseList list, ParseError>

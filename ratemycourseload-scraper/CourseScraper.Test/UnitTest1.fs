module CourseScraper.Test

open CourseScraper.Domain
open NUnit.Framework

[<SetUp>]
let Setup () = ()

let trickyCases =
    [ "ABRB 5100. International Study—Argentina. (20 Hours)"
      "ACCT 6201. Financial Reporting and Managerial Decision Making 2. (1.5 Hours)"
      "ABRC 5001. International Study: Australia. (0 Hours)"
      "ABRC 5001. International Study: Lithuania. (2.25 Hours)"
      "ACCT 6217. Corporate Governance, Ethics, and Financial Reporting. (3 Hours)"
      "ACCT 6217. Corporate Governance, Ethics, and Financial Reporting. (1-9 Hours)"
      "ARTG 7990. Thesis. (4,8 Hours)"
      "CAEP 7798. Doctoral Internship. (0.5-2 Hours)"
       ] 

[<Test>]
let Test1 () =
    let results = trickyCases |> List.map CourseListParser.parseTitle

    results |> List.forall Result.isOk |> Assert.IsTrue

    // check credit hours
    results
    |> List.map (Result.map _.CreditHours)
    |> List.map (Result.defaultValue (CreditHour.Exact -99999.0))
    |> List.zip
        [ CreditHour.Exact 20
          CreditHour.Exact 1.5
          CreditHour.Exact 0
          CreditHour.Exact 2.25
          CreditHour.Exact 3
          CreditHour.Range(1, 9)
          CreditHour.Consecutive(4,8)
          CreditHour.Range(0.5, 2)
           ]
    |> List.iter Assert.AreEqual

[<Test>]
let testParseTitleWithExtraPeriod () =
    CourseListParser.parseTitle "BUSN 6368. Immigrant Contributions to the U.S. Innovation Economy. (1 Hour)"
    |> function
        | Ok v ->
            Assert.AreEqual("BUSN 6368", v.Name)
            Assert.AreEqual("Immigrant Contributions to the U.S. Innovation Economy", v.Title)
            Assert.AreEqual(CreditHour.Exact 1, v.CreditHours)
        | _ -> Assert.Fail "Unable to parse example"

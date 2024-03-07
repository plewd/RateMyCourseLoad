module CourseScraper.DynamodbClient

open System.Collections.Generic
open Amazon.DynamoDBv2
open Amazon.DynamoDBv2.Model
open CourseScraper.Domain

let numberAttribute (n: string) =
    let attribute = AttributeValue()
    attribute.N <- n
    attribute

let dynamoDBMaxBatchWriteAmount = 25

let getWriteRequestBatch (prefix: string) (prefixLength: int) (course: Course) =
    let item =
        dict
            [ ("Prefix", AttributeValue(prefix))
              ("Number", course.Name.Substring(prefixLength + 1) |> AttributeValue)
              ("Title", AttributeValue(course.Title))
              ("Description", course.Description |> Option.defaultValue "" |> AttributeValue)
              ("CreditHours",
               (match course.CreditHours with
                | CreditHour.Exact hours -> hours.ToString()
                | CreditHour.Consecutive(first, _) -> first.ToString()
                | CreditHour.Range(_, max) -> max.ToString())
               |> numberAttribute) ]
        |> Dictionary<string, AttributeValue>

    let writeRequest = WriteRequest()
    writeRequest.PutRequest <- PutRequest(Item = item)
    writeRequest

let writeCourseToDynamoDbWithBatching
    (client: AmazonDynamoDBClient)
    (destinationTable: string)
    (coursesForPrefix: ParsedCourseList)
    =
    let prefixLength = coursesForPrefix.CoursePrefix.Length

    // collect the writes for each of the batchWrites
    let writeRequestBatches =
        coursesForPrefix.Courses
        |> List.chunkBySize dynamoDBMaxBatchWriteAmount
        |> List.map (fun courseList ->
            courseList
            |> List.map (getWriteRequestBatch coursesForPrefix.CoursePrefix prefixLength))

    // send the batch requests
    writeRequestBatches
    |> List.map (fun writeRequests ->
        let batchWriteItemRequest = BatchWriteItemRequest()

        batchWriteItemRequest.RequestItems <-
            dict [ (destinationTable, List<WriteRequest>(writeRequests)) ]
            |> Dictionary<_, _>

        client.BatchWriteItemAsync(batchWriteItemRequest)
        |> Async.AwaitTask
        |> Async.RunSynchronously)

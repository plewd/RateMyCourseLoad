using Amazon.DynamoDBv2.DataModel;

namespace ServerlessAPI.Entities;

// <summary>
/// Map the Course Class to DynamoDb Table
/// To learn more visit https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DeclarativeTagsList.html
/// </summary>
[DynamoDBTable("ratemycourseloadCourseCatalog")]
public class Course
{
    ///<summary>
    /// Map c# types to DynamoDb Columns 
    /// to learn more visit https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/MidLevelAPILimitations.SupportedTypes.html
    /// <summary>
    [DynamoDBHashKey] //Partition key
    public string Prefix { get; set; }
    
    [DynamoDBRangeKey]
    public int Number { get; set; }

    [DynamoDBProperty]
    public string Title { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string? Description { get; set; }

    [DynamoDBProperty]
    public int CreditHours { get; set; }

    public string GetCourseName()
    {
        return $"{Prefix}{Number}";
    }
}

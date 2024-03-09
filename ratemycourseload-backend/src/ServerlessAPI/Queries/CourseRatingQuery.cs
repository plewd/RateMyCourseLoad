using System.Text.Json.Serialization;

namespace ServerlessAPI.Queries;

public class CourseRatingQuery
{
    [JsonPropertyName("courses")]
    public List<string> Courses { get; set; }
    
}
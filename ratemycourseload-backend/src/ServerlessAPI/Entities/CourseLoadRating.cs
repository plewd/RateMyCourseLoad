using System.Text.Json.Serialization;

namespace ServerlessAPI.Entities;

// <summary>
/// Rating of the course load from the OpenAI API.
/// Broken up into 3 categories: credit hours, workload, and balance.
/// </summary>
public class CourseLoadRating
{
    [JsonPropertyName("credit hours")]
    public string CreditHours { get; set; }
    [JsonPropertyName("workload")]
    public string Workload { get; set; }
    [JsonPropertyName("balance")]
    public string Balance { get; set; }
    [JsonPropertyName("score")]
    public int Score { get; set; }
}
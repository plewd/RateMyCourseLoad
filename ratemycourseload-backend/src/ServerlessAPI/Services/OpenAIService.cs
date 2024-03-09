using System.Text;
using System.Text.RegularExpressions;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using ServerlessAPI.Exceptions;
using ServerlessAPI.Repositories;

namespace ServerlessAPI.Services;

public class OpenAIService : IOpenAIService
{
    private OpenAIClient _client;
    private ICourseRepository _repo;
    private ILogger<OpenAIClient> _logger;

    public OpenAIService(ILogger<OpenAIClient> logger, ICourseRepository repo, IOptions<Settings> settings)
    {
        _logger = logger;
        _repo = repo;
        _client = new OpenAIClient(settings.Value.OpenAIAPIKey);
    }

    public async Task<(string, int)> GetDescriptiveTextForSelectedCourses(string[] courseNames)
    {
        var sb = new StringBuilder();
        var pattern = "([A-Za-z]+)([0-9]+)";
        var totalCreditHours = 0;
        
        foreach (var courseString in courseNames)
        {
            // parse course string to match prefix and number
            var match = Regex.Match(courseString, pattern);
            var coursePrefix = match.Groups[1].Value;
            var courseNumber = int.Parse(match.Groups[2].Value);
            
            _logger.LogInformation(
                "Fetching course by id {CourseId} for student message",
                coursePrefix + courseNumber
            );
            
            // search for course info from DynamoDB
            var course = await _repo.GetByIdAsync(coursePrefix, courseNumber);
        
            if (course == null)
            {
                throw new NullDatabaseException($"Course '{course}' not found in database");
            }
            // count total credit hours manually
            totalCreditHours += course.CreditHours;
            // format course information and add to string
            sb.Append($"Course Code: {course.GetCourseName()}\nTitle: {course.Title}\n" +
                      $"Description: {course.Description}\n\n");
        }
        
        return (sb.ToString(), totalCreditHours);
    }

    public async Task<Response<ChatCompletions>> RateCourses(string courseDescriptions)
    {
        var options = new ChatCompletionsOptions("gpt-3.5-turbo",
            new ChatRequestMessage[]
            {
                new ChatRequestSystemMessage("You are a course advisor for a university. " +
                                             "When given a list of courses that the student is planning on taking, " +
                                             "you should analyze the courses and schedule, and give advice directly to the student by referring to them in second person." +
                                             "Return the output as a valid JSON array following this format:\n" +
                                             "[{\"workload\": (string) \"analysis and advice about whether the classes are too easy or difficult. Consider factors such as overall complexity of the concepts learnt.\",\n" +
                                             "\"balance\": (string) \"analysis and advice about whether the student has a good balance of technical classes and liberal arts classes. Consider the mix of courses and their nature.\",\n" +
                                             "\"score\": (integer) \"a score from 1-5 to rate the student's schedule. Consider overall workload and balance of the classes.}]"),
                new ChatRequestUserMessage("Here are the courses I am planning on taking, what do you think:\n" + courseDescriptions)
                {
                    Name="student"
                },
            })
        {
            ResponseFormat = ChatCompletionsResponseFormat.JsonObject
        };
        
        return await _client.GetChatCompletionsAsync(options);
    }
}
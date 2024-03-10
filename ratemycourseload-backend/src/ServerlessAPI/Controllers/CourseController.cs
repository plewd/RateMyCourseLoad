using Microsoft.AspNetCore.Mvc;
using ServerlessAPI.Entities;
using ServerlessAPI.Queries;
using ServerlessAPI.Repositories;
using System.Text.Json;
using ServerlessAPI.Services;

namespace ServerlessAPI.Controllers;

[Route("[controller]")]
[Produces("application/json")]
public class CourseController : ControllerBase
{
    private ILogger<CourseController> _logger;
    private ICourseRepository _repo;
    private IOpenAIService _oAIService;
    private static int veryLowCreditHours = 6;
    private static int minCreditHours = 16;
    private static int maxCreditHours = 19;
    
    // ReSharper disable once ConvertToPrimaryConstructor
    // doing this will break the IOptions functionality
    public CourseController(ILogger<CourseController> logger, ICourseRepository repo, IOpenAIService oAIService)
    {
        _logger = logger;
        _repo = repo;
        _oAIService = oAIService;
    }
    
    [HttpGet(template:"prefix/{coursePrefix}")]
    public async Task<ActionResult<IEnumerable<Course>>> Get(string coursePrefix)
    {
        _logger.LogInformation("Fetching courses with prefix {CoursePrefix}", coursePrefix);
        var courses = await _repo.GetCoursesAsync(coursePrefix);
        _logger.LogInformation("Found {CourseCount} courses", courses.Count);
        return Ok(courses);
    }
    
    [HttpGet(template:"{coursePrefix}/{courseNumber:int}")]
    public async Task<ActionResult<IEnumerable<Course>>> Get(string coursePrefix, int courseNumber)
    {
        _logger.LogInformation(
            "Fetching course by id {CourseId}",
            coursePrefix + courseNumber
        );
        
        var course = await _repo.GetByIdAsync(coursePrefix, courseNumber);

        if (course == null)
        { 
            return NotFound();
        }

        return Ok(
            course
        );
    }
    
    [HttpPost(template:"rate")]
    public async Task<ActionResult<CourseLoadRating>> Post([FromBody] CourseRatingQuery query)
    {
        try
        {
            _logger.LogInformation("contacting OpenAI API to rate courses...");
            var (courseDescriptions, totalCreditHours) = await _oAIService.GetDescriptiveTextForSelectedCourses(query.Courses.ToArray());
            var response = await _oAIService.RateCourses(courseDescriptions);
            var courseLoadRating = JsonSerializer.Deserialize<CourseLoadRating>(response.Value.Choices[0].Message.Content);
            
            if (totalCreditHours < minCreditHours)
            {
                courseLoadRating.CreditHours = $"You currently only have {totalCreditHours} credit hours in your schedule, consider adding more classes to your schedule.";
                courseLoadRating.Score = totalCreditHours < veryLowCreditHours ? 1 : 2;
            }
            else if (totalCreditHours > maxCreditHours)
            {
                courseLoadRating.CreditHours = $"You currently have {totalCreditHours} credit hours in your schedule, consider reducing the amount of classes you are taking.";
                courseLoadRating.Score = 3;
            }
            else
            {
                courseLoadRating.CreditHours = $"You currently have {totalCreditHours}, which is an appropriate amount for a semester.";
            }
            
            return Ok(
                courseLoadRating
            );
        }
        catch (Exception e)
        {
            return NotFound();
        }
    }
}

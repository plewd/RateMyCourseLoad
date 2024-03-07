using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServerlessAPI.Entities;
using ServerlessAPI.Queries;
using ServerlessAPI.Repositories;
using System;
using System.Text;
using System.Text.RegularExpressions;
using Azure;
using Azure.AI.OpenAI;
using ServerlessAPI.Services;

namespace ServerlessAPI.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
public class CourseController(ILogger<CourseController> logger, ICourseRepository repo, IOpenAIService oAIService) : ControllerBase
{
    private ILogger<CourseController> _logger;
    private ICourseRepository _repo;
    private IOptions<Settings> _settings;
    
    // ReSharper disable once ConvertToPrimaryConstructor
    // doing this will break the IOptions functionality
    public CourseController(ILogger<CourseController> logger, ICourseRepository repo, IOptions<Settings> settings)
    {
        _logger = logger;
        _repo = repo;
        _settings = settings;
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
    public async Task<ActionResult<IEnumerable<Course>>> Get([FromBody] CourseRatingQuery query)
    {
        var sb = new StringBuilder();
        var pattern = "([A-Za-z]+)([0-9]+)";
        
        foreach (var courseString in query.Courses)
        {
            // parse course string to match prefix and number
            Match match = Regex.Match(courseString, pattern);
            
            // search for course info from DynamoDB
            var course = await repo.GetByIdAsync(match.Groups[1].Value, int.Parse(match.Groups[2].Value));

            if (course == null)
            { 
                return NotFound();
            }
            
            sb.Append($"Course Code: {course.GetCourseName()}\nTitle: {course.Title}\n" +
                      $"Description: {course.Description}\nCredit Hours: {course.CreditHours}\n\n");
        }
        
        var oAIService = new OpenAIService("apiKey", sb.ToString());
        // should probably check that this worked properly, before just passing it to Ok output
        Response<ChatCompletions> response = await oAIService.RateCourses();
        
        return Ok(
            response.Value
        );
    }
}

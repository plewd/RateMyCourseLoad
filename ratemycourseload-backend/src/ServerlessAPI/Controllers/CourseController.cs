using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServerlessAPI.Entities;
using ServerlessAPI.Queries;
using ServerlessAPI.Repositories;
using System;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Azure;
using Azure.AI.OpenAI;
using ServerlessAPI.Services;

namespace ServerlessAPI.Controllers;

[Route("[controller]")]
[Produces("application/json")]
public class CourseController : ControllerBase
{
    private ILogger<CourseController> _logger;
    private ICourseRepository _repo;
    private IOpenAIService _oAIService;
    
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
            var response = await _oAIService.RateCourses(query.Courses.ToArray());
            var courseLoadRating = JsonSerializer.Deserialize<CourseLoadRating>(response.Value.Choices[0].Message.Content);
            // var responseContent = JsonContent.Create(json);
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

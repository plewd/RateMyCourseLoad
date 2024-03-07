using Microsoft.AspNetCore.Mvc;
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
    [HttpGet(template:"prefix/{coursePrefix}")]
    public async Task<ActionResult<IEnumerable<Course>>> Get(string coursePrefix)
    {
        logger.LogInformation("Fetching courses with prefix {CoursePrefix}", coursePrefix);
        return Ok(
            await repo.GetCoursesAsync(coursePrefix)
        );
    }
    
    [HttpGet(template:"{coursePrefix}/{courseNumber:int}")]
    public async Task<ActionResult<IEnumerable<Course>>> Get(string coursePrefix, int courseNumber)
    {
        logger.LogInformation(
            "Fetching course by id {CourseId}",
            coursePrefix + courseNumber
        );
        
        var course = await repo.GetByIdAsync(coursePrefix, courseNumber);

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

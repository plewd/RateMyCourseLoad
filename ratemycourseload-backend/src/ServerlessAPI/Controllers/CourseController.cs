using Microsoft.AspNetCore.Mvc;
using ServerlessAPI.Entities;
using ServerlessAPI.Repositories;

namespace ServerlessAPI.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
public class CourseController(ILogger<CourseController> logger, ICourseRepository repo) : ControllerBase
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
}

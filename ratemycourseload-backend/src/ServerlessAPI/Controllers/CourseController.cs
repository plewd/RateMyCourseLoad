using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServerlessAPI.Entities;
using ServerlessAPI.Repositories;

namespace ServerlessAPI.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
public class CourseController : ControllerBase
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
}

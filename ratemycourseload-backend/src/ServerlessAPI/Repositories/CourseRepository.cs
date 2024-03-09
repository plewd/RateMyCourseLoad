using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using ServerlessAPI.Entities;

namespace ServerlessAPI.Repositories;

public class CourseRepository(IDynamoDBContext context, ILogger<CourseRepository> logger) : ICourseRepository
{
    public async Task<bool> CreateAsync(Course course)
    {
        try
        {
            await context.SaveAsync(course);
            logger.LogInformation("Course {CourseName} is added", course.GetCourseName());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "fail to persist to DynamoDb Table");
            return false;
        }

        return true;
    }

    public async Task<bool> DeleteAsync(Course course)
    {
        bool result;
        try
        {
            // Delete the Course.
            await context.DeleteAsync<Course>(course.Prefix, course.Number);
            // Try to retrieve deleted Course. It should return null.
            var deletedCourse = await context.LoadAsync<Course>(
                course.Prefix,
                course.Number,
                new DynamoDBOperationConfig()
                { 
                    ConsistentRead = true
                },
                CancellationToken.None
            );

            result = deletedCourse == null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "fail to delete Course from DynamoDb Table");
            result = false;
        }

        if (result) logger.LogInformation("Course {CourseName} is deleted", course.GetCourseName());

        return result;
    }

    public async Task<bool> UpdateAsync(Course course)
    {
        try
        {
            await context.SaveAsync(course);
            logger.LogInformation("Course {id} is updated", course.GetCourseName());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "fail to update Course from DynamoDb Table");
            return false;
        }

        return true;
    }

    public async Task<Course?> GetByIdAsync(string coursePrefix, int courseNumber)
    {
        try
        {
            return await context.LoadAsync<Course>(coursePrefix, courseNumber.ToString());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "fail to fetch Course id {CourseID} from DynamoDb Table",
                coursePrefix + courseNumber);
            return null;
        }
    }

    public async Task<IList<Course>> GetCoursesAsync(string coursePrefix)
    {
        var result = new List<Course>();

        try
        {
            var queryResult = context.QueryAsync<Course>(coursePrefix.ToUpper());

            do
            {
                result.AddRange(await queryResult.GetNextSetAsync());
            } while (!queryResult.IsDone);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "fail to list Courses from DynamoDb Table");
            return new List<Course>();
        }

        return result;
    }
}
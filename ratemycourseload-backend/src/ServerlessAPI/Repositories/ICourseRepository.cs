using ServerlessAPI.Entities;

namespace ServerlessAPI.Repositories;

/// <summary>
/// DynamoDB Table Course CRUD
/// </summary>
public interface ICourseRepository
{
    /// <summary>
    /// Include new Course to the DynamoDB Table
    /// </summary>
    /// <param name="course">Course to include</param>
    /// <returns>success/failure</returns>
    Task<bool> CreateAsync(Course course);
    
    /// <summary>
    /// Remove existing Course from DynamoDB Table
    /// </summary>
    /// <param name="course">Course to remove</param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Course course);

    /// <summary>
    /// List Course from DynamoDb Table
    /// </summary>
    /// <returns>Collection of Courses</returns>
    Task<IList<Course>> GetCoursesAsync(string coursePrefix);

    /// <summary>
    /// Get Course by PK
    /// </summary>
    /// <param name="coursePrefix"></param>
    /// <param name="courseNumber"></param>
    /// <returns>Course object</returns>
    Task<Course?> GetByIdAsync(string coursePrefix, int courseNumber);
    
    /// <summary>
    /// Update Course content
    /// </summary>
    /// <param name="course">Course to be updated</param>
    /// <returns></returns>
    Task<bool> UpdateAsync(Course course);
}
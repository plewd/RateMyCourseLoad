using Azure;
using Azure.AI.OpenAI;

namespace ServerlessAPI.Services;

public interface IOpenAIService
{
    public Task<(string, int)> GetDescriptiveTextForSelectedCourses(string[] courseNames);
    public Task<Response<ChatCompletions>> RateCourses(string courseDescriptions);
}
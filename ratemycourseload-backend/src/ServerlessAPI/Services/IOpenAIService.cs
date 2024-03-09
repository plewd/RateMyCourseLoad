using Azure;
using Azure.AI.OpenAI;

namespace ServerlessAPI.Services;

public interface IOpenAIService
{
    public Task<Response<ChatCompletions>> RateCourses(string[] courseNames);
    
}
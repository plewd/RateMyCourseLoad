using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;

namespace ServerlessAPI.Services;

public class OpenAIService : IOpenAIService
{
    private OpenAIClient _client;
    private string _studentMessage;

    public OpenAIService(IOptions<string> apiKey, string studentMessage)
    {
        _client = new OpenAIClient(apiKey.Value);
        _studentMessage = studentMessage;
    }

    public async Task<Response<ChatCompletions>> RateCourses()
    {
        
        var options = new ChatCompletionsOptions("gpt-3.5-turbo",
            new ChatRequestMessage[]
            {
                new ChatRequestSystemMessage("You are a course advisor for a university, " +
                                             "when given a list of courses that the student is planning on taking, " +
                                             "you should analyze their schedule and whether the workload is too light or heavy."),
                new ChatRequestUserMessage("Here are the courses I am planning on taking:\n" + _studentMessage)
                {
                    Name="student"
                },
            });
        
        return await _client.GetChatCompletionsAsync(options);
    }
}
using System.Text.Json;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using ServerlessAPI;
using ServerlessAPI.Repositories;
using ServerlessAPI.Services;


var builder = WebApplication.CreateBuilder(args);

// Logger
builder.Logging
        .ClearProviders()
        .AddJsonConsole();
 
// Add services to the container.
builder.Services
        .AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

/*
builder.Services.Configure<AppConfiguration>(options =>
        var client = AmazonSystem
        
        options.AddOpenAIClient()
);
*/

var region = Environment.GetEnvironmentVariable("AWS_REGION") ?? RegionEndpoint.USEast1.SystemName;
builder.Services
        .AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(RegionEndpoint.GetBySystemName(region)))
        .AddScoped<IDynamoDBContext, DynamoDBContext>()
        .AddScoped<ICourseRepository, CourseRepository>()
        .AddScoped<IOpenAIService, OpenAIService>();

builder.Configuration.AddSystemsManager("/ratemycourseload/");
builder.Services.Configure<Settings>(builder.Configuration.GetSection("settings"));

// Add services to the container.
builder.Services
        .AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

// var corsSettings = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<Settings.CorsSettings>>().Value;
const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.WithOrigins(
                                          builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                                          ?? Array.Empty<string>())
                              .AllowAnyHeader()
                              .AllowAnyMethod(); //corsSettings.AllowedOrigins);
                      });
});

// Add AWS Lambda support. When running the application as an AWS Serverless application, Kestrel is replaced
// with a Lambda function contained in the Amazon.Lambda.AspNetCoreServer package, which marshals the request into the ASP.NET Core hosting framework.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

var app = builder.Build();

app.UseCors(myAllowSpecificOrigins);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Welcome to running ASP.NET Core Minimal API on AWS Lambda");

app.Run();

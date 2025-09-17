using ChatClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.SemanticKernel.Extensions;
using System.Net.Http.Headers;

Console.WriteLine("Press enter to start");
Console.ReadLine();

// AI CV matching model credentials
var deploymentName = "gpt-4.1";
var endpoint = "<MODEL URL>";
var apiKey = "<API KEY>";

// kernel builder aanmaken
var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);

var token = await LoginHelper.Login("alice", "password");
builder.Services.AddHttpClient(); // default client
builder.Services.PostConfigure<HttpClientFactoryOptions>(options =>
{
    options.HttpClientActions.Add(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5000/");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    });
});
var httpClient = builder.Services.BuildServiceProvider().GetService<HttpClient>();

// kernel bouwen
Kernel kernel = builder.Build();
await kernel.Plugins.AddMcpFunctionsFromSseServerAsync("McpServerWithAuth", new Uri("http://localhost:5000"), null, httpClient);

// Parameters zetten zodat methodes uit plugins aangeroepen kunnen worden
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
};

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
var history = new ChatHistory();
string? userInput;
do
{
    Console.Write("User > ");
    userInput = Console.ReadLine();

    if (!string.IsNullOrEmpty(userInput))
    {
        // Input aan de historie toevoegen
        history.AddUserMessage(userInput!);

        //var result = await chatCompletionService.GetChatMessageContentAsync(userInput);

        var result = await chatCompletionService.GetChatMessageContentAsync(
            history,
            executionSettings: openAIPromptExecutionSettings,
            kernel: kernel);


        Console.WriteLine("Assistant > " + result);

        // Antwoord aan de historie toevoegen
        history.AddMessage(result.Role, result.Content ?? string.Empty);
    }
} while (!String.IsNullOrEmpty(userInput));
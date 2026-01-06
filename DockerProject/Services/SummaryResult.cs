using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DockerProject.Models;

namespace DockerProject.Services;

public class SummaryResult

{
    public string OverAll { get; set; } = string.Empty;

    public string Members { get; set; } = string.Empty;

    public string Tasks { get; set; } = string.Empty;

    public string ProblemsIdentifyInComments { get; set; } = string.Empty;

    public bool Success { get; set; } = false;

    public string? ErrorMessage { get; set; }
}

public interface ISummaryAnalysisService

{
    Task<SummaryResult> AnalysisServiceAsync(Project project);
}

public class GoogleSummaryAnalysisService : ISummaryAnalysisService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<GoogleSummaryAnalysisService> _logger;

    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/";
    private const string ModelName = "gemini-2.5-flash-lite";

    public GoogleSummaryAnalysisService(IConfiguration configuration, ILogger<GoogleSummaryAnalysisService> logger)
    {
        _httpClient = new HttpClient();
        // Se preia cheia din variabila de mediu, păstrând stilul implementării tale, 
        _apiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY") ??
                  throw new ArgumentNullException("GoogleAI: ApiKey not configured");
        _logger = logger;
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<SummaryResult> AnalysisServiceAsync(Project project)
    {
        try
        {
            var systemPrompt =
                @"You are an expert Technical Project Manager and Data Analyst AI. 
                Your task is to analyze a JSON representation of a 'Project' entity and generate a specific summary.

                INPUT CONTEXT:
                You will receive a Project object containing:
                - General Info (Title, Description, Fields)
                - Members (ProjectMember)
                - Tasks (ProjectTask) with Status (ToDo, InProgress, Review, Done)
                - Comments (User comments on tasks and the project)

                OUTPUT FORMAT:
                You must respond with ONLY a valid, parseable JSON object. Do not include markdown formatting (like ```json), explanations, or preambles.
                The JSON must adhere to this exact schema:
                {
                    ""OverAll"": ""string"",
                    ""Tasks"": ""string"",
                    ""Members"": ""string"",
                    ""ProblemsIdentifyInComments"": ""string""
                }

                INSTRUCTIONS FOR FIELDS:

                1. ""OverAll"": 
                   - Analyze the `Title`, `Description`, and attached `Fields`. 
                   - Synthesize the project's targets and critical path.
                   - Determine the current status (trajectory) based on the ratio of 'Done' vs 'ToDo' tasks.
                   - Summarize expectations for the immediate future.

                2. ""Tasks"": 
                   - Analyze `ProjectTask` items. Prioritize tasks modified or assigned recently (based on `AssignedDate` or `DoneDate`).
                   - Structure the response logically:
                     * [Status: Review]: Summarize work completed based on `Comments` within these tasks. Identify what is pending approval.
                     * [Status: ToDo]: Explain the objective of new tasks. Estimate their importance relative to the overall project scope and future impact.
                     * [Status: Done]: Explain what was accomplished and the specific value/impact added to the project.
                     * [Status: InProgress]: Acknowledge the start of these tasks. STRICTLY look for negative sentiment or blockers in the task's `Comments` and highlight them.

                3. ""Members"": 
                   - Analyze `ProjectMember` list and their interactions.
                   - Correlate members with `ProjectTask.Users` (assignments) and `Comment.Author`.
                   - Rank members by relevance based on:
                     a) Number of active/completed tasks recently.
                     b) Quality of contribution in `Comments` (constructive feedback vs. noise).
                   - Summarize the specific impact of key members.

                4. ""ProblemsIdentifyInComments"": 
                   - Scan all `Comments` (both Project-level and Task-level).
                   - Identify unresolved issues, conflicts, or technical blockers.
                   - Ignore casual conversation; focus on sentiment indicating frustration, delays, or bugs.

                RULES:
                - Escape all double quotes within the content strings to ensure valid JSON.
                - If a section has no relevant data (e.g., no problems found), state ""No major issues identified based on available data.""
                - Keep the tone professional, objective, and analytical.";

            var userPrompt = $"Analyze the JSON representation of this Project:\"{Helper.GetProjectJson(project)}\"";

            // Combinăm mesajele pentru Gemini
            var fullPrompt = $"{systemPrompt}\n\n{userPrompt}";

            var requestBody = new GoogleAiRequest
            {
                Contents = new List<GoogleAiContent>
                {
                    new GoogleAiContent
                    {
                        Parts = new List<GoogleAiPart>
                        {
                            new GoogleAiPart { Text = fullPrompt }
                        }
                    }
                },
                GenerationConfig = new GoogleAiGenerationConfig
                {
                    Temperature = 0.1,
                    MaxOutputTokens = 1024
                }
            };

            var jsonContent = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var requestUrl = $"{BaseUrl}{ModelName}:generateContent?key={_apiKey}";

            _logger.LogInformation("Sending request to Google AI API");

            var response = await _httpClient.PostAsync(requestUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Google AI API error: {StatusCode} {Content}", response.StatusCode, responseContent);
                return new SummaryResult
                {
                    Success = false,
                    ErrorMessage = $"API ERROR: {response.StatusCode}"
                };
            }

            var googleResponse = JsonSerializer.Deserialize<GoogleAiResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var assistantMessage = googleResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

            if (string.IsNullOrEmpty(assistantMessage))
            {
                return new SummaryResult
                {
                    Success = false,
                    ErrorMessage = "Empty response from Google AI API"
                };
            }

            _logger.LogInformation("Google AI response: {Response}", assistantMessage);

            var cleanedResponse = CleanJsonResponse(assistantMessage);

            var summaryData = JsonSerializer.Deserialize<SummaryResponse>(cleanedResponse);

            if (summaryData is null)
            {
                return new SummaryResult
                {
                    Success = false,
                    ErrorMessage = "Failed to parse summary JSON"
                };
            }

            return new SummaryResult
            {
                OverAll = summaryData.OverAll,
                Tasks = summaryData.Tasks,
                Members = summaryData.Members,
                ProblemsIdentifyInComments = summaryData.ProblemsIdentifyInComments,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing summary with Google AI");
            return new SummaryResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }


    private string CleanJsonResponse(string response)
    {
        var cleaned = response.Trim();

        if (cleaned.StartsWith("```json"))
        {
            cleaned = cleaned.Substring(7);
        }
        else if (cleaned.StartsWith("```"))
        {
            cleaned = cleaned.Substring(3);
        }

        if (cleaned.EndsWith("```"))
        {
            cleaned = cleaned.Substring(0, cleaned.Length - 3);
        }

        return cleaned.Trim();
    }
}

public class GoogleAiRequest
{
    [JsonPropertyName("contents")] public List<GoogleAiContent> Contents { get; set; } = new();

    [JsonPropertyName("generationConfig")] public GoogleAiGenerationConfig? GenerationConfig { get; set; }
}

public class GoogleAiContent
{
    [JsonPropertyName("parts")] public List<GoogleAiPart> Parts { get; set; } = new();
}

public class GoogleAiPart
{
    [JsonPropertyName("text")] public string Text { get; set; } = string.Empty;
}

public class GoogleAiGenerationConfig
{
    [JsonPropertyName("temperature")] public double Temperature { get; set; } = 0.7;

    [JsonPropertyName("maxOutputTokens")] public int MaxOutputTokens { get; set; } = 1024;
}

public class GoogleAiResponse
{
    [JsonPropertyName("candidates")] public List<GoogleAiCandidate>? Candidates { get; set; }
}

public class GoogleAiCandidate
{
    [JsonPropertyName("content")] public GoogleAiContent? Content { get; set; }
}

public class SummaryResponse

{
    [JsonPropertyName("OverAll")] public string OverAll { get; set; } = string.Empty;


    [JsonPropertyName("Members")] public string Members { get; set; } = string.Empty;


    [JsonPropertyName("Tasks")] public string Tasks { get; set; } = string.Empty;


    [JsonPropertyName("ProblemsIdentifyInComments")]

    public string ProblemsIdentifyInComments { get; set; } = string.Empty;
}
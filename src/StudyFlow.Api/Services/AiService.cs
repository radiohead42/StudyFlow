using System.ClientModel;
using Microsoft.EntityFrameworkCore;
using OpenAI;
using OpenAI.Chat;
using StudyFlow.Api.Data;
namespace StudyFlow.Api.Services;

public class AiService: IAiService
{

    private readonly IConfiguration config;
    private readonly StudyFlowDbContext dbContext;
    private readonly string endPoint;
    private readonly string apiKey;

    public AiService(IConfiguration config, StudyFlowDbContext dbContext)
    {
        this.config = config;
        this.dbContext = dbContext;
        endPoint =
            config["AZURE_AI_ENDPOINT"]
            ?? throw new Exception(
                    "AZURE_AI_ENDPOINT no configurado");
         
            apiKey =
            config["AZURE_AI_API_KEY"]
            ?? throw new Exception(
                    "AZURE_AI_API_KEY no configurado");
    }

    public async Task<string>TestAsync()
    {
        ChatClient client = new(
                model: "Phi-4-mini-instruct",
                credential: new ApiKeyCredential(apiKey),
                options: new OpenAIClientOptions
                {
                Endpoint = new Uri(endPoint)
                });
        ChatCompletion completion = await client.CompleteChatAsync(
                "Responde unicamente: StudyFlow API funcionando correctamente");

        return completion.Content[0].Text;
    }

    public async Task<string> GenerateStudyPlanAsync()
    {
        var tasks = await dbContext.Tasks
            .Where(t => t.Status == Models.Enums.StudyTaskStatus.Pending)
            .ToListAsync();

        ChatClient client = new(
                model: "Phi-4-mini-instruct",
                credential: new ApiKeyCredential(apiKey),
                options: new OpenAIClientOptions
                {
                Endpoint = new Uri(endPoint)
                });


        var taskInfo =
            string.Join(
                    Environment.NewLine,
                    tasks.Select(t =>
                        $"""
                        Tarea: {t.Title}
                        Prioridad: {t.Priority}
                        Fecha límite: {t.DueDate}
                        """));
                        var prompt =
                            $"""
                            Eres un asistente académico.

                            Analiza estas tareas:

                            {taskInfo}

                            Genera:
                            1. Qué debería estudiar primero.
                            2. Qué tareas son urgentes.
                            3. Un plan para los próximos 7 días.

                            Responde en español.
                            """;
            ChatCompletion completion =
            await client.CompleteChatAsync([new UserChatMessage(prompt)]);
            return completion.Content[0].Text;
    
    }
}

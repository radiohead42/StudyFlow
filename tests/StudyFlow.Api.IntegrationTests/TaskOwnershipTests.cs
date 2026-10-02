using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using StudyFlow.Api.IntegrationTests.Infrastructure;

namespace StudyFlow.Api.IntegrationTests;

public class TaskOwnershipTests(
        IntegrationTestFixture fixture)
    : IClassFixture<IntegrationTestFixture>
{
    [Fact]
    public async Task User_Cannot_Create_Task_In_Another_Users_Subject()
    {
        // Arrange
        var clientA =
            await TestUserHelper
            .CreateAuthenticatedClientAsync(fixture);

        var clientB =
            await TestUserHelper
            .CreateAuthenticatedClientAsync(fixture);

        // Usuario A crea su materia
        var subjectResponse =
            await clientA.PostAsJsonAsync(
                    "/api/subjects",
                    new
                    {
                    name = "Bases de Datos",
                    teacher = "Profesor A"
                    });

        subjectResponse.EnsureSuccessStatusCode();

        var subjectJson =
            await subjectResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var subjectId =
            subjectJson.GetProperty("id").GetInt32();

        // Act
        // Usuario B intenta crear una Task
        // dentro de la materia del Usuario A.
        var response =
            await clientB.PostAsJsonAsync(
                    "/api/tasks",
                    new
                    {
                    title = "Tarea invasora",
                    description = "No debería poder crearse",
                    dueDate = DateTime.UtcNow.AddDays(3),
                    priority = "High",
                    subjectId
                    });

        // Assert
        var responseBody =
            await response.Content.ReadAsStringAsync();

        Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                $"""
                Expected: 404 NotFound
                Actual: {(int)response.StatusCode} {response.StatusCode}

                Response body:
                {responseBody}
                """);
    }
}

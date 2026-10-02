using System.Net;
using System.Net.Http.Json;
using StudyFlow.Api.DTOs.Responses;
using StudyFlow.Api.IntegrationTests.Infrastructure;

namespace StudyFlow.Api.IntegrationTests;

public class SubjectOwnershipTests(
        IntegrationTestFixture fixture)
    : IClassFixture<IntegrationTestFixture>
{
    [Fact]
    public async Task User_Cannot_Read_Subject_Owned_By_Another_User()
    {
        // Arrange
        var clientA =
            await TestUserHelper
            .CreateAuthenticatedClientAsync(fixture);

        var clientB =
            await TestUserHelper
            .CreateAuthenticatedClientAsync(fixture);

        var createResponse =
            await clientA.PostAsJsonAsync(
                    "/api/subjects",
                    new
                    {
                    name = "Programación Web",
                    teacher = "Profesor A"
                    });

        createResponse.EnsureSuccessStatusCode();

        var subject =
            await createResponse.Content
            .ReadFromJsonAsync<SubjectResponse>();

        Assert.NotNull(subject);

        // Act
        var response =
            await clientB.GetAsync(
                    $"/api/subjects/{subject.Id}");

        // Assert
        Assert.Equal(
                HttpStatusCode.NotFound,
                response.StatusCode);
    }

    [Fact]
    public async Task User_Cannot_Update_Subject_Owned_By_Another_User()
    {
        // Arrange
        var clientA =
            await TestUserHelper
            .CreateAuthenticatedClientAsync(fixture);

        var clientB =
            await TestUserHelper
            .CreateAuthenticatedClientAsync(fixture);

        var createResponse =
            await clientA.PostAsJsonAsync(
                    "/api/subjects",
                    new
                    {
                    name = "Programación Web",
                    teacher = "Profesor A"
                    });

        createResponse.EnsureSuccessStatusCode();

        var subject =
            await createResponse.Content
            .ReadFromJsonAsync<SubjectResponse>();

        Assert.NotNull(subject);

        // Act
        var response =
            await clientB.PutAsJsonAsync(
                    $"/api/subjects/{subject.Id}",
                    new
                    {
                    name = "Materia Hackeada",
                    teacher = "Usuario B"
                    });

        // Assert
        Assert.Equal(
                HttpStatusCode.NotFound,
                response.StatusCode);
    }

    [Fact]
    public async Task User_Cannot_Delete_Subject_Owned_By_Another_User()
    {
        // Arrange
        var clientA =
            await TestUserHelper
            .CreateAuthenticatedClientAsync(fixture);

        var clientB =
            await TestUserHelper
            .CreateAuthenticatedClientAsync(fixture);

        var createResponse =
            await clientA.PostAsJsonAsync(
                    "/api/subjects",
                    new
                    {
                    name = "Bases de Datos",
                    teacher = "Profesor A"
                    });

        createResponse.EnsureSuccessStatusCode();

        var subject =
            await createResponse.Content
            .ReadFromJsonAsync<SubjectResponse>();

        Assert.NotNull(subject);

        // Act
        var response =
            await clientB.DeleteAsync(
                    $"/api/subjects/{subject.Id}");

        // Assert
        Assert.Equal(
                HttpStatusCode.NotFound,
                response.StatusCode);
    }

}

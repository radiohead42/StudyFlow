using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace StudyFlow.Api.IntegrationTests.Infrastructure;

public static class TestUserHelper
{
    public static async Task<HttpClient> CreateAuthenticatedClientAsync(
        IntegrationTestFixture fixture)
    {
        var email =
            $"user-{Guid.NewGuid()}@studyflow.test";

        const string password = "StudyFlow123!";

        var registerResponse =
            await fixture.Client.PostAsJsonAsync(
                "/api/auth/register",
                new
                {
                    email,
                    password
                });

        registerResponse.EnsureSuccessStatusCode();

        var loginResponse =
            await fixture.Client.PostAsJsonAsync(
                "/api/auth/login?useCookies=false",
                new
                {
                    email,
                    password
                });

        loginResponse.EnsureSuccessStatusCode();

        var json =
            await loginResponse.Content
                .ReadFromJsonAsync<JsonElement>();

        var accessToken =
            json
                .GetProperty("accessToken")
                .GetString();

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new InvalidOperationException(
                "Identity did not return an access token.");
        }

        var client =
            fixture.Factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        return client;
    }
}

using System.Net;
using StudyFlow.Api.IntegrationTests.Infrastructure;

namespace StudyFlow.Api.IntegrationTests;

public class AuthenticationTests(
    IntegrationTestFixture fixture)
    : IClassFixture<IntegrationTestFixture>
{
    [Fact]
    public async Task Dashboard_WithoutToken_ReturnsUnauthorized()
    {
        var response =
            await fixture.Client.GetAsync("/api/dashboard");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudyFlow.Api.Data;
using Testcontainers.PostgreSql;

namespace StudyFlow.Api.IntegrationTests.Infrastructure;

public sealed class IntegrationTestFixture
    : IAsyncLifetime
{
    private readonly PostgreSqlContainer postgreSqlContainer =
        new PostgreSqlBuilder("postgres:17-alpine")
            .WithDatabase("studyflow_tests")
            .WithUsername("studyflow")
            .WithPassword("studyflow")
            .Build();

    public StudyFlowWebApplicationFactory Factory { get; private set; }
        = null!;

    public HttpClient Client { get; private set; }
        = null!;

    public async Task InitializeAsync()
    {
        await postgreSqlContainer.StartAsync();

        Factory = new StudyFlowWebApplicationFactory(
            postgreSqlContainer.GetConnectionString());

        using var scope =
            Factory.Services.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<StudyFlowDbContext>();

        await dbContext.Database.MigrateAsync();

        Client = Factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();
        Factory?.Dispose();

        await postgreSqlContainer.DisposeAsync();
    }
}

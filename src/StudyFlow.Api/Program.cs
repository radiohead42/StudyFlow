using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Scalar.AspNetCore;
using StudyFlow.Api.Data;
using StudyFlow.Api.Models.Identity;
using StudyFlow.Api.OpenApi;
using StudyFlow.Api.Services;
using StudyFlow.Api.Services.CurrentUser;

var builder = WebApplication.CreateBuilder(args);

var sentryDsn =
    builder.Configuration["SENTRY_DSN"];

if (!string.IsNullOrWhiteSpace(sentryDsn))
{
    builder.WebHost.UseSentry(options =>
    {
        options.Dsn = sentryDsn;

        options.Environment =
            builder.Environment.EnvironmentName;

        options.Debug =
            builder.Environment.IsDevelopment();

        options.SendDefaultPii = false;
    });
}

// ------------------------------------------------------------
// Heroku / reverse proxy
// ------------------------------------------------------------

var isHeroku =
    !string.IsNullOrEmpty(
        Environment.GetEnvironmentVariable("DYNO"));

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;

    if (isHeroku)
    {
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
    }
});

builder.Services.AddHttpsRedirection(options =>
{
    if (isHeroku)
    {
        options.RedirectStatusCode =
            StatusCodes.Status308PermanentRedirect;

        options.HttpsPort = 443;
    }
});

// ------------------------------------------------------------
// Database
// ------------------------------------------------------------

builder.Services.AddDbContext<StudyFlowDbContext>(
    (serviceProvider, options) =>
    {
        var configuration =
            serviceProvider.GetRequiredService<IConfiguration>();

        var connectionString =
            ResolveDatabaseConnectionString(configuration);

        options.UseNpgsql(connectionString);
    });

// ------------------------------------------------------------
// Identity / Authentication / Authorization
// ------------------------------------------------------------

builder.Services.AddAuthorization();

builder.Services
    .AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<StudyFlowDbContext>();

builder.Services.AddHttpContextAccessor();

// ------------------------------------------------------------
// Application services
// ------------------------------------------------------------

builder.Services.AddControllers();

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddScoped<
    ICurrentUserService,
    CurrentUserService>();

// ------------------------------------------------------------
// OpenAPI
// ------------------------------------------------------------

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<
        BearerSecuritySchemeTransformer>();

    options.AddOperationTransformer<
        AuthOperationTransformer>();

    options.AddDocumentTransformer(
        (document, context, cancellationToken) =>
        {
            document.Info.Title = "StudyFlow API";
            document.Info.Version = "v1";

            document.Info.Description = """
                StudyFlow es una API REST para organizar
                materias y tareas de estudio.

                Permite crear materias, registrar tareas,
                consultar fechas de entrega y administrar
                el proceso académico.
                """;

            return Task.CompletedTask;
        });
});

// ------------------------------------------------------------
// ProblemDetails
// ------------------------------------------------------------

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] =
            context.HttpContext.TraceIdentifier;
    };
});

// ------------------------------------------------------------
// Rate limiting
// ------------------------------------------------------------

var authPermitLimit =
    builder.Configuration.GetValue<int?>(
        "RateLimiting:AuthPermitLimit")
    ?? 10;

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter(
        "auth",
        limiter =>
        {
            limiter.PermitLimit = authPermitLimit;
            limiter.Window =
                TimeSpan.FromMinutes(1);

            limiter.QueueLimit = 0;
            limiter.AutoReplenishment = true;
        });
});

// ------------------------------------------------------------
// Build
// ------------------------------------------------------------

var app = builder.Build();

// ------------------------------------------------------------
// Middleware pipeline
// ------------------------------------------------------------

// Heroku must be able to tell ASP.NET Core
// that the original request used HTTPS.
app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}

// Static documentation/assets.
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

// Explicit routing because endpoint-specific
// rate limiting is used.
app.UseRouting();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// ------------------------------------------------------------
// OpenAPI / Scalar
// ------------------------------------------------------------

var enableApiDocs =
    app.Environment.IsDevelopment() ||
    app.Configuration.GetValue<bool>("ENABLE_API_DOCS");

if (enableApiDocs)
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

// ------------------------------------------------------------
// Controllers
// ------------------------------------------------------------

app.MapControllers();

// ------------------------------------------------------------
// Identity endpoints
// ------------------------------------------------------------

var auth = app
    .MapGroup("/api/auth")
    .RequireRateLimiting("auth");

auth.MapIdentityApi<ApplicationUser>();

// ------------------------------------------------------------
// Root endpoint
// ------------------------------------------------------------

app.MapGet("/", () =>
    Results.Ok(new
    {
        name = "StudyFlow API",
        status = "running"
    }));

app.Run();

// ------------------------------------------------------------
// Database connection resolution
// ------------------------------------------------------------

static string ResolveDatabaseConnectionString(
    IConfiguration configuration)
{
    // Local development / tests
    var connectionString =
        configuration.GetConnectionString(
            "DefaultConnection");

    // Heroku production
    var databaseUrl =
        configuration["DATABASE_URL"];

    if (!string.IsNullOrWhiteSpace(databaseUrl))
    {
        var databaseUri =
            new Uri(databaseUrl);

        var userInfo =
            databaseUri.UserInfo.Split(':', 2);

        var username =
            Uri.UnescapeDataString(
                userInfo[0]);

        var password =
            userInfo.Length > 1
                ? Uri.UnescapeDataString(
                    userInfo[1])
                : string.Empty;

        var connectionStringBuilder =
            new NpgsqlConnectionStringBuilder
            {
                Host =
                    databaseUri.Host,

                Port =
                    databaseUri.Port,

                Username =
                    username,

                Password =
                    password,

                Database =
                    databaseUri
                        .AbsolutePath
                        .TrimStart('/'),

                SslMode =
                    SslMode.Require
            };

        return
            connectionStringBuilder
                .ConnectionString;
    }

    if (string.IsNullOrWhiteSpace(
            connectionString))
    {
        throw new InvalidOperationException(
            "Database connection string is not configured.");
    }

    return connectionString;
}

// Required by WebApplicationFactory<Program>
// in integration tests.
public partial class Program
{
}

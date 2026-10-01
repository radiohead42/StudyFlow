using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Data;
using StudyFlow.Api.Services;
using Npgsql;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using StudyFlow.Api.Models.Identity;
using StudyFlow.Api.OpenApi;
using StudyFlow.Api.Services.CurrentUser;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add redirection from headers for Heroku deployment
var isHeroku =
    !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DYNO"));

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

// Add Verification for API Live
builder.Services.AddHttpsRedirection(options =>
{
    if (isHeroku)
    {
        options.RedirectStatusCode =
            StatusCodes.Status308PermanentRedirect;

        options.HttpsPort = 443;
    }
});

// Add builder configuration for PostgreSQl
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var databaseUrl = builder.Configuration["DATABASE_URL"];

if (!string.IsNullOrEmpty(databaseUrl))
{
    var databaseUri = new Uri(databaseUrl);

    var userInfo = databaseUri.UserInfo.Split(':', 2);

    var username = Uri.UnescapeDataString(userInfo[0]);

    var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;

    var connectionStringBuilder =
        new NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.Port,
            Username = username,
            Password = password,
            Database = databaseUri.AbsolutePath.TrimStart('/'),
            SslMode = SslMode.Require
        };

    connectionString = connectionStringBuilder.ConnectionString;
}

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Database connection string is not configured.");
}

builder.Services.AddAuthorization();

builder.Services
    .AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<StudyFlowDbContext>();

builder.Services.AddDbContext<StudyFlowDbContext>(options => options.UseNpgsql(connectionString));

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi( options => 
        {
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        options.AddOperationTransformer<AuthOperationTransformer>();
        options.AddDocumentTransformer(
                (document, context, cancellationToken) =>
                {
                    document.Info.Title = "StudyFlow API";
                    document.Info.Version = "v1";
                    document.Info.Description = """
                        StudyFlow es una API REST para organizar
                        materias y tareas de estudio.

                        permite crear materias, registrar tareas,
                        consultar fechas de entrega y administrar
                        el proceso academico
                        """;
                    return Task.CompletedTask;
                });
        });

builder.Services.AddProblemDetails(option =>
        {
        option.CustomizeProblemDetails = context => 
        { 
            context.ProblemDetails.Extensions["traceId"] = 
                context.HttpContext.TraceIdentifier;
        };
});

builder.Services.AddRateLimiter( options => 
        {
            options.AddFixedWindowLimiter("auth", limiter => 
                    {
                        limiter.PermitLimit = 10;
                        limiter.Window = TimeSpan.FromMinutes(1);
                        limiter.QueueLimit = 0;
                        limiter.AutoReplenishment = true;
                     });
            });

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRateLimiter();

//Documentation for scalar API
var enableApiDocs = app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("ENABLE_API_DOCS");

// Configure the HTTP request pipeline.
if (enableApiDocs)
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseForwardedHeaders();

if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapIdentityApi<ApplicationUser>();

app.MapGet("/", () => Results.Ok(new
            {
            name = "StudyFlow API",
            status = "running"
            }));

app.Run();

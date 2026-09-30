using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Data;
using StudyFlow.Api.Services;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add builder configuration for PostgreSQl
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var databaseUrl = builder.Configuration["DatabaseUrl"];

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

builder.Services.AddDbContext<StudyFlowDbContext>(options => options.UseNpgsql(connectionString));

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Ok(new
            {
            name = "StudyFlow API",
            status = "running"
            }));

app.Run();

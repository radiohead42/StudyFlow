using System.Text.Json.Serialization;

namespace StudyFlow.Api.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<TaskPriority>))]
public enum TaskPriority
{
    Low,
    Medium,
    High
}

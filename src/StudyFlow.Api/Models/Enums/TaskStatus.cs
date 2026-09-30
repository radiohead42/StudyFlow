using System.Text.Json.Serialization;

namespace StudyFlow.Api.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<StudyTaskStatus>))]
public enum StudyTaskStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled
}

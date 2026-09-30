namespace StudyFlow.Api.DTOs.Responses;

public class DashboardResponse
{
    public int TotalTasks { get; set; }

    public int Pending { get; set; }

    public int InProgress { get; set; }

    public int Completed { get; set; }

    public int Cancelled { get; set; }

    public int Overdue { get; set; }

    public int DueNext7Days { get; set; }
}

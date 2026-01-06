namespace DockerProject.ViewModels;
            
public class TaskSummary
{
    public string Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }

    public bool IsOverdue => Deadline.HasValue && Deadline.Value < DateTime.Now;
    public int DaysUntilDeadline => Deadline.HasValue
        ? (int)(Deadline.Value - DateTime.Now).TotalDays
        : int.MaxValue;
}
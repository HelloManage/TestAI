namespace TestAI.Web.Models;

/// <summary>
/// Represents a task within a project.
/// Tasks can be assigned to team members and have priorities and time estimates.
/// </summary>
public class ProjectTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public WorkItemStatus Status { get; set; } = WorkItemStatus.Backlog;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid? AssigneeId { get; set; }
    public TeamMember? Assignee { get; set; }
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Calculates the variance between estimated and actual hours.
    /// Positive values indicate over-estimation, negative indicates under-estimation.
    /// </summary>
    public int HoursVariance => EstimatedHours - ActualHours;

    /// <summary>
    /// Determines if the task is overdue based on due date and completion status.
    /// </summary>
    public bool IsOverdue => DueDate.HasValue &&
                             DueDate.Value < DateTime.UtcNow &&
                             Status != WorkItemStatus.Completed;

    /// <summary>
    /// Marks the task as completed and records the completion time.
    /// </summary>
    public void Complete()
    {
        Status = WorkItemStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Assigns the task to a team member.
    /// Automatically moves task to Todo status if in Backlog.
    /// </summary>
    public void AssignTo(TeamMember member)
    {
        AssigneeId = member.Id;
        Assignee = member;
        if (Status == WorkItemStatus.Backlog)
        {
            Status = WorkItemStatus.Todo;
        }
    }
}

public enum WorkItemStatus
{
    Backlog,
    Todo,
    InProgress,
    InReview,
    Completed
}

public enum TaskPriority
{
    Low,
    Medium,
    High,
    Critical
}

namespace TestAI.Web.Models;

/// <summary>
/// Represents a software development project in the system.
/// Projects contain tasks, team members, and track overall progress.
/// </summary>
public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public List<ProjectTask> Tasks { get; set; } = new();
    public List<TeamMember> TeamMembers { get; set; } = new();

    /// <summary>
    /// Calculates the overall completion percentage based on completed tasks.
    /// </summary>
    public double CompletionPercentage
    {
        get
        {
            if (Tasks.Count == 0) return 0;
            return (double)Tasks.Count(t => t.Status == WorkItemStatus.Completed) / Tasks.Count * 100;
        }
    }

    /// <summary>
    /// Determines if the project is at risk based on due date and progress.
    /// A project is at risk if less than 50% complete with less than 25% time remaining.
    /// </summary>
    public bool IsAtRisk
    {
        get
        {
            if (!StartDate.HasValue || !DueDate.HasValue) return false;

            var totalDuration = (DueDate.Value - StartDate.Value).TotalDays;
            var elapsed = (DateTime.UtcNow - StartDate.Value).TotalDays;
            var timePercentage = elapsed / totalDuration * 100;

            return timePercentage > 75 && CompletionPercentage < 50;
        }
    }
}

public enum ProjectStatus
{
    Planning,
    InProgress,
    OnHold,
    Completed,
    Cancelled
}

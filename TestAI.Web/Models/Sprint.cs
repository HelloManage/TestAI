namespace TestAI.Web.Models;

/// <summary>
/// Represents a sprint (iteration) within a project.
/// Sprints group tasks into time-boxed development cycles.
/// </summary>
public class Sprint
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public SprintStatus Status { get; set; } = SprintStatus.Planning;
    public List<ProjectTask> Tasks { get; set; } = new();

    /// <summary>
    /// Gets the duration of the sprint in days.
    /// </summary>
    public int DurationDays => (EndDate - StartDate).Days;

    /// <summary>
    /// Calculates total story points (estimated hours) planned for the sprint.
    /// </summary>
    public int TotalPlannedHours => Tasks.Sum(t => t.EstimatedHours);

    /// <summary>
    /// Calculates completed story points (estimated hours) in the sprint.
    /// </summary>
    public int CompletedHours => Tasks
        .Where(t => t.Status == WorkItemStatus.Completed)
        .Sum(t => t.EstimatedHours);

    /// <summary>
    /// Calculates sprint velocity (completed hours per day).
    /// </summary>
    public double Velocity
    {
        get
        {
            if (Status == SprintStatus.Planning) return 0;

            var daysElapsed = (DateTime.UtcNow - StartDate).Days;
            if (daysElapsed <= 0) return 0;

            return (double)CompletedHours / daysElapsed;
        }
    }

    /// <summary>
    /// Predicts if the sprint goal will be met based on current velocity.
    /// </summary>
    public bool IsOnTrack
    {
        get
        {
            if (Status != SprintStatus.Active) return true;

            var remainingDays = (EndDate - DateTime.UtcNow).Days;
            var remainingHours = TotalPlannedHours - CompletedHours;

            if (remainingDays <= 0) return remainingHours <= 0;

            var requiredVelocity = (double)remainingHours / remainingDays;
            return Velocity >= requiredVelocity;
        }
    }

    /// <summary>
    /// Starts the sprint if it's in planning status.
    /// </summary>
    public bool Start()
    {
        if (Status != SprintStatus.Planning) return false;
        Status = SprintStatus.Active;
        return true;
    }

    /// <summary>
    /// Completes the sprint and moves incomplete tasks back to backlog.
    /// </summary>
    /// <returns>List of incomplete tasks that need to be moved.</returns>
    public List<ProjectTask> Complete()
    {
        Status = SprintStatus.Completed;
        return Tasks.Where(t => t.Status != WorkItemStatus.Completed).ToList();
    }
}

public enum SprintStatus
{
    Planning,
    Active,
    Completed
}

namespace TestAI.Web.Models;

/// <summary>
/// Represents a team member who can be assigned to projects and tasks.
/// Tracks workload and specialization areas.
/// </summary>
public class TeamMember
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TeamRole Role { get; set; } = TeamRole.Developer;
    public List<string> Skills { get; set; } = new();
    public int MaxHoursPerWeek { get; set; } = 40;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the full name of the team member.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Calculates the current workload based on assigned incomplete tasks.
    /// </summary>
    /// <param name="assignedTasks">List of tasks assigned to this member.</param>
    /// <returns>Total estimated hours of incomplete work.</returns>
    public int CalculateWorkload(IEnumerable<ProjectTask> assignedTasks)
    {
        return assignedTasks
            .Where(t => t.Status != WorkItemStatus.Completed)
            .Sum(t => t.EstimatedHours);
    }

    /// <summary>
    /// Determines if the team member has capacity for additional work.
    /// </summary>
    /// <param name="assignedTasks">List of tasks assigned to this member.</param>
    /// <param name="additionalHours">Hours of additional work to check.</param>
    /// <returns>True if member has capacity, false otherwise.</returns>
    public bool HasCapacity(IEnumerable<ProjectTask> assignedTasks, int additionalHours)
    {
        var currentWorkload = CalculateWorkload(assignedTasks);
        return currentWorkload + additionalHours <= MaxHoursPerWeek;
    }
}

public enum TeamRole
{
    Developer,
    SeniorDeveloper,
    TechLead,
    ProjectManager,
    Designer,
    QAEngineer,
    DevOps
}

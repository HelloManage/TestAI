using TestAI.Web.Models;

namespace TestAI.Web.Services;

/// <summary>
/// Service interface for managing projects.
/// Provides CRUD operations and project-specific business logic.
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// Gets all projects in the system.
    /// </summary>
    Task<IEnumerable<Project>> GetAllProjectsAsync();

    /// <summary>
    /// Gets a specific project by its identifier.
    /// </summary>
    Task<Project?> GetProjectByIdAsync(Guid id);

    /// <summary>
    /// Creates a new project.
    /// </summary>
    Task<Project> CreateProjectAsync(Project project);

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    Task<Project> UpdateProjectAsync(Project project);

    /// <summary>
    /// Deletes a project by its identifier.
    /// </summary>
    Task<bool> DeleteProjectAsync(Guid id);

    /// <summary>
    /// Gets project statistics including task counts and completion rates.
    /// </summary>
    Task<ProjectStatistics> GetProjectStatisticsAsync(Guid projectId);

    /// <summary>
    /// Adds a team member to a project.
    /// </summary>
    Task<bool> AddTeamMemberToProjectAsync(Guid projectId, Guid memberId);
}

/// <summary>
/// Contains aggregated statistics for a project.
/// </summary>
public record ProjectStatistics(
    int TotalTasks,
    int CompletedTasks,
    int OverdueTasks,
    int TeamMemberCount,
    double AverageTaskCompletionDays,
    Dictionary<TaskPriority, int> TasksByPriority
);

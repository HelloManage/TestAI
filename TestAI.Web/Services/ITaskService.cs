using TestAI.Web.Models;

namespace TestAI.Web.Services;

/// <summary>
/// Service interface for managing project tasks.
/// Provides task-specific operations and workflow management.
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Gets all tasks for a specific project.
    /// </summary>
    Task<IEnumerable<ProjectTask>> GetTasksByProjectAsync(Guid projectId);

    /// <summary>
    /// Gets a specific task by its identifier.
    /// </summary>
    Task<ProjectTask?> GetTaskByIdAsync(Guid taskId);

    /// <summary>
    /// Creates a new task within a project.
    /// </summary>
    Task<ProjectTask> CreateTaskAsync(Guid projectId, ProjectTask task);

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    Task<ProjectTask> UpdateTaskAsync(ProjectTask task);

    /// <summary>
    /// Deletes a task by its identifier.
    /// </summary>
    Task<bool> DeleteTaskAsync(Guid taskId);

    /// <summary>
    /// Changes the status of a task with workflow validation.
    /// </summary>
    Task<TaskTransitionResult> TransitionTaskStatusAsync(Guid taskId, WorkItemStatus newStatus);

    /// <summary>
    /// Assigns a task to a team member.
    /// </summary>
    Task<bool> AssignTaskAsync(Guid taskId, Guid memberId);

    /// <summary>
    /// Logs hours worked on a task.
    /// </summary>
    Task<ProjectTask> LogHoursAsync(Guid taskId, int hours);

    /// <summary>
    /// Gets tasks filtered by status.
    /// </summary>
    Task<IEnumerable<ProjectTask>> GetTasksByStatusAsync(Guid projectId, WorkItemStatus status);

    /// <summary>
    /// Gets overdue tasks for a project.
    /// </summary>
    Task<IEnumerable<ProjectTask>> GetOverdueTasksAsync(Guid projectId);
}

/// <summary>
/// Result of a task status transition attempt.
/// </summary>
public record TaskTransitionResult(
    bool Success,
    string? ErrorMessage,
    ProjectTask? Task
);

using TestAI.Web.Models;

namespace TestAI.Web.Services;

/// <summary>
/// Service interface for managing sprints.
/// Provides sprint planning, tracking, and retrospective capabilities.
/// </summary>
public interface ISprintService
{
    /// <summary>
    /// Gets all sprints for a project.
    /// </summary>
    Task<IEnumerable<Sprint>> GetSprintsByProjectAsync(Guid projectId);

    /// <summary>
    /// Gets the currently active sprint for a project.
    /// </summary>
    Task<Sprint?> GetActiveSprintAsync(Guid projectId);

    /// <summary>
    /// Creates a new sprint for a project.
    /// </summary>
    Task<Sprint> CreateSprintAsync(Guid projectId, Sprint sprint);

    /// <summary>
    /// Starts a sprint, making it active.
    /// </summary>
    Task<SprintOperationResult> StartSprintAsync(Guid sprintId);

    /// <summary>
    /// Completes a sprint and returns incomplete tasks.
    /// </summary>
    Task<SprintCompletionResult> CompleteSprintAsync(Guid sprintId);

    /// <summary>
    /// Adds a task to a sprint.
    /// </summary>
    Task<bool> AddTaskToSprintAsync(Guid sprintId, Guid taskId);

    /// <summary>
    /// Removes a task from a sprint.
    /// </summary>
    Task<bool> RemoveTaskFromSprintAsync(Guid sprintId, Guid taskId);

    /// <summary>
    /// Gets sprint burndown data for charting.
    /// </summary>
    Task<SprintBurndown> GetSprintBurndownAsync(Guid sprintId);
}

/// <summary>
/// Result of a sprint operation (start/complete).
/// </summary>
public record SprintOperationResult(
    bool Success,
    string? ErrorMessage,
    Sprint? Sprint
);

/// <summary>
/// Result of completing a sprint, including incomplete tasks.
/// </summary>
public record SprintCompletionResult(
    bool Success,
    Sprint? Sprint,
    IEnumerable<ProjectTask> IncompleteTasks
);

/// <summary>
/// Burndown chart data for a sprint.
/// </summary>
public record SprintBurndown(
    int TotalHours,
    int RemainingHours,
    List<BurndownPoint> DailyProgress,
    double IdealBurnRate
);

/// <summary>
/// A single point in the burndown chart.
/// </summary>
public record BurndownPoint(
    DateTime Date,
    int RemainingHours,
    int IdealRemaining
);

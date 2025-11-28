using TestAI.Web.Models;

namespace TestAI.Web.Services;

/// <summary>
/// Implementation of task management service.
/// Handles task lifecycle, assignments, and workflow transitions.
/// </summary>
public class TaskService : ITaskService
{
    private readonly IProjectService _projectService;

    /// <summary>
    /// Defines valid status transitions for task workflow.
    /// </summary>
    private static readonly Dictionary<WorkItemStatus, WorkItemStatus[]> ValidTransitions = new()
    {
        { WorkItemStatus.Backlog, new[] { WorkItemStatus.Todo } },
        { WorkItemStatus.Todo, new[] { WorkItemStatus.InProgress, WorkItemStatus.Backlog } },
        { WorkItemStatus.InProgress, new[] { WorkItemStatus.InReview, WorkItemStatus.Todo } },
        { WorkItemStatus.InReview, new[] { WorkItemStatus.Completed, WorkItemStatus.InProgress } },
        { WorkItemStatus.Completed, new[] { WorkItemStatus.InProgress } } // Allow reopening
    };

    public TaskService(IProjectService projectService)
    {
        _projectService = projectService;
    }

    public async Task<IEnumerable<ProjectTask>> GetTasksByProjectAsync(Guid projectId)
    {
        var project = await _projectService.GetProjectByIdAsync(projectId);
        return project?.Tasks ?? Enumerable.Empty<ProjectTask>();
    }

    public async Task<ProjectTask?> GetTaskByIdAsync(Guid taskId)
    {
        var projects = await _projectService.GetAllProjectsAsync();
        return projects
            .SelectMany(p => p.Tasks)
            .FirstOrDefault(t => t.Id == taskId);
    }

    public async Task<ProjectTask> CreateTaskAsync(Guid projectId, ProjectTask task)
    {
        var project = await _projectService.GetProjectByIdAsync(projectId);
        if (project == null)
        {
            throw new InvalidOperationException($"Project {projectId} not found");
        }

        task.Id = Guid.NewGuid();
        task.ProjectId = projectId;
        task.CreatedAt = DateTime.UtcNow;
        project.Tasks.Add(task);

        return task;
    }

    public async Task<ProjectTask> UpdateTaskAsync(ProjectTask task)
    {
        var existing = await GetTaskByIdAsync(task.Id);
        if (existing == null)
        {
            throw new InvalidOperationException($"Task {task.Id} not found");
        }

        // Update properties
        existing.Title = task.Title;
        existing.Description = task.Description;
        existing.Priority = task.Priority;
        existing.EstimatedHours = task.EstimatedHours;
        existing.DueDate = task.DueDate;
        existing.Tags = task.Tags;

        return existing;
    }

    public async Task<bool> DeleteTaskAsync(Guid taskId)
    {
        var projects = await _projectService.GetAllProjectsAsync();
        foreach (var project in projects)
        {
            var task = project.Tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                project.Tasks.Remove(task);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Transitions a task to a new status with workflow validation.
    /// Ensures only valid state transitions are allowed.
    /// </summary>
    public async Task<TaskTransitionResult> TransitionTaskStatusAsync(Guid taskId, WorkItemStatus newStatus)
    {
        var task = await GetTaskByIdAsync(taskId);
        if (task == null)
        {
            return new TaskTransitionResult(false, "Task not found", null);
        }

        // Check if transition is valid
        if (!IsValidTransition(task.Status, newStatus))
        {
            return new TaskTransitionResult(
                false,
                $"Cannot transition from {task.Status} to {newStatus}",
                task
            );
        }

        // Apply transition
        task.Status = newStatus;

        // Handle completion
        if (newStatus == WorkItemStatus.Completed)
        {
            task.CompletedAt = DateTime.UtcNow;
        }
        else if (task.CompletedAt.HasValue && newStatus != WorkItemStatus.Completed)
        {
            task.CompletedAt = null; // Reopened task
        }

        return new TaskTransitionResult(true, null, task);
    }

    public async Task<bool> AssignTaskAsync(Guid taskId, Guid memberId)
    {
        var task = await GetTaskByIdAsync(taskId);
        if (task == null) return false;

        var projects = await _projectService.GetAllProjectsAsync();
        var project = projects.FirstOrDefault(p => p.Id == task.ProjectId);
        var member = project?.TeamMembers.FirstOrDefault(m => m.Id == memberId);

        if (member == null) return false;

        task.AssignTo(member);
        return true;
    }

    public async Task<ProjectTask> LogHoursAsync(Guid taskId, int hours)
    {
        var task = await GetTaskByIdAsync(taskId);
        if (task == null)
        {
            throw new InvalidOperationException($"Task {taskId} not found");
        }

        task.ActualHours += hours;
        return task;
    }

    public async Task<IEnumerable<ProjectTask>> GetTasksByStatusAsync(Guid projectId, WorkItemStatus status)
    {
        var tasks = await GetTasksByProjectAsync(projectId);
        return tasks.Where(t => t.Status == status);
    }

    public async Task<IEnumerable<ProjectTask>> GetOverdueTasksAsync(Guid projectId)
    {
        var tasks = await GetTasksByProjectAsync(projectId);
        return tasks.Where(t => t.IsOverdue);
    }

    /// <summary>
    /// Validates if a status transition is allowed in the workflow.
    /// </summary>
    private static bool IsValidTransition(WorkItemStatus from, WorkItemStatus to)
    {
        if (from == to) return true;
        return ValidTransitions.TryGetValue(from, out var validTargets) &&
               validTargets.Contains(to);
    }
}

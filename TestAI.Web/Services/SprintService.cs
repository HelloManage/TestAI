using TestAI.Web.Models;

namespace TestAI.Web.Services;

/// <summary>
/// Implementation of sprint management service.
/// Handles sprint lifecycle and burndown tracking.
/// </summary>
public class SprintService : ISprintService
{
    private readonly List<Sprint> _sprints = new();
    private readonly IProjectService _projectService;
    private readonly ITaskService _taskService;

    public SprintService(IProjectService projectService, ITaskService taskService)
    {
        _projectService = projectService;
        _taskService = taskService;
    }

    public Task<IEnumerable<Sprint>> GetSprintsByProjectAsync(Guid projectId)
    {
        var sprints = _sprints.Where(s => s.ProjectId == projectId);
        return Task.FromResult(sprints);
    }

    public Task<Sprint?> GetActiveSprintAsync(Guid projectId)
    {
        var sprint = _sprints.FirstOrDefault(s =>
            s.ProjectId == projectId && s.Status == SprintStatus.Active);
        return Task.FromResult(sprint);
    }

    public async Task<Sprint> CreateSprintAsync(Guid projectId, Sprint sprint)
    {
        var project = await _projectService.GetProjectByIdAsync(projectId);
        if (project == null)
        {
            throw new InvalidOperationException($"Project {projectId} not found");
        }

        sprint.Id = Guid.NewGuid();
        sprint.ProjectId = projectId;
        sprint.Status = SprintStatus.Planning;
        _sprints.Add(sprint);

        return sprint;
    }

    /// <summary>
    /// Starts a sprint after validating no other sprint is active.
    /// </summary>
    public async Task<SprintOperationResult> StartSprintAsync(Guid sprintId)
    {
        var sprint = _sprints.FirstOrDefault(s => s.Id == sprintId);
        if (sprint == null)
        {
            return new SprintOperationResult(false, "Sprint not found", null);
        }

        // Check for existing active sprint
        var activeSprint = await GetActiveSprintAsync(sprint.ProjectId);
        if (activeSprint != null)
        {
            return new SprintOperationResult(
                false,
                $"Cannot start sprint: '{activeSprint.Name}' is already active",
                sprint
            );
        }

        if (!sprint.Start())
        {
            return new SprintOperationResult(
                false,
                "Sprint must be in Planning status to start",
                sprint
            );
        }

        return new SprintOperationResult(true, null, sprint);
    }

    /// <summary>
    /// Completes a sprint and identifies tasks that need to be moved to backlog.
    /// </summary>
    public Task<SprintCompletionResult> CompleteSprintAsync(Guid sprintId)
    {
        var sprint = _sprints.FirstOrDefault(s => s.Id == sprintId);
        if (sprint == null)
        {
            return Task.FromResult(new SprintCompletionResult(
                false,
                null,
                Enumerable.Empty<ProjectTask>()
            ));
        }

        var incompleteTasks = sprint.Complete();

        return Task.FromResult(new SprintCompletionResult(
            true,
            sprint,
            incompleteTasks
        ));
    }

    public async Task<bool> AddTaskToSprintAsync(Guid sprintId, Guid taskId)
    {
        var sprint = _sprints.FirstOrDefault(s => s.Id == sprintId);
        if (sprint == null || sprint.Status != SprintStatus.Planning)
        {
            return false;
        }

        var task = await _taskService.GetTaskByIdAsync(taskId);
        if (task == null || task.ProjectId != sprint.ProjectId)
        {
            return false;
        }

        if (!sprint.Tasks.Any(t => t.Id == taskId))
        {
            sprint.Tasks.Add(task);
        }

        return true;
    }

    public Task<bool> RemoveTaskFromSprintAsync(Guid sprintId, Guid taskId)
    {
        var sprint = _sprints.FirstOrDefault(s => s.Id == sprintId);
        if (sprint == null)
        {
            return Task.FromResult(false);
        }

        var task = sprint.Tasks.FirstOrDefault(t => t.Id == taskId);
        if (task != null)
        {
            sprint.Tasks.Remove(task);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    /// <summary>
    /// Generates burndown chart data for sprint progress visualization.
    /// </summary>
    public Task<SprintBurndown> GetSprintBurndownAsync(Guid sprintId)
    {
        var sprint = _sprints.FirstOrDefault(s => s.Id == sprintId);
        if (sprint == null)
        {
            return Task.FromResult(new SprintBurndown(0, 0, new List<BurndownPoint>(), 0));
        }

        var totalHours = sprint.TotalPlannedHours;
        var remainingHours = totalHours - sprint.CompletedHours;
        var idealBurnRate = (double)totalHours / sprint.DurationDays;

        // Generate daily progress points
        var dailyProgress = new List<BurndownPoint>();
        var currentDate = sprint.StartDate;
        var dayIndex = 0;

        while (currentDate <= DateTime.UtcNow && currentDate <= sprint.EndDate)
        {
            var idealRemaining = (int)(totalHours - (idealBurnRate * dayIndex));

            // Simulate actual progress (in real app, this would come from historical data)
            var actualRemaining = CalculateRemainingAtDate(sprint, currentDate);

            dailyProgress.Add(new BurndownPoint(currentDate, actualRemaining, idealRemaining));

            currentDate = currentDate.AddDays(1);
            dayIndex++;
        }

        return Task.FromResult(new SprintBurndown(
            totalHours,
            remainingHours,
            dailyProgress,
            idealBurnRate
        ));
    }

    /// <summary>
    /// Calculates remaining hours at a specific date based on task completion times.
    /// </summary>
    private int CalculateRemainingAtDate(Sprint sprint, DateTime date)
    {
        var completedByDate = sprint.Tasks
            .Where(t => t.CompletedAt.HasValue && t.CompletedAt.Value.Date <= date.Date)
            .Sum(t => t.EstimatedHours);

        return sprint.TotalPlannedHours - completedByDate;
    }
}

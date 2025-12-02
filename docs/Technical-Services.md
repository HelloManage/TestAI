# Services Layer

The services layer contains the business logic and orchestrates operations between the presentation and domain layers.

## IProjectService

Manages project lifecycle and aggregated statistics.

### Interface Definition

```csharp
public interface IProjectService
{
    Task<IEnumerable<Project>> GetAllProjectsAsync();
    Task<Project?> GetProjectByIdAsync(Guid id);
    Task<Project> CreateProjectAsync(Project project);
    Task<Project> UpdateProjectAsync(Project project);
    Task<bool> DeleteProjectAsync(Guid id);
    Task<ProjectStatistics> GetProjectStatisticsAsync(Guid projectId);
    Task AddTeamMemberToProjectAsync(Guid projectId, TeamMember member);
}
```

### Project Statistics

Returns comprehensive metrics about a project:

```csharp
public record ProjectStatistics(
    int TotalTasks,
    int CompletedTasks,
    int OverdueTasks,
    int TeamMemberCount,
    double AverageCompletionTimeHours,
    Dictionary<TaskPriority, int> TasksByPriority
);
```

### Usage Example

```csharp
@inject IProjectService ProjectService

var projects = await ProjectService.GetAllProjectsAsync();
var stats = await ProjectService.GetProjectStatisticsAsync(projectId);
```

---

## ITaskService

Manages individual tasks and workflow transitions.

### Interface Definition

```csharp
public interface ITaskService
{
    Task<IEnumerable<ProjectTask>> GetTasksByProjectAsync(Guid projectId);
    Task<ProjectTask?> GetTaskByIdAsync(Guid taskId);
    Task<ProjectTask> CreateTaskAsync(Guid projectId, ProjectTask task);
    Task<ProjectTask> UpdateTaskAsync(ProjectTask task);
    Task<bool> DeleteTaskAsync(Guid taskId);
    Task<TaskTransitionResult> TransitionTaskStatusAsync(Guid taskId, TaskStatus newStatus);
    Task AssignTaskAsync(Guid taskId, Guid memberId);
    Task LogHoursAsync(Guid taskId, double hours);
    Task<IEnumerable<ProjectTask>> GetTasksByStatusAsync(Guid projectId, TaskStatus status);
    Task<IEnumerable<ProjectTask>> GetOverdueTasksAsync(Guid projectId);
}
```

### Workflow Validation

The `TransitionTaskStatusAsync` method enforces valid status transitions:

| From | Allowed To |
|------|------------|
| Backlog | Todo |
| Todo | InProgress, Backlog |
| InProgress | Todo, InReview, Backlog |
| InReview | Completed, InProgress |
| Completed | InProgress (reopen) |

### Transition Result

```csharp
public record TaskTransitionResult(
    bool Success,
    string Message,
    TaskStatus? PreviousStatus,
    TaskStatus? NewStatus
);
```

### Usage Example

```csharp
@inject ITaskService TaskService

// Transition task to in progress
var result = await TaskService.TransitionTaskStatusAsync(taskId, TaskStatus.InProgress);
if (!result.Success)
{
    // Handle invalid transition
    Console.WriteLine(result.Message);
}

// Log work hours
await TaskService.LogHoursAsync(taskId, 4.5);
```

---

## ISprintService

Manages sprint planning, execution, and metrics.

### Interface Definition

```csharp
public interface ISprintService
{
    Task<IEnumerable<Sprint>> GetSprintsByProjectAsync(Guid projectId);
    Task<Sprint?> GetSprintByIdAsync(Guid sprintId);
    Task<Sprint> CreateSprintAsync(Guid projectId, Sprint sprint);
    Task<Sprint> UpdateSprintAsync(Sprint sprint);
    Task<bool> DeleteSprintAsync(Guid sprintId);
    Task<SprintStartResult> StartSprintAsync(Guid sprintId);
    Task<SprintCompleteResult> CompleteSprintAsync(Guid sprintId);
    Task AddTaskToSprintAsync(Guid sprintId, Guid taskId);
    Task RemoveTaskFromSprintAsync(Guid sprintId, Guid taskId);
    Task<BurndownData> GetSprintBurndownAsync(Guid sprintId);
}
```

### Sprint Constraints

- Only one sprint can be active at a time per project
- Tasks can only be added during Planning status
- Completing a sprint returns incomplete tasks for backlog

### Burndown Data

```csharp
public record BurndownData(
    List<BurndownPoint> IdealBurn,
    List<BurndownPoint> ActualBurn,
    double TotalHours,
    double RemainingHours
);

public record BurndownPoint(
    DateTime Date,
    double RemainingHours
);
```

### Usage Example

```csharp
@inject ISprintService SprintService

// Start a sprint (validates no other active sprint)
var result = await SprintService.StartSprintAsync(sprintId);

// Get burndown chart data
var burndown = await SprintService.GetSprintBurndownAsync(sprintId);

// Complete sprint and get incomplete tasks
var completion = await SprintService.CompleteSprintAsync(sprintId);
foreach (var task in completion.IncompleteTasks)
{
    // Move back to backlog
}
```

---

## Service Registration

Services are registered as singletons in `Program.cs`:

```csharp
builder.Services.AddSingleton<IProjectService, ProjectService>();
builder.Services.AddSingleton<ITaskService, TaskService>();
builder.Services.AddSingleton<ISprintService, SprintService>();
```

For production with database:
```csharp
builder.Services.AddScoped<IProjectService, ProjectService>();
```

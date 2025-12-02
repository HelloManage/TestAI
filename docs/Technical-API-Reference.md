# API Reference

This document provides a complete reference for all public APIs in the TestAI system.

## Project Operations

### Get All Projects

```csharp
Task<IEnumerable<Project>> GetAllProjectsAsync()
```

Returns all projects in the system.

**Returns**: Collection of `Project` entities

---

### Get Project by ID

```csharp
Task<Project?> GetProjectByIdAsync(Guid id)
```

Retrieves a specific project.

**Parameters**:
- `id`: Project unique identifier

**Returns**: `Project` or `null` if not found

---

### Create Project

```csharp
Task<Project> CreateProjectAsync(Project project)
```

Creates a new project.

**Parameters**:
- `project`: Project entity (Id will be generated)

**Returns**: Created `Project` with assigned Id

---

### Update Project

```csharp
Task<Project> UpdateProjectAsync(Project project)
```

Updates an existing project.

**Parameters**:
- `project`: Project entity with updated values

**Returns**: Updated `Project`

---

### Delete Project

```csharp
Task<bool> DeleteProjectAsync(Guid id)
```

Deletes a project.

**Parameters**:
- `id`: Project unique identifier

**Returns**: `true` if deleted, `false` if not found

---

### Get Project Statistics

```csharp
Task<ProjectStatistics> GetProjectStatisticsAsync(Guid projectId)
```

Retrieves aggregated metrics for a project.

**Parameters**:
- `projectId`: Project unique identifier

**Returns**: `ProjectStatistics` record

```csharp
record ProjectStatistics(
    int TotalTasks,
    int CompletedTasks,
    int OverdueTasks,
    int TeamMemberCount,
    double AverageCompletionTimeHours,
    Dictionary<TaskPriority, int> TasksByPriority
);
```

---

## Task Operations

### Get Tasks by Project

```csharp
Task<IEnumerable<ProjectTask>> GetTasksByProjectAsync(Guid projectId)
```

Returns all tasks for a project.

**Parameters**:
- `projectId`: Project unique identifier

**Returns**: Collection of `ProjectTask` entities

---

### Create Task

```csharp
Task<ProjectTask> CreateTaskAsync(Guid projectId, ProjectTask task)
```

Creates a new task in a project.

**Parameters**:
- `projectId`: Target project Id
- `task`: Task entity

**Returns**: Created `ProjectTask`

---

### Transition Task Status

```csharp
Task<TaskTransitionResult> TransitionTaskStatusAsync(Guid taskId, TaskStatus newStatus)
```

Attempts to change task status following workflow rules.

**Parameters**:
- `taskId`: Task unique identifier
- `newStatus`: Target status

**Returns**: `TaskTransitionResult`

```csharp
record TaskTransitionResult(
    bool Success,
    string Message,
    TaskStatus? PreviousStatus,
    TaskStatus? NewStatus
);
```

**Valid Transitions**:
| Current Status | Valid Next Status |
|----------------|-------------------|
| Backlog | Todo |
| Todo | InProgress, Backlog |
| InProgress | Todo, InReview, Backlog |
| InReview | Completed, InProgress |
| Completed | InProgress |

---

### Assign Task

```csharp
Task AssignTaskAsync(Guid taskId, Guid memberId)
```

Assigns a task to a team member.

**Parameters**:
- `taskId`: Task unique identifier
- `memberId`: Team member Id

**Side Effects**: Moves task from Backlog to Todo if unassigned

---

### Log Hours

```csharp
Task LogHoursAsync(Guid taskId, double hours)
```

Records actual hours worked on a task.

**Parameters**:
- `taskId`: Task unique identifier
- `hours`: Hours to add

---

### Get Overdue Tasks

```csharp
Task<IEnumerable<ProjectTask>> GetOverdueTasksAsync(Guid projectId)
```

Returns tasks past their due date.

**Parameters**:
- `projectId`: Project unique identifier

**Returns**: Collection of overdue `ProjectTask` entities

---

## Sprint Operations

### Get Sprints by Project

```csharp
Task<IEnumerable<Sprint>> GetSprintsByProjectAsync(Guid projectId)
```

Returns all sprints for a project.

**Parameters**:
- `projectId`: Project unique identifier

**Returns**: Collection of `Sprint` entities

---

### Start Sprint

```csharp
Task<SprintStartResult> StartSprintAsync(Guid sprintId)
```

Activates a sprint.

**Parameters**:
- `sprintId`: Sprint unique identifier

**Returns**: `SprintStartResult`

**Constraints**:
- Sprint must be in Planning status
- No other sprint can be Active

---

### Complete Sprint

```csharp
Task<SprintCompleteResult> CompleteSprintAsync(Guid sprintId)
```

Closes an active sprint.

**Parameters**:
- `sprintId`: Sprint unique identifier

**Returns**: `SprintCompleteResult` with list of incomplete tasks

---

### Add Task to Sprint

```csharp
Task AddTaskToSprintAsync(Guid sprintId, Guid taskId)
```

Adds a task to sprint backlog.

**Parameters**:
- `sprintId`: Sprint unique identifier
- `taskId`: Task unique identifier

**Constraints**: Sprint must be in Planning status

---

### Get Sprint Burndown

```csharp
Task<BurndownData> GetSprintBurndownAsync(Guid sprintId)
```

Returns burndown chart data.

**Parameters**:
- `sprintId`: Sprint unique identifier

**Returns**: `BurndownData`

```csharp
record BurndownData(
    List<BurndownPoint> IdealBurn,
    List<BurndownPoint> ActualBurn,
    double TotalHours,
    double RemainingHours
);

record BurndownPoint(
    DateTime Date,
    double RemainingHours
);
```

---

## Enumerations

### TaskStatus

```csharp
enum TaskStatus
{
    Backlog,
    Todo,
    InProgress,
    InReview,
    Completed
}
```

### TaskPriority

```csharp
enum TaskPriority
{
    Low,
    Medium,
    High,
    Critical
}
```

### ProjectStatus

```csharp
enum ProjectStatus
{
    Planning,
    InProgress,
    OnHold,
    Completed,
    Cancelled
}
```

### SprintStatus

```csharp
enum SprintStatus
{
    Planning,
    Active,
    Completed
}
```

### TeamRole

```csharp
enum TeamRole
{
    Developer,
    SeniorDeveloper,
    TechLead,
    ProjectManager,
    Designer,
    QAEngineer,
    DevOps
}
```

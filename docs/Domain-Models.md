# Domain Models

The domain layer contains the core business entities that represent the agile project management concepts.

## Project

Represents a software development project being managed.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | Unique identifier |
| `Name` | `string` | Project name |
| `Description` | `string` | Detailed description |
| `Status` | `ProjectStatus` | Current lifecycle state |
| `StartDate` | `DateTime` | Project start date |
| `DueDate` | `DateTime?` | Target completion date |
| `Tasks` | `List<ProjectTask>` | Associated tasks |
| `TeamMembers` | `List<TeamMember>` | Assigned team |

### Status Values

```csharp
public enum ProjectStatus
{
    Planning,      // Initial planning phase
    InProgress,    // Active development
    OnHold,        // Temporarily paused
    Completed,     // Successfully finished
    Cancelled      // Terminated
}
```

### Computed Properties

**CompletionPercentage**: Calculates progress based on completed tasks
```csharp
public double CompletionPercentage =>
    Tasks.Count == 0 ? 0 :
    (double)Tasks.Count(t => t.Status == TaskStatus.Completed) / Tasks.Count * 100;
```

**IsAtRisk**: Identifies at-risk projects (>75% time elapsed, <50% complete)

---

## ProjectTask

Individual work items within a project.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | Unique identifier |
| `Title` | `string` | Task title |
| `Description` | `string` | Task details |
| `Status` | `TaskStatus` | Workflow state |
| `Priority` | `TaskPriority` | Importance level |
| `EstimatedHours` | `double` | Planned effort |
| `ActualHours` | `double` | Recorded effort |
| `DueDate` | `DateTime?` | Deadline |
| `CompletedAt` | `DateTime?` | Completion timestamp |
| `AssigneeId` | `Guid?` | Assigned team member |
| `Tags` | `List<string>` | Categorization tags |

### Status Workflow

```
┌──────────┐
│ Backlog  │
└────┬─────┘
     │
     ▼
┌──────────┐     ┌────────────┐
│   Todo   │◄───►│ InProgress │
└────┬─────┘     └─────┬──────┘
     │                 │
     │                 ▼
     │          ┌──────────┐
     │          │ InReview │
     │          └─────┬────┘
     │                │
     ▼                ▼
┌─────────────────────────┐
│       Completed         │
└─────────────────────────┘
```

### Priority Levels

```csharp
public enum TaskPriority
{
    Low,
    Medium,
    High,
    Critical
}
```

### Key Methods

- `Complete()`: Marks task done with timestamp
- `AssignTo(Guid memberId)`: Assigns and moves from Backlog to Todo
- `IsOverdue`: Checks if past due date

---

## Sprint

Time-boxed development iteration.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | Unique identifier |
| `Name` | `string` | Sprint name (e.g., "Sprint 5") |
| `Goal` | `string` | Sprint objective |
| `Status` | `SprintStatus` | Current state |
| `StartDate` | `DateTime` | Sprint start |
| `EndDate` | `DateTime` | Sprint end |
| `Tasks` | `List<ProjectTask>` | Sprint backlog |

### Status Values

```csharp
public enum SprintStatus
{
    Planning,    // Backlog grooming
    Active,      // In progress
    Completed    // Finished
}
```

### Metrics

**Velocity**: Completed hours per day
```csharp
public double Velocity => DurationDays > 0 ? CompletedHours / DurationDays : 0;
```

**IsOnTrack**: Predicts if sprint goal will be met based on current velocity

### Key Methods

- `Start()`: Transitions to Active status
- `Complete()`: Closes sprint, returns incomplete tasks

---

## TeamMember

Team resource with skills and capacity.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `Guid` | Unique identifier |
| `Name` | `string` | Full name |
| `Email` | `string` | Contact email |
| `Role` | `TeamRole` | Job function |
| `Skills` | `List<string>` | Technical skills |
| `MaxHoursPerWeek` | `int` | Capacity limit |
| `JoinDate` | `DateTime` | Team join date |

### Roles

```csharp
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
```

### Capacity Methods

- `CalculateWorkload(tasks)`: Sums estimated hours of assigned incomplete tasks
- `HasCapacity(tasks, additionalHours)`: Validates available capacity

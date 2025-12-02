# Architecture Overview

TestAI follows a clean architecture pattern with clear separation of concerns between the presentation, business logic, and domain layers.

## Layer Architecture

```
┌─────────────────────────────────────┐
│         Blazor Components           │  Presentation Layer
│    (Pages, Layouts, Shared UI)      │
├─────────────────────────────────────┤
│           Services                  │  Business Logic Layer
│  (ProjectService, TaskService,      │
│   SprintService)                    │
├─────────────────────────────────────┤
│         Domain Models               │  Domain Layer
│  (Project, Task, Sprint, Team)      │
└─────────────────────────────────────┘
```

## Design Principles

### 1. Separation of Concerns

Each layer has distinct responsibilities:

- **Presentation**: User interface rendering and user interaction handling
- **Services**: Business rules, workflow orchestration, data operations
- **Domain**: Core business entities with intrinsic behavior

### 2. Dependency Injection

All services are registered in the DI container and injected where needed:

```csharp
builder.Services.AddSingleton<IProjectService, ProjectService>();
builder.Services.AddSingleton<ITaskService, TaskService>();
builder.Services.AddSingleton<ISprintService, SprintService>();
```

### 3. Interface Segregation

Services expose operations through interfaces, enabling:
- Testability through mocking
- Flexibility to swap implementations
- Clear contracts between layers

### 4. Async/Await Pattern

All service operations are asynchronous, preparing for:
- Database operations
- External API calls
- Improved scalability

## Component Interaction

```
User Action
    │
    ▼
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   Blazor    │────►│   Service   │────►│   Domain    │
│  Component  │◄────│   Layer     │◄────│   Model     │
└─────────────┘     └─────────────┘     └─────────────┘
    │                     │
    ▼                     ▼
 UI Update          State Change
```

## Data Flow

1. **User Interaction**: User triggers action in Blazor component
2. **Service Call**: Component calls injected service method
3. **Business Logic**: Service applies rules and validations
4. **Domain Operations**: Domain models enforce invariants
5. **State Update**: Changes persisted and returned
6. **UI Refresh**: Component re-renders with new state

## Current Storage

The application uses in-memory storage for demonstration:

```csharp
private readonly List<Project> _projects = new();
```

This can be replaced with:
- Entity Framework Core for SQL databases
- MongoDB for document storage
- External APIs for distributed systems

## Security Considerations

- Input validation at service layer
- State transitions validated before execution
- Nullable reference types for null safety

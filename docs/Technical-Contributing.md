# Contributing Guide

Thank you for your interest in contributing to TestAI! This guide will help you get started.

## Development Setup

### Prerequisites

1. Install [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
2. Install Git
3. Choose an IDE:
   - Visual Studio 2022
   - Visual Studio Code with C# extension
   - JetBrains Rider

### Clone and Build

```bash
git clone https://github.com/HelloManage/TestAI.git
cd TestAI
dotnet restore
dotnet build
```

### Run Tests

```bash
dotnet test
```

## Code Style

### General Guidelines

- Use meaningful, descriptive names
- Follow C# naming conventions (PascalCase for public members, camelCase for private)
- Keep methods focused and small
- Add XML documentation for public APIs

### File Organization

```
Feature/
├── IFeatureService.cs    # Interface
├── FeatureService.cs     # Implementation
└── FeatureModels.cs      # Related models (if needed)
```

### Example Service Pattern

```csharp
public interface IExampleService
{
    Task<Example?> GetByIdAsync(Guid id);
    Task<Example> CreateAsync(Example entity);
}

public class ExampleService : IExampleService
{
    private readonly List<Example> _items = new();

    public async Task<Example?> GetByIdAsync(Guid id)
    {
        await Task.CompletedTask;
        return _items.FirstOrDefault(x => x.Id == id);
    }

    public async Task<Example> CreateAsync(Example entity)
    {
        await Task.CompletedTask;
        entity.Id = Guid.NewGuid();
        _items.Add(entity);
        return entity;
    }
}
```

## Testing Requirements

### Test Coverage

All new features should include unit tests:

```csharp
public class ExampleServiceTests
{
    private readonly ExampleService _service;

    public ExampleServiceTests()
    {
        _service = new ExampleService();
    }

    [Fact]
    public async Task CreateAsync_ShouldGenerateId()
    {
        // Arrange
        var entity = new Example { Name = "Test" };

        // Act
        var result = await _service.CreateAsync(entity);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
    }
}
```

### Test Naming Convention

Use the pattern: `MethodName_Condition_ExpectedResult`

Examples:
- `GetByIdAsync_WithValidId_ReturnsEntity`
- `TransitionStatus_FromBacklogToTodo_Succeeds`
- `StartSprint_WhenAnotherActive_Fails`

## Pull Request Process

### 1. Create a Branch

```bash
git checkout -b feature/your-feature-name
```

### 2. Make Changes

- Follow code style guidelines
- Add tests for new functionality
- Update documentation if needed

### 3. Commit

Write clear commit messages:

```bash
git commit -m "Add sprint velocity calculation"
```

### 4. Push and Create PR

```bash
git push origin feature/your-feature-name
```

Then create a pull request on GitHub.

### PR Checklist

- [ ] Code follows style guidelines
- [ ] All tests pass
- [ ] New tests added for new features
- [ ] Documentation updated
- [ ] No breaking changes (or documented if necessary)

## Documentation

### Updating Wiki

Documentation lives in the `docs/` folder and syncs with the GitHub Wiki:

1. Edit markdown files in `docs/`
2. Commit and push changes
3. GitHub Actions will sync to wiki

Or edit wiki directly - changes sync back to `docs/`.

### Documentation Files

| File | Purpose |
|------|---------|
| `Home.md` | Wiki home page |
| `Architecture.md` | System design overview |
| `Domain-Models.md` | Entity documentation |
| `Services.md` | Service layer reference |
| `API-Reference.md` | Complete API docs |
| `Getting-Started.md` | Setup guide |
| `Contributing.md` | This file |

## Reporting Issues

### Bug Reports

Include:
- Description of the bug
- Steps to reproduce
- Expected vs actual behavior
- .NET version and OS

### Feature Requests

Include:
- Clear description of the feature
- Use case / motivation
- Proposed implementation (optional)

## Questions?

- Open a GitHub issue for questions
- Check existing issues first
- Be respectful and constructive

Thank you for contributing!

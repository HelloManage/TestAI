using TestAI.Web.Models;
using TestAI.Web.Services;

namespace TestAI.Web.Tests.Services;

public class TaskServiceTests
{
    private readonly TaskService _taskService;
    private readonly ProjectService _projectService;

    public TaskServiceTests()
    {
        _projectService = new ProjectService();
        _taskService = new TaskService(_projectService);
    }

    [Fact]
    public async Task GetTasksByProjectAsync_ReturnsProjectTasks()
    {
        // Arrange
        var projects = await _projectService.GetAllProjectsAsync();
        var project = projects.First(p => p.Tasks.Any());

        // Act
        var tasks = await _taskService.GetTasksByProjectAsync(project.Id);

        // Assert
        Assert.Equal(project.Tasks.Count, tasks.Count());
    }

    [Fact]
    public async Task GetTaskByIdAsync_WithValidId_ReturnsTask()
    {
        // Arrange
        var projects = await _projectService.GetAllProjectsAsync();
        var task = projects.SelectMany(p => p.Tasks).First();

        // Act
        var result = await _taskService.GetTaskByIdAsync(task.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(task.Id, result.Id);
    }

    [Fact]
    public async Task CreateTaskAsync_AddsTaskToProject()
    {
        // Arrange
        var projects = await _projectService.GetAllProjectsAsync();
        var project = projects.First();
        var initialCount = project.Tasks.Count;

        var newTask = new ProjectTask
        {
            Title = "New Test Task",
            Description = "Test Description",
            Priority = TaskPriority.High
        };

        // Act
        var created = await _taskService.CreateTaskAsync(project.Id, newTask);

        // Assert
        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal(project.Id, created.ProjectId);
        Assert.Equal(initialCount + 1, project.Tasks.Count);
    }

    [Fact]
    public async Task TransitionTaskStatusAsync_ValidTransition_Succeeds()
    {
        // Arrange
        var projects = await _projectService.GetAllProjectsAsync();
        var task = projects.SelectMany(p => p.Tasks)
            .First(t => t.Status == WorkItemStatus.Backlog);

        // Act
        var result = await _taskService.TransitionTaskStatusAsync(task.Id, WorkItemStatus.Todo);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.ErrorMessage);
        Assert.Equal(WorkItemStatus.Todo, result.Task?.Status);
    }

    [Fact]
    public async Task TransitionTaskStatusAsync_InvalidTransition_Fails()
    {
        // Arrange
        var projects = await _projectService.GetAllProjectsAsync();
        var task = projects.SelectMany(p => p.Tasks)
            .First(t => t.Status == WorkItemStatus.Backlog);

        // Act - Try to skip directly to Completed
        var result = await _taskService.TransitionTaskStatusAsync(task.Id, WorkItemStatus.Completed);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("Cannot transition", result.ErrorMessage);
    }

    [Fact]
    public async Task TransitionTaskStatusAsync_ToCompleted_SetsCompletedAt()
    {
        // Arrange
        var projects = await _projectService.GetAllProjectsAsync();
        var task = projects.SelectMany(p => p.Tasks)
            .First(t => t.Status == WorkItemStatus.InReview);

        // Act
        var result = await _taskService.TransitionTaskStatusAsync(task.Id, WorkItemStatus.Completed);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Task?.CompletedAt);
    }

    [Fact]
    public async Task LogHoursAsync_IncreasesActualHours()
    {
        // Arrange
        var projects = await _projectService.GetAllProjectsAsync();
        var task = projects.SelectMany(p => p.Tasks).First();
        var initialHours = task.ActualHours;

        // Act
        var result = await _taskService.LogHoursAsync(task.Id, 5);

        // Assert
        Assert.Equal(initialHours + 5, result.ActualHours);
    }

    [Fact]
    public async Task GetTasksByStatusAsync_FiltersCorrectly()
    {
        // Arrange
        var projects = await _projectService.GetAllProjectsAsync();
        var project = projects.First(p => p.Tasks.Any(t => t.Status == WorkItemStatus.Completed));

        // Act
        var completedTasks = await _taskService.GetTasksByStatusAsync(project.Id, WorkItemStatus.Completed);

        // Assert
        Assert.All(completedTasks, t => Assert.Equal(WorkItemStatus.Completed, t.Status));
    }

    [Fact]
    public async Task DeleteTaskAsync_RemovesTask()
    {
        // Arrange
        var projects = await _projectService.GetAllProjectsAsync();
        var project = projects.First();
        var newTask = await _taskService.CreateTaskAsync(project.Id, new ProjectTask { Title = "To Delete" });

        // Act
        var result = await _taskService.DeleteTaskAsync(newTask.Id);
        var deleted = await _taskService.GetTaskByIdAsync(newTask.Id);

        // Assert
        Assert.True(result);
        Assert.Null(deleted);
    }
}

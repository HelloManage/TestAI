using TestAI.Web.Models;
using TestAI.Web.Services;

namespace TestAI.Web.Tests.Services;

public class ProjectServiceTests
{
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        _service = new ProjectService();
    }

    [Fact]
    public async Task GetAllProjectsAsync_ReturnsSeededProjects()
    {
        // Act
        var projects = await _service.GetAllProjectsAsync();

        // Assert
        Assert.NotEmpty(projects);
        Assert.True(projects.Count() >= 2);
    }

    [Fact]
    public async Task GetProjectByIdAsync_WithValidId_ReturnsProject()
    {
        // Arrange
        var projects = await _service.GetAllProjectsAsync();
        var firstProject = projects.First();

        // Act
        var result = await _service.GetProjectByIdAsync(firstProject.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(firstProject.Id, result.Id);
        Assert.Equal(firstProject.Name, result.Name);
    }

    [Fact]
    public async Task GetProjectByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _service.GetProjectByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateProjectAsync_AddsNewProject()
    {
        // Arrange
        var initialCount = (await _service.GetAllProjectsAsync()).Count();
        var newProject = new Project
        {
            Name = "Test Project",
            Description = "Test Description"
        };

        // Act
        var created = await _service.CreateProjectAsync(newProject);
        var allProjects = await _service.GetAllProjectsAsync();

        // Assert
        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal(initialCount + 1, allProjects.Count());
    }

    [Fact]
    public async Task DeleteProjectAsync_WithValidId_ReturnsTrue()
    {
        // Arrange
        var newProject = await _service.CreateProjectAsync(new Project { Name = "To Delete" });

        // Act
        var result = await _service.DeleteProjectAsync(newProject.Id);

        // Assert
        Assert.True(result);
        var deleted = await _service.GetProjectByIdAsync(newProject.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteProjectAsync_WithInvalidId_ReturnsFalse()
    {
        // Act
        var result = await _service.DeleteProjectAsync(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetProjectStatisticsAsync_ReturnsCorrectStats()
    {
        // Arrange
        var projects = await _service.GetAllProjectsAsync();
        var projectWithTasks = projects.First(p => p.Tasks.Any());

        // Act
        var stats = await _service.GetProjectStatisticsAsync(projectWithTasks.Id);

        // Assert
        Assert.Equal(projectWithTasks.Tasks.Count, stats.TotalTasks);
        Assert.Equal(projectWithTasks.TeamMembers.Count, stats.TeamMemberCount);
        Assert.True(stats.CompletedTasks >= 0);
        Assert.True(stats.OverdueTasks >= 0);
    }

    [Fact]
    public async Task GetProjectStatisticsAsync_WithInvalidId_ReturnsEmptyStats()
    {
        // Act
        var stats = await _service.GetProjectStatisticsAsync(Guid.NewGuid());

        // Assert
        Assert.Equal(0, stats.TotalTasks);
        Assert.Equal(0, stats.CompletedTasks);
        Assert.Equal(0, stats.TeamMemberCount);
    }
}

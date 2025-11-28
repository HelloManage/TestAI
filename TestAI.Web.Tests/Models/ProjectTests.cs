using TestAI.Web.Models;

namespace TestAI.Web.Tests.Models;

public class ProjectTests
{
    [Fact]
    public void CompletionPercentage_WithNoTasks_ReturnsZero()
    {
        // Arrange
        var project = new Project { Tasks = new List<ProjectTask>() };

        // Act
        var result = project.CompletionPercentage;

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompletionPercentage_WithAllCompletedTasks_ReturnsHundred()
    {
        // Arrange
        var project = new Project
        {
            Tasks = new List<ProjectTask>
            {
                new() { Status = WorkItemStatus.Completed },
                new() { Status = WorkItemStatus.Completed },
                new() { Status = WorkItemStatus.Completed }
            }
        };

        // Act
        var result = project.CompletionPercentage;

        // Assert
        Assert.Equal(100, result);
    }

    [Fact]
    public void CompletionPercentage_WithMixedTasks_ReturnsCorrectPercentage()
    {
        // Arrange
        var project = new Project
        {
            Tasks = new List<ProjectTask>
            {
                new() { Status = WorkItemStatus.Completed },
                new() { Status = WorkItemStatus.InProgress },
                new() { Status = WorkItemStatus.Todo },
                new() { Status = WorkItemStatus.Backlog }
            }
        };

        // Act
        var result = project.CompletionPercentage;

        // Assert
        Assert.Equal(25, result);
    }

    [Fact]
    public void IsAtRisk_WithNoDateSet_ReturnsFalse()
    {
        // Arrange
        var project = new Project();

        // Act & Assert
        Assert.False(project.IsAtRisk);
    }

    [Fact]
    public void IsAtRisk_WithPlentyOfTimeRemaining_ReturnsFalse()
    {
        // Arrange
        var project = new Project
        {
            StartDate = DateTime.UtcNow.AddDays(-10),
            DueDate = DateTime.UtcNow.AddDays(90),
            Tasks = new List<ProjectTask>
            {
                new() { Status = WorkItemStatus.InProgress }
            }
        };

        // Act & Assert
        Assert.False(project.IsAtRisk);
    }

    [Fact]
    public void IsAtRisk_WithLowProgressAndLittleTime_ReturnsTrue()
    {
        // Arrange
        var project = new Project
        {
            StartDate = DateTime.UtcNow.AddDays(-80),
            DueDate = DateTime.UtcNow.AddDays(5),
            Tasks = new List<ProjectTask>
            {
                new() { Status = WorkItemStatus.Completed },
                new() { Status = WorkItemStatus.InProgress },
                new() { Status = WorkItemStatus.Todo },
                new() { Status = WorkItemStatus.Backlog }
            }
        };

        // Act & Assert
        Assert.True(project.IsAtRisk);
    }
}

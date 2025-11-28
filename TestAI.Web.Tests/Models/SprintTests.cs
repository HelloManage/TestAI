using TestAI.Web.Models;

namespace TestAI.Web.Tests.Models;

public class SprintTests
{
    [Fact]
    public void DurationDays_CalculatesCorrectly()
    {
        // Arrange
        var sprint = new Sprint
        {
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 1, 15)
        };

        // Act
        var duration = sprint.DurationDays;

        // Assert
        Assert.Equal(14, duration);
    }

    [Fact]
    public void TotalPlannedHours_SumsAllTaskEstimates()
    {
        // Arrange
        var sprint = new Sprint
        {
            Tasks = new List<ProjectTask>
            {
                new() { EstimatedHours = 10 },
                new() { EstimatedHours = 20 },
                new() { EstimatedHours = 15 }
            }
        };

        // Act
        var total = sprint.TotalPlannedHours;

        // Assert
        Assert.Equal(45, total);
    }

    [Fact]
    public void CompletedHours_OnlyCountsCompletedTasks()
    {
        // Arrange
        var sprint = new Sprint
        {
            Tasks = new List<ProjectTask>
            {
                new() { EstimatedHours = 10, Status = WorkItemStatus.Completed },
                new() { EstimatedHours = 20, Status = WorkItemStatus.InProgress },
                new() { EstimatedHours = 15, Status = WorkItemStatus.Completed }
            }
        };

        // Act
        var completed = sprint.CompletedHours;

        // Assert
        Assert.Equal(25, completed);
    }

    [Fact]
    public void Velocity_WhenPlanning_ReturnsZero()
    {
        // Arrange
        var sprint = new Sprint
        {
            Status = SprintStatus.Planning,
            StartDate = DateTime.UtcNow.AddDays(-5)
        };

        // Act
        var velocity = sprint.Velocity;

        // Assert
        Assert.Equal(0, velocity);
    }

    [Fact]
    public void Start_WhenPlanning_ChangesStatusToActive()
    {
        // Arrange
        var sprint = new Sprint { Status = SprintStatus.Planning };

        // Act
        var result = sprint.Start();

        // Assert
        Assert.True(result);
        Assert.Equal(SprintStatus.Active, sprint.Status);
    }

    [Fact]
    public void Start_WhenNotPlanning_ReturnsFalse()
    {
        // Arrange
        var sprint = new Sprint { Status = SprintStatus.Active };

        // Act
        var result = sprint.Start();

        // Assert
        Assert.False(result);
        Assert.Equal(SprintStatus.Active, sprint.Status);
    }

    [Fact]
    public void Complete_ReturnsIncompleteTasks()
    {
        // Arrange
        var incompleteTask1 = new ProjectTask { Status = WorkItemStatus.InProgress };
        var incompleteTask2 = new ProjectTask { Status = WorkItemStatus.Todo };
        var completedTask = new ProjectTask { Status = WorkItemStatus.Completed };

        var sprint = new Sprint
        {
            Status = SprintStatus.Active,
            Tasks = new List<ProjectTask> { incompleteTask1, completedTask, incompleteTask2 }
        };

        // Act
        var incompleteTasks = sprint.Complete();

        // Assert
        Assert.Equal(SprintStatus.Completed, sprint.Status);
        Assert.Equal(2, incompleteTasks.Count);
        Assert.Contains(incompleteTask1, incompleteTasks);
        Assert.Contains(incompleteTask2, incompleteTasks);
        Assert.DoesNotContain(completedTask, incompleteTasks);
    }
}

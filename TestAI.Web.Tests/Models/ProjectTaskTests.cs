using TestAI.Web.Models;

namespace TestAI.Web.Tests.Models;

public class ProjectTaskTests
{
    [Fact]
    public void HoursVariance_WhenUnderEstimated_ReturnsNegative()
    {
        // Arrange
        var task = new ProjectTask
        {
            EstimatedHours = 10,
            ActualHours = 15
        };

        // Act
        var variance = task.HoursVariance;

        // Assert
        Assert.Equal(-5, variance);
    }

    [Fact]
    public void HoursVariance_WhenOverEstimated_ReturnsPositive()
    {
        // Arrange
        var task = new ProjectTask
        {
            EstimatedHours = 20,
            ActualHours = 15
        };

        // Act
        var variance = task.HoursVariance;

        // Assert
        Assert.Equal(5, variance);
    }

    [Fact]
    public void IsOverdue_WhenPastDueDateAndNotCompleted_ReturnsTrue()
    {
        // Arrange
        var task = new ProjectTask
        {
            DueDate = DateTime.UtcNow.AddDays(-1),
            Status = WorkItemStatus.InProgress
        };

        // Act & Assert
        Assert.True(task.IsOverdue);
    }

    [Fact]
    public void IsOverdue_WhenPastDueDateButCompleted_ReturnsFalse()
    {
        // Arrange
        var task = new ProjectTask
        {
            DueDate = DateTime.UtcNow.AddDays(-1),
            Status = WorkItemStatus.Completed
        };

        // Act & Assert
        Assert.False(task.IsOverdue);
    }

    [Fact]
    public void IsOverdue_WhenNoDueDate_ReturnsFalse()
    {
        // Arrange
        var task = new ProjectTask
        {
            DueDate = null,
            Status = WorkItemStatus.InProgress
        };

        // Act & Assert
        Assert.False(task.IsOverdue);
    }

    [Fact]
    public void IsOverdue_WhenBeforeDueDate_ReturnsFalse()
    {
        // Arrange
        var task = new ProjectTask
        {
            DueDate = DateTime.UtcNow.AddDays(5),
            Status = WorkItemStatus.InProgress
        };

        // Act & Assert
        Assert.False(task.IsOverdue);
    }

    [Fact]
    public void Complete_SetsStatusAndCompletedAt()
    {
        // Arrange
        var task = new ProjectTask { Status = WorkItemStatus.InProgress };
        var beforeComplete = DateTime.UtcNow;

        // Act
        task.Complete();

        // Assert
        Assert.Equal(WorkItemStatus.Completed, task.Status);
        Assert.NotNull(task.CompletedAt);
        Assert.True(task.CompletedAt >= beforeComplete);
    }

    [Fact]
    public void AssignTo_SetsAssigneeAndMovesFromBacklog()
    {
        // Arrange
        var task = new ProjectTask { Status = WorkItemStatus.Backlog };
        var member = new TeamMember { FirstName = "John", LastName = "Doe" };

        // Act
        task.AssignTo(member);

        // Assert
        Assert.Equal(member.Id, task.AssigneeId);
        Assert.Equal(member, task.Assignee);
        Assert.Equal(WorkItemStatus.Todo, task.Status);
    }

    [Fact]
    public void AssignTo_DoesNotChangeStatusIfNotBacklog()
    {
        // Arrange
        var task = new ProjectTask { Status = WorkItemStatus.InProgress };
        var member = new TeamMember { FirstName = "John", LastName = "Doe" };

        // Act
        task.AssignTo(member);

        // Assert
        Assert.Equal(WorkItemStatus.InProgress, task.Status);
    }
}

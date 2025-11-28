using TestAI.Web.Models;

namespace TestAI.Web.Tests.Models;

public class TeamMemberTests
{
    [Fact]
    public void FullName_ReturnsCombinedName()
    {
        // Arrange
        var member = new TeamMember
        {
            FirstName = "Jane",
            LastName = "Smith"
        };

        // Act
        var fullName = member.FullName;

        // Assert
        Assert.Equal("Jane Smith", fullName);
    }

    [Fact]
    public void CalculateWorkload_ExcludesCompletedTasks()
    {
        // Arrange
        var member = new TeamMember();
        var tasks = new List<ProjectTask>
        {
            new() { EstimatedHours = 10, Status = WorkItemStatus.InProgress },
            new() { EstimatedHours = 5, Status = WorkItemStatus.Completed },
            new() { EstimatedHours = 8, Status = WorkItemStatus.Todo }
        };

        // Act
        var workload = member.CalculateWorkload(tasks);

        // Assert
        Assert.Equal(18, workload); // 10 + 8, excluding completed
    }

    [Fact]
    public void CalculateWorkload_WithNoTasks_ReturnsZero()
    {
        // Arrange
        var member = new TeamMember();
        var tasks = new List<ProjectTask>();

        // Act
        var workload = member.CalculateWorkload(tasks);

        // Assert
        Assert.Equal(0, workload);
    }

    [Fact]
    public void HasCapacity_WhenUnderMax_ReturnsTrue()
    {
        // Arrange
        var member = new TeamMember { MaxHoursPerWeek = 40 };
        var tasks = new List<ProjectTask>
        {
            new() { EstimatedHours = 20, Status = WorkItemStatus.InProgress }
        };

        // Act
        var hasCapacity = member.HasCapacity(tasks, 15);

        // Assert
        Assert.True(hasCapacity);
    }

    [Fact]
    public void HasCapacity_WhenAtMax_ReturnsTrue()
    {
        // Arrange
        var member = new TeamMember { MaxHoursPerWeek = 40 };
        var tasks = new List<ProjectTask>
        {
            new() { EstimatedHours = 30, Status = WorkItemStatus.InProgress }
        };

        // Act
        var hasCapacity = member.HasCapacity(tasks, 10);

        // Assert
        Assert.True(hasCapacity);
    }

    [Fact]
    public void HasCapacity_WhenOverMax_ReturnsFalse()
    {
        // Arrange
        var member = new TeamMember { MaxHoursPerWeek = 40 };
        var tasks = new List<ProjectTask>
        {
            new() { EstimatedHours = 35, Status = WorkItemStatus.InProgress }
        };

        // Act
        var hasCapacity = member.HasCapacity(tasks, 10);

        // Assert
        Assert.False(hasCapacity);
    }
}

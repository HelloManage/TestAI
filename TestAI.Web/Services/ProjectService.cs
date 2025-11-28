using TestAI.Web.Models;

namespace TestAI.Web.Services;

/// <summary>
/// In-memory implementation of the project service.
/// Provides sample data and business logic for demonstration purposes.
/// </summary>
public class ProjectService : IProjectService
{
    private readonly List<Project> _projects = new();
    private readonly List<TeamMember> _teamMembers = new();

    public ProjectService()
    {
        SeedSampleData();
    }

    public Task<IEnumerable<Project>> GetAllProjectsAsync()
    {
        return Task.FromResult<IEnumerable<Project>>(_projects);
    }

    public Task<Project?> GetProjectByIdAsync(Guid id)
    {
        var project = _projects.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(project);
    }

    public Task<Project> CreateProjectAsync(Project project)
    {
        project.Id = Guid.NewGuid();
        project.CreatedAt = DateTime.UtcNow;
        _projects.Add(project);
        return Task.FromResult(project);
    }

    public Task<Project> UpdateProjectAsync(Project project)
    {
        var existing = _projects.FirstOrDefault(p => p.Id == project.Id);
        if (existing != null)
        {
            var index = _projects.IndexOf(existing);
            _projects[index] = project;
        }
        return Task.FromResult(project);
    }

    public Task<bool> DeleteProjectAsync(Guid id)
    {
        var project = _projects.FirstOrDefault(p => p.Id == id);
        if (project == null) return Task.FromResult(false);

        _projects.Remove(project);
        return Task.FromResult(true);
    }

    public Task<ProjectStatistics> GetProjectStatisticsAsync(Guid projectId)
    {
        var project = _projects.FirstOrDefault(p => p.Id == projectId);
        if (project == null)
        {
            return Task.FromResult(new ProjectStatistics(0, 0, 0, 0, 0, new Dictionary<TaskPriority, int>()));
        }

        var completedTasks = project.Tasks.Where(t => t.Status == WorkItemStatus.Completed).ToList();
        var avgCompletionDays = completedTasks.Any()
            ? completedTasks.Average(t => t.CompletedAt.HasValue
                ? (t.CompletedAt.Value - t.CreatedAt).TotalDays
                : 0)
            : 0;

        var tasksByPriority = project.Tasks
            .GroupBy(t => t.Priority)
            .ToDictionary(g => g.Key, g => g.Count());

        var stats = new ProjectStatistics(
            TotalTasks: project.Tasks.Count,
            CompletedTasks: completedTasks.Count,
            OverdueTasks: project.Tasks.Count(t => t.IsOverdue),
            TeamMemberCount: project.TeamMembers.Count,
            AverageTaskCompletionDays: avgCompletionDays,
            TasksByPriority: tasksByPriority
        );

        return Task.FromResult(stats);
    }

    public Task<bool> AddTeamMemberToProjectAsync(Guid projectId, Guid memberId)
    {
        var project = _projects.FirstOrDefault(p => p.Id == projectId);
        var member = _teamMembers.FirstOrDefault(m => m.Id == memberId);

        if (project == null || member == null) return Task.FromResult(false);

        if (!project.TeamMembers.Any(m => m.Id == memberId))
        {
            project.TeamMembers.Add(member);
        }

        return Task.FromResult(true);
    }

    /// <summary>
    /// Seeds the service with sample data for demonstration purposes.
    /// Creates realistic project management scenarios.
    /// </summary>
    private void SeedSampleData()
    {
        // Create team members
        var alice = new TeamMember
        {
            FirstName = "Alice",
            LastName = "Johnson",
            Email = "alice@company.com",
            Role = TeamRole.TechLead,
            Skills = new List<string> { "C#", "Azure", "Architecture" }
        };

        var bob = new TeamMember
        {
            FirstName = "Bob",
            LastName = "Smith",
            Email = "bob@company.com",
            Role = TeamRole.SeniorDeveloper,
            Skills = new List<string> { "C#", "Blazor", "SQL" }
        };

        var carol = new TeamMember
        {
            FirstName = "Carol",
            LastName = "Williams",
            Email = "carol@company.com",
            Role = TeamRole.Developer,
            Skills = new List<string> { "JavaScript", "React", "CSS" }
        };

        var dave = new TeamMember
        {
            FirstName = "Dave",
            LastName = "Brown",
            Email = "dave@company.com",
            Role = TeamRole.QAEngineer,
            Skills = new List<string> { "Testing", "Automation", "Selenium" }
        };

        _teamMembers.AddRange(new[] { alice, bob, carol, dave });

        // Create sample project
        var project = new Project
        {
            Name = "Customer Portal Redesign",
            Description = "Complete redesign of the customer-facing portal with modern UI/UX and improved performance.",
            Status = ProjectStatus.InProgress,
            StartDate = DateTime.UtcNow.AddDays(-30),
            DueDate = DateTime.UtcNow.AddDays(60),
            TeamMembers = new List<TeamMember> { alice, bob, carol, dave }
        };

        // Add tasks to project
        project.Tasks = new List<ProjectTask>
        {
            new()
            {
                ProjectId = project.Id,
                Title = "Design System Setup",
                Description = "Create a comprehensive design system with reusable components",
                Status = WorkItemStatus.Completed,
                Priority = TaskPriority.High,
                EstimatedHours = 24,
                ActualHours = 28,
                CompletedAt = DateTime.UtcNow.AddDays(-20),
                AssigneeId = carol.Id,
                Assignee = carol,
                Tags = new List<string> { "design", "foundation" }
            },
            new()
            {
                ProjectId = project.Id,
                Title = "API Gateway Implementation",
                Description = "Implement API gateway for backend services integration",
                Status = WorkItemStatus.Completed,
                Priority = TaskPriority.Critical,
                EstimatedHours = 40,
                ActualHours = 35,
                CompletedAt = DateTime.UtcNow.AddDays(-10),
                AssigneeId = alice.Id,
                Assignee = alice,
                Tags = new List<string> { "backend", "api" }
            },
            new()
            {
                ProjectId = project.Id,
                Title = "User Dashboard",
                Description = "Build interactive user dashboard with analytics widgets",
                Status = WorkItemStatus.InProgress,
                Priority = TaskPriority.High,
                EstimatedHours = 32,
                ActualHours = 20,
                DueDate = DateTime.UtcNow.AddDays(7),
                AssigneeId = bob.Id,
                Assignee = bob,
                Tags = new List<string> { "frontend", "dashboard" }
            },
            new()
            {
                ProjectId = project.Id,
                Title = "Authentication Flow",
                Description = "Implement OAuth2 authentication with social login options",
                Status = WorkItemStatus.InReview,
                Priority = TaskPriority.Critical,
                EstimatedHours = 24,
                ActualHours = 22,
                DueDate = DateTime.UtcNow.AddDays(3),
                AssigneeId = alice.Id,
                Assignee = alice,
                Tags = new List<string> { "security", "auth" }
            },
            new()
            {
                ProjectId = project.Id,
                Title = "Performance Testing",
                Description = "Conduct load testing and performance optimization",
                Status = WorkItemStatus.Todo,
                Priority = TaskPriority.Medium,
                EstimatedHours = 16,
                DueDate = DateTime.UtcNow.AddDays(14),
                AssigneeId = dave.Id,
                Assignee = dave,
                Tags = new List<string> { "testing", "performance" }
            },
            new()
            {
                ProjectId = project.Id,
                Title = "Mobile Responsive Design",
                Description = "Ensure all components are mobile-friendly",
                Status = WorkItemStatus.Backlog,
                Priority = TaskPriority.Medium,
                EstimatedHours = 20,
                Tags = new List<string> { "frontend", "mobile" }
            }
        };

        _projects.Add(project);

        // Add second project
        var project2 = new Project
        {
            Name = "Internal Tools Migration",
            Description = "Migrate legacy internal tools to modern cloud-native architecture.",
            Status = ProjectStatus.Planning,
            StartDate = DateTime.UtcNow.AddDays(7),
            DueDate = DateTime.UtcNow.AddDays(90),
            TeamMembers = new List<TeamMember> { bob, dave }
        };

        project2.Tasks = new List<ProjectTask>
        {
            new()
            {
                ProjectId = project2.Id,
                Title = "Legacy System Analysis",
                Description = "Document existing systems and create migration plan",
                Status = WorkItemStatus.Todo,
                Priority = TaskPriority.High,
                EstimatedHours = 40,
                Tags = new List<string> { "analysis", "planning" }
            },
            new()
            {
                ProjectId = project2.Id,
                Title = "Database Schema Migration",
                Description = "Design and implement new database schema",
                Status = WorkItemStatus.Backlog,
                Priority = TaskPriority.Critical,
                EstimatedHours = 60,
                Tags = new List<string> { "database", "migration" }
            }
        };

        _projects.Add(project2);
    }
}

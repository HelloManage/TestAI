# Getting Started

This guide will help you set up and run the TestAI Agile Project Management System.

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Git
- An IDE (Visual Studio 2022, VS Code, or Rider)

## Installation

### 1. Clone the Repository

```bash
git clone https://github.com/HelloManage/TestAI.git
cd TestAI
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Solution

```bash
dotnet build
```

### 4. Run the Application

```bash
cd TestAI.Web
dotnet run
```

The application will start at `https://localhost:5001` or `http://localhost:5000`.

## Project Structure

After cloning, you'll see:

```
TestAI/
├── TestAI.Web/              # Main Blazor application
│   ├── Components/          # UI components
│   │   ├── Pages/          # Routable pages
│   │   └── Layout/         # Layout components
│   ├── Models/             # Domain entities
│   ├── Services/           # Business logic
│   └── Program.cs          # Entry point
├── TestAI.Web.Tests/        # Unit tests
├── docs/                    # Documentation (synced with wiki)
└── TestAI.sln              # Solution file
```

## Running Tests

Execute all unit tests:

```bash
dotnet test
```

Run with verbose output:

```bash
dotnet test --verbosity normal
```

## Sample Data

The application comes with pre-loaded sample data:

### Projects
1. **Customer Portal Redesign** - InProgress status with 4 team members
2. **Internal Tools Migration** - Planning phase

### Team Members
- Sarah Chen (Tech Lead)
- Mike Johnson (Senior Developer)
- Emily Davis (Developer)
- Alex Kim (QA Engineer)

### Sample Tasks
Various tasks across different statuses demonstrating the workflow.

## Development Workflow

### Adding a New Feature

1. Create a branch from `main`
2. Implement changes following existing patterns
3. Add unit tests in `TestAI.Web.Tests`
4. Run tests to ensure nothing breaks
5. Submit a pull request

### Code Organization

- **Models**: Add domain entities in `Models/` folder
- **Services**: Business logic goes in `Services/` with interface + implementation
- **Pages**: New pages in `Components/Pages/` with `.razor` extension
- **Tests**: Mirror the main project structure

## Configuration

### Application Settings

Edit `appsettings.json` for configuration:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### Service Registration

Register new services in `Program.cs`:

```csharp
builder.Services.AddSingleton<IMyService, MyService>();
```

## Troubleshooting

### Port Already in Use

If port 5000/5001 is busy:

```bash
dotnet run --urls "http://localhost:5050"
```

### Build Errors

Clear and rebuild:

```bash
dotnet clean
dotnet restore
dotnet build
```

### Missing SDK

Verify .NET 9.0 is installed:

```bash
dotnet --list-sdks
```

## Next Steps

- Read the [Architecture Overview](Architecture) to understand the system design
- Explore [Domain Models](Domain-Models) for entity documentation
- Review [Services](Services) for business logic details

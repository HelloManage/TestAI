# TestAI - Agile Project Management System

Welcome to the TestAI technical documentation. This wiki provides comprehensive documentation for the Agile Project Management System built with ASP.NET Core Blazor.

## Overview

TestAI is a comprehensive project management solution designed for agile software development teams. It enables teams to:

- **Track Projects**: Monitor project progress with automatic risk detection
- **Manage Tasks**: Handle task workflows from backlog to completion
- **Plan Sprints**: Time-boxed iterations with velocity tracking and burndown charts
- **Allocate Resources**: Team capacity planning and workload management

## Quick Links

- [Architecture Overview](Architecture)
- [Domain Models](Domain-Models)
- [Services Layer](Services)
- [API Reference](API-Reference)
- [Getting Started](Getting-Started)

## Technology Stack

| Component | Technology |
|-----------|------------|
| Framework | ASP.NET Core 9.0 |
| UI | Blazor Server |
| Language | C# 12 |
| Testing | xUnit |

## Project Structure

```
TestAI/
├── TestAI.Web/           # Main application
│   ├── Models/           # Domain entities
│   ├── Services/         # Business logic
│   └── Components/       # Blazor UI components
└── TestAI.Web.Tests/     # Unit tests
```

## Contributing

See the [Contributing Guide](Contributing) for development setup and guidelines.

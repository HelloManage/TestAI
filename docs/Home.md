# TestAI - Agile Project Management System

Welcome to the TestAI documentation wiki.

## Documentation Structure

This wiki is organized into two sections:

### Technical Documentation (synced with repo)
Documentation for developers, synced bidirectionally with the `docs/` folder in the repository.

- [Architecture Overview](Technical-Architecture)
- [Domain Models](Technical-Domain-Models)
- [Services Layer](Technical-Services)
- [API Reference](Technical-API-Reference)
- [Getting Started](Technical-Getting-Started)
- [Contributing Guide](Technical-Contributing)

### Business Documentation (wiki-only)
Documentation for product managers and stakeholders. These pages are **not synced** to the repository.

- Pages prefixed with `Business-` stay in the wiki only
- Use this section for: requirements, user stories, feature specs, meeting notes, etc.

---

## Overview

TestAI is a comprehensive project management solution designed for agile software development teams. It enables teams to:

- **Track Projects**: Monitor project progress with automatic risk detection
- **Manage Tasks**: Handle task workflows from backlog to completion
- **Plan Sprints**: Time-boxed iterations with velocity tracking and burndown charts
- **Allocate Resources**: Team capacity planning and workload management

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

# Deployment Guide

## Overview

This project uses GitHub Actions for CI/CD with deployment to Vercel.

## Pipeline Architecture

### CI Pipeline (`ci.yml`)
Runs on every push and pull request:

1. **Build Job** - Compiles the solution
2. **Test Job** - Runs unit tests with coverage
3. **Code Quality Job** - Checks formatting and warnings
4. **Publish Artifacts Job** - Creates deployment package (main branch only)

### CD Pipeline (`cd.yml`)
Runs after successful CI on main branch:

1. **Validate Job** - Re-runs build and tests for safety
2. **Approval Job** - **Manual approval gate** (requires user confirmation)
3. **Deploy Job** - Deploys to Vercel after approval

## Setup Instructions

### 1. GitHub Repository Secrets

Add the following secrets to your repository (`Settings > Secrets and variables > Actions`):

| Secret Name | Description |
|-------------|-------------|
| `VERCEL_TOKEN` | Your Vercel personal access token |
| `VERCEL_ORG_ID` | Your Vercel organization/team ID |
| `VERCEL_PROJECT_ID` | Your Vercel project ID |

### 2. Create Vercel Project

```bash
# Install Vercel CLI
npm install -g vercel

# Login to Vercel
vercel login

# Link project (run from repository root)
vercel link

# This creates .vercel folder with project.json containing org and project IDs
```

### 3. Configure GitHub Environment

Create a GitHub environment for approval gate:

1. Go to `Settings > Environments`
2. Create environment named `production-approval`
3. Enable "Required reviewers"
4. Add team members who can approve deployments

### 4. Get Vercel IDs

After running `vercel link`, check `.vercel/project.json`:

```json
{
  "orgId": "your-org-id",      // Use as VERCEL_ORG_ID
  "projectId": "your-project-id" // Use as VERCEL_PROJECT_ID
}
```

### 5. Get Vercel Token

1. Go to https://vercel.com/account/tokens
2. Create new token with appropriate scope
3. Copy token to `VERCEL_TOKEN` secret

## Important: Blazor Hosting Considerations

### Current Setup (Blazor Server)

The current project uses Blazor Server with interactive server-side rendering.
**Vercel is a static hosting platform and cannot run Blazor Server apps directly.**

### Options for Vercel Deployment

#### Option A: Convert to Blazor WebAssembly (Recommended for Vercel)

Modify the project to use Blazor WebAssembly for static hosting:

```bash
# Create new WASM project
dotnet new blazorwasm -n TestAI.Web.Wasm

# Move components and services
# Update to use WebAssembly interactivity
```

#### Option B: Use Azure/AWS for Blazor Server

If you need server-side rendering, consider:
- Azure App Service
- AWS Elastic Beanstalk
- Docker containers on any cloud provider

#### Option C: Static Export (Limited)

For demo/documentation purposes, you can publish static content,
but interactive features won't work without WebAssembly.

## Workflow Triggers

### CI Triggers
- Push to `main`, `develop`, or `feature/**` branches
- Pull requests to `main` or `develop`

### CD Triggers
- Automatically after successful CI on `main`
- Manual dispatch via GitHub Actions UI

## Manual Deployment

```bash
# Build locally
dotnet publish TestAI.Web/TestAI.Web.csproj -c Release -o ./publish

# Deploy to Vercel
cd publish
vercel --prod
```

## Monitoring

- Check Actions tab for pipeline status
- Review test reports in workflow summaries
- Vercel dashboard for deployment logs

## Troubleshooting

### Tests Failing
- Check test output in Actions logs
- Run locally: `dotnet test --verbosity detailed`

### Deployment Stuck on Approval
- Go to Actions > CD workflow run
- Click "Review deployments"
- Approve or reject

### Vercel Deployment Failed
- Verify secrets are correct
- Check Vercel project exists
- Review deployment logs in Vercel dashboard

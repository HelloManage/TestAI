<#
.SYNOPSIS
    Bidirectional sync between docs folder and GitHub Wiki

.DESCRIPTION
    This script syncs documentation between the local docs/ folder and the GitHub Wiki.
    It can push local changes to wiki, pull wiki changes to local, or do both.

.PARAMETER Direction
    Sync direction: 'push' (docs to wiki), 'pull' (wiki to docs), or 'both'

.EXAMPLE
    .\sync-wiki.ps1 -Direction push
    .\sync-wiki.ps1 -Direction pull
    .\sync-wiki.ps1 -Direction both
#>

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet('push', 'pull', 'both')]
    [string]$Direction
)

$ErrorActionPreference = "Stop"

# Get the repository root
$RepoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
if (-not $RepoRoot) {
    $RepoRoot = Split-Path -Parent $PSScriptRoot
}

$DocsPath = Join-Path $RepoRoot "docs"
$WikiPath = Join-Path $RepoRoot ".wiki"

# Get remote URL and derive wiki URL
$RemoteUrl = git remote get-url origin
$WikiUrl = $RemoteUrl -replace '\.git$', '.wiki.git'
if ($WikiUrl -notmatch '\.wiki\.git$') {
    $WikiUrl = "$WikiUrl.wiki.git"
}

Write-Host "Repository root: $RepoRoot" -ForegroundColor Cyan
Write-Host "Docs path: $DocsPath" -ForegroundColor Cyan
Write-Host "Wiki URL: $WikiUrl" -ForegroundColor Cyan

function Initialize-Wiki {
    if (-not (Test-Path $WikiPath)) {
        Write-Host "Cloning wiki repository..." -ForegroundColor Yellow
        try {
            git clone $WikiUrl $WikiPath 2>&1
        }
        catch {
            Write-Host "Wiki doesn't exist yet. Creating local wiki folder..." -ForegroundColor Yellow
            New-Item -ItemType Directory -Path $WikiPath -Force | Out-Null
            Push-Location $WikiPath
            git init
            git remote add origin $WikiUrl
            Pop-Location
        }
    }
    else {
        Write-Host "Updating wiki repository..." -ForegroundColor Yellow
        Push-Location $WikiPath
        git pull origin master 2>&1 || git pull origin main 2>&1 || Write-Host "Could not pull (wiki may be empty)"
        Pop-Location
    }
}

function Sync-DocsToWiki {
    Write-Host "`n=== Syncing docs to wiki ===" -ForegroundColor Green

    Initialize-Wiki

    # Copy docs to wiki
    Write-Host "Copying docs to wiki..." -ForegroundColor Yellow
    Get-ChildItem -Path $DocsPath -Filter "*.md" | ForEach-Object {
        Copy-Item $_.FullName -Destination $WikiPath -Force
        Write-Host "  Copied: $($_.Name)" -ForegroundColor Gray
    }

    # Commit and push
    Push-Location $WikiPath
    git add -A
    $changes = git status --porcelain
    if ($changes) {
        git commit -m "Sync from local docs folder"
        Write-Host "Pushing to wiki..." -ForegroundColor Yellow
        git push origin HEAD:master 2>&1 || git push origin HEAD:main 2>&1
        Write-Host "Wiki updated successfully!" -ForegroundColor Green
    }
    else {
        Write-Host "No changes to push to wiki" -ForegroundColor Yellow
    }
    Pop-Location
}

function Sync-WikiToDocs {
    Write-Host "`n=== Syncing wiki to docs ===" -ForegroundColor Green

    Initialize-Wiki

    # Check if wiki has files
    $wikiFiles = Get-ChildItem -Path $WikiPath -Filter "*.md" -ErrorAction SilentlyContinue
    if (-not $wikiFiles) {
        Write-Host "No markdown files in wiki to sync" -ForegroundColor Yellow
        return
    }

    # Copy wiki to docs
    Write-Host "Copying wiki to docs..." -ForegroundColor Yellow
    $wikiFiles | ForEach-Object {
        Copy-Item $_.FullName -Destination $DocsPath -Force
        Write-Host "  Copied: $($_.Name)" -ForegroundColor Gray
    }

    # Check for changes in main repo
    $changes = git status --porcelain docs/
    if ($changes) {
        Write-Host "Changes detected in docs folder. Remember to commit!" -ForegroundColor Yellow
        git status docs/
    }
    else {
        Write-Host "No changes from wiki" -ForegroundColor Yellow
    }
}

# Execute based on direction
switch ($Direction) {
    'push' { Sync-DocsToWiki }
    'pull' { Sync-WikiToDocs }
    'both' {
        Sync-WikiToDocs
        Sync-DocsToWiki
    }
}

Write-Host "`nSync complete!" -ForegroundColor Green

#!/bin/bash
#
# Bidirectional sync between docs folder and GitHub Wiki
#
# Usage:
#   ./sync-wiki.sh push  - Push docs to wiki
#   ./sync-wiki.sh pull  - Pull wiki to docs
#   ./sync-wiki.sh both  - Sync both directions

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
DOCS_PATH="$REPO_ROOT/docs"
WIKI_PATH="$REPO_ROOT/.wiki"

# Get remote URL and derive wiki URL
REMOTE_URL=$(git remote get-url origin)
WIKI_URL="${REMOTE_URL%.git}.wiki.git"

echo "Repository root: $REPO_ROOT"
echo "Docs path: $DOCS_PATH"
echo "Wiki URL: $WIKI_URL"

initialize_wiki() {
    if [ ! -d "$WIKI_PATH" ]; then
        echo "Cloning wiki repository..."
        if ! git clone "$WIKI_URL" "$WIKI_PATH" 2>/dev/null; then
            echo "Wiki doesn't exist yet. Creating local wiki folder..."
            mkdir -p "$WIKI_PATH"
            cd "$WIKI_PATH"
            git init
            git remote add origin "$WIKI_URL"
            cd "$REPO_ROOT"
        fi
    else
        echo "Updating wiki repository..."
        cd "$WIKI_PATH"
        git pull origin master 2>/dev/null || git pull origin main 2>/dev/null || echo "Could not pull (wiki may be empty)"
        cd "$REPO_ROOT"
    fi
}

sync_docs_to_wiki() {
    echo ""
    echo "=== Syncing docs to wiki ==="

    initialize_wiki

    # Copy docs to wiki
    echo "Copying docs to wiki..."
    cp "$DOCS_PATH"/*.md "$WIKI_PATH/" 2>/dev/null || echo "No markdown files to copy"

    # Commit and push
    cd "$WIKI_PATH"
    git add -A
    if ! git diff --cached --quiet; then
        git commit -m "Sync from local docs folder"
        echo "Pushing to wiki..."
        git push origin HEAD:master 2>/dev/null || git push origin HEAD:main 2>/dev/null || echo "Push failed"
        echo "Wiki updated successfully!"
    else
        echo "No changes to push to wiki"
    fi
    cd "$REPO_ROOT"
}

sync_wiki_to_docs() {
    echo ""
    echo "=== Syncing wiki to docs ==="

    initialize_wiki

    # Check if wiki has files
    if [ ! "$(ls -A "$WIKI_PATH"/*.md 2>/dev/null)" ]; then
        echo "No markdown files in wiki to sync"
        return
    fi

    # Copy wiki to docs
    echo "Copying wiki to docs..."
    mkdir -p "$DOCS_PATH"
    cp "$WIKI_PATH"/*.md "$DOCS_PATH/"

    # Check for changes
    if ! git diff --quiet docs/; then
        echo "Changes detected in docs folder. Remember to commit!"
        git status docs/
    else
        echo "No changes from wiki"
    fi
}

# Main
case "$1" in
    push)
        sync_docs_to_wiki
        ;;
    pull)
        sync_wiki_to_docs
        ;;
    both)
        sync_wiki_to_docs
        sync_docs_to_wiki
        ;;
    *)
        echo "Usage: $0 {push|pull|both}"
        echo ""
        echo "  push  - Push local docs to GitHub Wiki"
        echo "  pull  - Pull GitHub Wiki to local docs"
        echo "  both  - Sync both directions"
        exit 1
        ;;
esac

echo ""
echo "Sync complete!"

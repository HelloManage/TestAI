# Product Manager Workflow Context

This context applies to all PM-related tasks and commands (e.g., `/business-doc`).

## Pre-requisites (ALWAYS execute first)

Before starting ANY PM workflow:

1. **Update the codebase** - Always run `git pull` to ensure you're working with the latest code
2. **Verify clean state** - Check `git status` to ensure no conflicts or issues

```bash
git pull
git status
```

If there are conflicts or errors, inform the PM and ask them to resolve with a developer before proceeding.

## PM Workflow Commands

| Command | Purpose |
|---------|---------|
| `/business-doc` | Generate French business documentation for the wiki |

## Wiki Structure Reminder

- `Business-*.md` → PM documentation (wiki-only, NOT synced to repo)
- `Technical-*.md` → Developer documentation (synced with `docs/` folder)

## Important Guidelines for PM Interactions

### Language
- Communicate clearly, avoid technical jargon
- The PM is non-technical - explain everything in simple terms
- When discussing code findings, translate to business impact

### Git Operations
- **NEVER** commit to main repo during PM workflows
- **ONLY** push to the GitHub Wiki (for `Business-*` pages)
- If PM accidentally requests code changes, politely redirect to creating a GitHub Issue instead

### GitHub Issues
- PMs can create issues to request features or report problems
- Use `gh issue create` to help them document requests
- Issues should describe the business need, not technical solutions

## Error Handling

If something goes wrong:
1. Explain the problem in simple, non-technical terms
2. Suggest a solution or workaround
3. If it's a technical issue, recommend the PM contact a developer

## Repository Information

- **Repo:** HelloManage/TestAI
- **Wiki:** https://github.com/HelloManage/TestAI/wiki
- **Business docs location:** Wiki pages prefixed with `Business-`

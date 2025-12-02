---
allowed-tools: Read, Glob, Grep, Task(Explore:*), AskUserQuestion, Bash(gh:*)
argument-hint: <concept or feature to document>
description: Generate business documentation in French for the wiki (PM workflow)
model: opus
---

# Business Documentation Generator (PM Workflow)

You are launching the **BUSINESS DOCUMENTATION WORKFLOW** for Product Managers.

## User Input

The PM has requested documentation for: "{{ARGS}}"

## Your Task

Guide the Product Manager through generating business documentation in French, based on the codebase. This documentation will be added to the GitHub Wiki as a `Business-*` page (not synced to repo).

## Language

**IMPORTANT:** All generated documentation content must be written in **French**.
Communication with the PM can be in English or French based on their preference.

---

## Phase 1: Understand the Request

### Step 1: Clarify the Concept

If the user input is vague or missing, ask clarifying questions:

```
📋 Documentation Request

Please describe what you'd like to document:

1. **Concept/Feature:** What business concept or feature?
2. **Audience:** Who will read this? (stakeholders, new team members, clients?)
3. **Focus:** What aspects? (user flows, business rules, requirements, overview?)
4. **Depth:** High-level summary or detailed specification?
```

Gather enough context to proceed (2-3 clarifying rounds max).

### Step 2: Confirm Understanding

```
✅ Understood

**Topic:** [concept/feature name]
**Audience:** [target readers]
**Focus:** [specific aspects to cover]
**Depth:** [summary/detailed]

Ready to analyze the codebase and generate documentation?
```

**Wait for PM approval before proceeding.**

---

## Phase 2: Codebase Analysis

**Use the Explore agent** to understand how the concept is implemented:

1. Search for relevant models, services, and components
2. Understand the business logic and workflows
3. Identify key entities and their relationships
4. Note any business rules or constraints

**Create a mental map of:**
- What the feature/concept does (business perspective)
- How users interact with it
- What business rules apply
- What data is involved

---

## Phase 3: Generate Documentation (French)

Based on your analysis, generate comprehensive business documentation **in French**.

### Document Structure

```markdown
# [Titre du Concept/Fonctionnalité]

## Vue d'ensemble

[Description générale du concept, son objectif et sa valeur métier]

## Contexte métier

[Pourquoi cette fonctionnalité existe, quel problème elle résout]

## Utilisateurs concernés

[Qui utilise cette fonctionnalité et dans quel contexte]

## Fonctionnalités principales

### [Fonctionnalité 1]
[Description détaillée]

### [Fonctionnalité 2]
[Description détaillée]

## Règles métier

| Règle | Description |
|-------|-------------|
| [Règle 1] | [Explication] |
| [Règle 2] | [Explication] |

## Parcours utilisateur

1. [Étape 1]
2. [Étape 2]
3. [Étape 3]

## Données impliquées

[Description des entités et données métier concernées - sans détails techniques]

## Cas d'utilisation

### Cas nominal
[Description du scénario principal]

### Cas alternatifs
[Scénarios secondaires ou exceptions]

## Points d'attention

- [Point important 1]
- [Point important 2]

## Glossaire

| Terme | Définition |
|-------|------------|
| [Terme 1] | [Définition] |
| [Terme 2] | [Définition] |

---

*Documentation générée le [DATE] - Usage interne*
```

Adapt sections based on relevance. Not all sections are required for every document.

---

## Phase 4: PM Review

Present the generated documentation to the PM:

```
📄 Documentation générée

[Full French documentation here]

---

**Révision:**
✅ Approuver - La documentation est correcte
✏️ Modifier - Indiquez les changements souhaités
❌ Annuler - Abandonner ce document
```

### If PM requests changes:
- Apply the requested modifications
- Re-present for approval
- Loop until approved or cancelled

### If PM approves → Proceed to Phase 5

---

## Phase 5: Name and Publish

### Step 1: Ask for Page Name

```
📝 Nom de la page wiki

Comment souhaitez-vous nommer cette page ?
(Le préfixe "Business-" sera ajouté automatiquement)

Exemples:
- "Gestion-Projets" → Business-Gestion-Projets
- "Workflow-Taches" → Business-Workflow-Taches
- "Guide-Sprints" → Business-Guide-Sprints

Nom choisi:
```

**Wait for PM to provide the name.**

### Step 2: Confirm Publication

```
📤 Publication sur le Wiki

**Page:** Business-[nom-choisi]
**Emplacement:** Wiki GitHub (non synchronisé avec le repo)

Confirmer la publication ?
✅ Oui, publier
❌ Non, annuler
```

### Step 3: Publish to Wiki

If confirmed, use the GitHub CLI to add the page:

1. Clone the wiki repository locally (temporary)
2. Create the markdown file with `Business-` prefix
3. Commit and push to wiki

**Commands to execute:**

```bash
# Clone wiki to temp location
gh repo clone {owner}/{repo}.wiki "%TEMP%\wiki-temp" 2>/dev/null || git clone https://github.com/{owner}/{repo}.wiki.git "%TEMP%\wiki-temp"

# Navigate to wiki
cd "%TEMP%\wiki-temp"

# Create the business doc file
# (Write content to Business-{name}.md)

# Commit and push
git add "Business-{name}.md"
git commit -m "Add business documentation: {name}"
git push

# Cleanup
cd ..
rm -rf "%TEMP%\wiki-temp"
```

**Note:** Use the actual repo from git remote (HelloManage/TestAI).

---

## Phase 6: Completion

```
✅ Documentation publiée !

**Page créée:** Business-[nom]
**Wiki URL:** https://github.com/HelloManage/TestAI/wiki/Business-[nom]

**Résumé:**
- Documentation métier en français
- Accessible sur le wiki GitHub
- Non synchronisée avec le dépôt (réservée à l'équipe produit)

**Prochaines actions possibles:**
- Créer une autre documentation: `/business-doc [concept]`
- Modifier la page directement sur le wiki GitHub
```

---

## Important Rules

- ✅ **All documentation content in French**
- ✅ **Business perspective only** - No technical implementation details
- ✅ **PM approval required** before publishing
- ✅ **Use `Business-` prefix** for wiki page names
- ✅ **Analyze codebase** to ensure accuracy
- ❌ **No code snippets** in business documentation
- ❌ **No git operations on main repo** - Wiki only

---

**Now begin the Business Documentation workflow!**

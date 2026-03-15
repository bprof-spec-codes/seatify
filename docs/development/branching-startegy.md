# Branching Strategy

## Main Principle

Development is based on the **develop** branch.

### Rule

Only **working code** may be merged into `develop`.

This does not require a full feature to be complete, but the delivered part must function independently.

Examples of acceptable merges:

- backend endpoint implemented and working
- frontend component implemented and functional

Unacceptable merges:

- broken builds
- incomplete experimental code
- non-functional partial implementations

---

# Issue Based Development

All work must be performed in branches created from `develop`.

## Branch Source

Branches must be created from:

develop

## Branch Naming Convention

feature/[#issue-number]-[task-name-lowercase-kebab-case]-[fe|be]

Examples:

feature/#42-seat-layout-editor-fe  
feature/#57-booking-session-endpoint-be  
feature/#61-event-occurrence-list-fe  

Possible prefixes:

feature  
fix  
docs
refactor

Examples:

fix/#73-seat-hold-timeout-be  
docs/#23-developement-guide
refactor/#95-event-service-cleanup-fe  

### Naming Rules

- issue number must be included
- lowercase only
- words separated by hyphen
- suffix indicates scope:
  - fe = frontend
  - be = backend

---

# Recommended Development Flow

1. Update develop

git checkout develop  
git pull  

2. Create branch

git checkout -b feature/#42-seat-layout-editor-fe  

3. Implement task

4. Verify locally

5. Merge back to develop

---

# Merge Checklist

Before merging into develop:

- project builds
- relevant part runs locally
- no blocking errors
- issue scope respected
- naming conventions followed
- documentation updated if required
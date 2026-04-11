
# Development Guide

## Overview

This document describes the development workflow, local setup, and engineering conventions used during implementation of the project.

Its purpose is to ensure that every team member works in a consistent way during development, especially during the MVP phase.

---

# Technology Stack

## Backend
- ASP.NET Core Web API
- Entity Framework Core
- InMemory database for the first sprint only

## Frontend
- Angular

---

# Development Environment

## Required Tools

### Backend
- .NET SDK
- Visual Studio / Rider / VS Code
- Git

### Frontend
- Node.js
- npm
- Angular CLI
- Git

---

# Database Strategy

## First Sprint Rule

During the **first sprint only**, the backend uses an **InMemory database**.

Purpose:

- fast startup
- no local SQL Server dependency
- easier onboarding
- fast prototyping

This is a **temporary development decision** and not the final production database.

## Important Note

Because the InMemory database is temporary:

- data is not persistent between restarts
- it is only used for early development
- code must still be written with relational DB migration in mind

---

# Configuration

## Backend

Configuration files:

appsettings.json  
appsettings.Development.json  

## Frontend

Angular environment configuration:

src/environments/environment.ts  
src/environments/environment.development.ts  

---

# Development Rules

## General

- keep changes scoped to the issue
- avoid mixing refactors with features
- keep domain models aligned with architecture docs

## Backend

- maintain clean layering
- keep entity relationships explicit
- write code compatible with later relational DB migration

## Frontend

- reusable components
- centralized API communication
- avoid hardcoded URLs

---

# Summary

Workflow:

- develop must remain stable and runnable
- working code only in develop
- InMemory DB for sprint 1
- backend-only or frontend-only work is acceptable

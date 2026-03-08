---
name: Epic Planning Task
about: Use this template to plan out an Epic before development begins
title: "[PLANNING] Epic: "
labels: Planning, Management
assignees: ''
---

# Epic Planning: [Epic Name]

## Parent Epic
**Parent:** #[Insert Epic Issue Number here]

## Description
This task is for planning the implementation of the parent Epic. The goal is to break down the high-level Epic requirements into manageable Frontend, Backend, and/or Basic Tasks, define the data models, clarify the UI flow, and ensure the team can start working on this Epic without blocking each other.

## Planning Checklist
- [ ] **Scope definition:** Clarify what is *in-scope* and *out-of-scope* for this MVP iteration.
- [ ] **Data Model:** Draft the database entities and relationships needed for this Epic.
- [ ] **API Contracts:** Briefly list the API endpoints that will be needed (e.g., `GET /api/events`, `POST /api/events`).
- [ ] **UI/UX Flow:** Identify the screens and components required (Figma links if available).
- [ ] **Task Breakdown:** Create all necessary child issues in GitHub (Frontend, Backend, or Basic Tasks) and link them to the Parent Epic.

## Definition of Done
- [ ] The Parent Epic description is fully updated and fleshed out.
- [ ] All necessary child issues are created in GitHub and properly linked to the Parent Epic.
- [ ] Architecture / Database schema approved (@batoriandras).
- [ ] Design / UI requirements approved (@fbarnabas55).
- [ ] Planning accepted by PM (@szczukabendeguz).

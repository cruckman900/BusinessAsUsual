# Services Module (scaffold)

This directory contains the initial scaffolding for a new Services module.

Next steps:
- Create projects following the platform pattern: .API, .Web, .Application, .Domain, .Infrastructure, .Contracts, .Tests
- Add project files to the solution (dotnet sln add ...) as described in the root SKILL.md
- Implement persistence in the Infrastructure project and register the module with ModuleRegistry
- Build UI pages for managing services in the Web project and add navigation entries to the Admin

Core entities suggested:
- Service
- ServiceCategory
- ServicePrice
- ServiceProvider
- ServiceOrder / Appointment

Use SKILL.md in the repository root for a step-by-step guide on creating the full module.

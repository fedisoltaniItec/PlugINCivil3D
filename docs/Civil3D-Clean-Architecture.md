# Civil 3D Plugin – Clean Architecture Starter

This repository now includes a **Clean Architecture starter shape** for a Civil 3D plugin.

## Recommended architecture (practical and scalable)

Use four layers with strict dependency direction:

1. **Presentation**
   - Civil 3D command entry points (`[CommandMethod]`), dialogs, command options.
   - Only translates Civil 3D/user input into application requests.

2. **Application**
   - Use cases (orchestrate business flows), DTOs, validation.
   - Depends only on `Domain` and domain-facing ports (interfaces).

3. **Domain**
   - Entities, value objects, domain rules.
   - Pure .NET logic, no Civil 3D/AutoCAD APIs.

4. **Infrastructure**
   - Civil 3D API adapters, repositories, file I/O, logging implementations.
   - Implements interfaces from `Domain`/`Application`.

Dependency rule:

```text
Presentation -> Application -> Domain
Presentation -> Infrastructure (only through composition root wiring)
Infrastructure -> Domain / Application contracts
Domain -> (nothing external)
```

## Why this is the right fit for Civil 3D plugins

- Civil 3D APIs are highly integration-heavy. Isolating them in `Infrastructure` keeps logic testable.
- Command handlers stay thin, which reduces command-side bugs and improves maintainability.
- You can evolve business logic without touching API-specific code.

## Suggested development roadmap

1. Keep command methods very small and delegate to use cases.
2. Start with in-process composition root (no heavy DI framework needed initially).
3. Add unit tests for domain and application flows.
4. Add integration tests later for infrastructure adapters (where feasible).

## Project structure in this repo

```text
PlugINCivil3D/
  Bootstrap/                 # composition root
  Presentation/Commands/     # [CommandMethod] entry points
  Application/UseCases/      # use-case orchestration
  Domain/
    Entities/
    Ports/
  Infrastructure/Civil3D/    # Civil 3D API implementations
```

## Future scaling option

When plugin complexity grows, split into multiple projects:

- `PlugINCivil3D.Domain`
- `PlugINCivil3D.Application`
- `PlugINCivil3D.Infrastructure.Civil3D`
- `PlugINCivil3D.Presentation` (or main plugin assembly)

For now, namespaces + folder boundaries provide a low-friction start.

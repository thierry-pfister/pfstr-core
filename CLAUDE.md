## Vision

This project is a self-hosted personal data and content platform for Thierry Pfister.

It starts as the backend for thierrypfister.dev, but it should be designed as reusable infrastructure for future personal projects.

The system should not be limited to blog posts and portfolio projects. It should provide a clean, extensible foundation for any structured data that Thierry may need across his work, experiments, tools, and public-facing projects.

Examples of data this platform may eventually manage:
- Portfolio projects
- Blog posts
- Personal profile information
- Skills and technologies
- Case studies
- Media/assets metadata
- Social links
- Testimonials
- Project-specific configuration
- Small app datasets
- Webhook/event records
- Cached external API data
- Realtime/project experiment data

The goal is not to build a generic SaaS CMS.

The goal is to build a personal, self-hosted backend that is flexible enough to support future ideas while staying small, understandable, and maintainable.

The platform should be:
- backend-first
- strongly typed
- modular
- self-hosted
- production-minded
- extensible by adding new modules/data models
- simple enough for one person to maintain

Avoid:
- hardcoding the system around only blogs and projects
- pretending this is a public multi-tenant SaaS
- overengineering for unknown future users
- making everything generic too early
- adding abstractions before a real use case exists

---

## Project Stack

This section is specific to this repository.

### Backend

- F# for domain logic and core business rules
- C# with ASP.NET Core for the HTTP API layer
- PostgreSQL as the primary relational database
- Redis for caching, sessions, queues, rate limiting, temporary state, and future realtime/event-driven features

The backend should be built first. The admin UI and portfolio integration depend on a stable backend API.

### Frontend / Admin

- Next.js for the CMS admin panel
- TypeScript strict mode
- React Server Components where useful
- Client components only where interaction requires them

The admin panel should be treated as a consumer of the API, not as the source of business logic.

### Public Consumers

The CMS should expose data for:
- thierrypfister.dev portfolio
- blog pages
- future personal projects
- internal tools or experiments

Consumers should depend on stable API contracts, not database internals.

### Infrastructure

- Docker for local development services
- Docker Compose for reproducible local setup
- PostgreSQL container for local database development
- Redis container for local cache/queue development
- Persistent Docker volumes for local database state
- Environment-based configuration
- Database migrations required for schema changes

### Docker Development Environment

The project should be Dockerized for local development.

Docker should provide:
- PostgreSQL
- Redis
- any required local infrastructure services

The backend and admin app may run either:
- locally on the host machine during development
- inside Docker when useful

The development environment should be reproducible with minimal setup.

Prefer:
- `docker compose` for local services
- environment variables for configuration
- documented startup commands
- predictable local ports
- persistent volumes for database state

Avoid:
- requiring manually installed databases
- hidden local machine dependencies
- undocumented setup steps
- production-only Docker configuration without local development support

### Deployment Direction

- Self-hosted backend
- Self-hosted PostgreSQL
- Self-hosted Redis
- Portfolio remains deployable separately, likely on Vercel
- Admin panel may be self-hosted or deployed separately later

### Non-Goals

This project is not:
- a generic SaaS CMS
- a public multi-tenant platform
- a drag-and-drop website builder
- a WordPress clone
- an overabstracted content framework

The stack should serve Thierry's personal infrastructure needs first.

PostgreSQL is the source of truth.

Redis is an acceleration and coordination layer, not permanent storage.

---

## Architecture Principles

### Clear Boundaries

Separate:
- domain logic
- infrastructure
- persistence
- API contracts
- frontend concerns

Avoid leaking infrastructure concerns into the domain layer.

---

### Stable API Contracts

Consumers should rely on stable contracts.

Avoid breaking API changes unless necessary.

Version APIs when appropriate.

---

### Modular Growth

The system should grow through explicit modules and domains.

Examples:
- Projects
- Blog
- Profile
- Skills
- Assets
- Webhooks
- Experiments

Avoid giant generic content systems without clear structure.

---

## Code Structure Principles

### Single Responsibility First

Files, modules, and functions should have one clear responsibility.

Prefer:
- small focused files
- isolated responsibilities
- explicit boundaries
- composable units

Avoid:
- god files
- mixed responsibilities
- large utility dumping grounds
- deeply nested logic

---

### File Size Guideline

Files should generally remain below 100 lines.

Going slightly above is acceptable when readability benefits from staying together, but large files should usually be split.

Large files are often a signal of:
- mixed responsibilities
- hidden abstractions
- unclear boundaries
- poor separation of concerns

This is a guideline, not a hard rule.

---

### Small Functions

Functions should:
- do one thing
- have clear inputs/outputs
- remain easy to scan quickly
- avoid excessive nesting
- avoid hidden side effects

Prefer extracting logic into named functions over large inline blocks.

---

### Data First Design

Model data and domain concepts before implementation details.

Prefer:
- strong types
- explicit domain models
- value objects
- typed state transitions
- predictable data flow

Avoid:
- designing around frameworks first
- passing raw dictionaries/maps everywhere
- weakly typed primitives as domain concepts

The shape of the data should help explain the system.

---

## Functional Programming Practices

Functional patterns are encouraged when they improve:
- readability
- maintainability
- correctness
- predictability

Examples:
- pipes
- options/maybes
- immutable data
- pure functions
- discriminated unions
- function composition

Do not force functional patterns where they reduce clarity.

Avoid:
- functional abstractions used only for cleverness
- unreadable composition chains
- theoretical purity over practical maintainability

Pragmatism is preferred over ideology.

---

## Development Workflow

### Think Before Coding

Implementation should begin with understanding.

Before writing code:
- understand the problem
- identify the domain boundaries
- think through data flow
- identify responsibilities
- evaluate tradeoffs
- prefer simple solutions first

Avoid:
- coding immediately without design thought
- speculative abstractions
- premature optimization
- framework-driven architecture decisions

---

### Incremental Development

Build systems in small verified steps.

Prefer:
- vertical slices
- small iterations
- testable progress
- stable intermediate states

Avoid:
- massive untested rewrites
- large unstable branches
- unfinished architectural foundations

---

## Testing Workflow

### Test-Driven Development

TDD is the default workflow.

Preferred cycle:
1. write failing test
2. implement minimal solution
3. make test pass
4. refactor safely

Tests should drive implementation, not merely verify it afterward.

---

### Unit Tests First

Domain and business logic should primarily be verified through unit tests.

Unit tests should:
- test behavior
- remain fast
- isolate logic clearly
- avoid unnecessary infrastructure

---

### Integration Tests for Connected Flows

When behavior spans multiple components or function chains, introduce integration tests.

Examples:
- database interactions
- Redis integration
- API request flows
- persistence boundaries
- background jobs
- multi-step workflows

Integration tests should verify systems working together correctly.

---

### Test Readability Matters

Tests should clearly explain:
- what is being tested
- expected behavior
- failure conditions

Avoid:
- overly abstract test helpers
- unreadable test setups
- implementation-coupled tests

---

## Naming Principles

Prefer:
- explicit names
- domain terminology
- intention-revealing identifiers

Avoid:
- abbreviations unless universally understood
- generic names like `Manager`, `Helper`, `Util`
- misleading or overloaded terminology

Names should explain purpose clearly.

---

## Git Workflow

### Branch Strategy

Use feature branches for all work.

Examples:
- `feature/blog-posts`
- `feature/redis-cache`
- `refactor/domain-validation`
- `fix/api-error-handling`

The main branch should remain stable and deployable.

---

### Small Commits

Commits should remain:
- focused
- readable
- logically grouped

Avoid combining:
- refactors
- formatting
- features
- unrelated fixes

in a single commit.

---

### Commit Style

Use commit emojis consistently.

Examples:
- `✨ feat: add blog post aggregate`
- `🐛 fix: resolve redis cache invalidation`
- `♻️ refactor: simplify slug generation`
- `🧪 test: add integration tests for projects api`
- `📝 docs: update architecture notes`
- `🚀 chore: update docker configuration`

Commit messages should explain intent, not only changed files.

---

## AI Assistant Rules

AI assistants working in this repository should:
- follow existing architecture and patterns
- prefer small focused changes
- avoid speculative abstractions
- preserve strong typing
- prioritize readability
- avoid unnecessary dependencies
- maintain separation of concerns
- write tests for behavior changes
- avoid rewriting working systems without reason

AI should ask before:
- introducing major dependencies
- changing architecture significantly
- weakening type safety
- replacing core infrastructure choices

---

## Repository Structure

The project uses a monorepo structure.

Current structure:

```txt
pfstr-core/
├── services/
│   ├── core-api/
│   ├── core-domain/
│   ├── core-application/
│   ├── core-infrastructure/
│   └── core-tests/
├── apps/
│   └── admin/
├── docker/
├── docs/
├── CLAUDE.md
├── docker-compose.yml
└── README.md
```

### Folder Responsibilities

#### `services/core-api`

ASP.NET Core API layer.

Responsibilities:
- HTTP endpoints
- authentication/authorization
- API contracts
- request/response mapping
- OpenAPI/Swagger configuration

Avoid:
- domain business logic
- persistence logic
- infrastructure-heavy behavior

---

#### `services/core-domain`

Pure domain layer written primarily in F#.

Responsibilities:
- entities
- value objects
- domain rules
- validation
- domain events
- business invariants

This layer should remain framework-independent whenever possible.

---

#### `services/core-application`

Application orchestration layer.

Responsibilities:
- use cases
- commands
- queries
- workflows
- coordination between layers

This layer connects API, domain, and infrastructure.

---

#### `services/core-infrastructure`

Infrastructure and external integrations.

Responsibilities:
- PostgreSQL access
- Redis integration
- repositories
- caching
- background jobs
- external APIs
- filesystem or storage integrations

Infrastructure concerns should stay isolated from the domain layer.

---

#### `services/core-tests`

Testing projects.

May contain:
- unit tests
- integration tests
- architecture tests

Tests should mirror real application behavior clearly.

---

#### `apps/admin`

Next.js admin application.

Responsibilities:
- content management UI
- dashboard interfaces
- API consumption
- admin workflows

The admin app should remain a client of the API.

Avoid placing business logic directly in the frontend.

---

#### `docker`

Docker-related configuration.

Examples:
- Dockerfiles
- container setup
- local development services
- environment-specific compose configuration

---

#### `docs`

Project documentation.

Examples:
- architecture notes
- ADRs (Architecture Decision Records)
- diagrams
- API documentation
- deployment notes

Documentation is treated as part of the project, not an afterthought.

---

## Definition of Done

A task is considered complete when:
- code is understandable
- tests pass
- naming is clear
- responsibilities are separated correctly
- no unnecessary complexity was introduced
- behavior is verified
- integration points are tested where needed
- the implementation matches the intended domain behavior
- the codebase remains maintainable

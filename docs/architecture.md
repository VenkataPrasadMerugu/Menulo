# Menulo Architecture

## Folder Structure

```text
Menulo.Web
|-- Areas
|   `-- Owner
|       |-- Controllers
|       `-- Views
|-- Controllers
|-- Infrastructure
|-- ViewModels
|-- Views
`-- wwwroot

Menulo.Application
|-- Abstractions
|-- Common
|-- DTOs
`-- Services

Menulo.Domain
|-- Common
`-- Modules
    |-- Menu
    `-- Restaurants

Menulo.Infrastructure
|-- Extensions
|-- Identity
|-- Persistence
|   |-- Configurations
|   |-- Migrations
|   |-- Repositories
|   `-- Seed
`-- Services
```

## Clean Architecture Mapping

- Presentation layer: `Menulo.Web`
- Application layer: `Menulo.Application`
- Domain layer: `Menulo.Domain`
- Infrastructure layer: `Menulo.Infrastructure`

Dependencies flow inward:

```text
Web -> Application
Web -> Infrastructure
Infrastructure -> Application
Infrastructure -> Domain
Application -> Domain
Domain -> none
```

## Module Isolation

### Restaurant
- Owns restaurant identity, slug, and owner association.
- Exposed through `IRestaurantService` and `IRestaurantRepository`.

### Branding
- Currently part of the restaurant aggregate for MVP.
- Uses dedicated branding update DTOs and keeps UI concerns in separate owner pages.
- Can be moved into a standalone branding service later by splitting `UpdateBrandingAsync`.

### Menu
- Owns menu item CRUD and status transitions.
- Exposed through `IMenuItemService` and `IMenuItemRepository`.
- Public menu rendering depends on a read model from the application layer, not direct EF access from views.

### Media
- Isolated via `IFileStorage`.
- Current implementation is local disk under `wwwroot/uploads`.
- Future migration to S3, Azure Blob, or a media microservice only requires a new infrastructure adapter.

### Ordering
- Not implemented in MVP.
- No ordering concepts have been pushed into restaurant or menu modules, so an ordering service can be added without untangling current write models.

## Microservice Extraction Guidance

### Keep now
- Service interfaces in the application layer.
- DTOs as stable contracts.
- Module-specific repositories instead of a shared generic repository.
- Independent routes and UI flows per module.
- No direct controller access to `DbContext`.

### Extract later
- `Identity Service`: move ASP.NET Identity tables and auth endpoints; keep cookie auth local for monolith or switch to JWT/OpenIddict later.
- `Restaurant Service`: move restaurant profile, slug validation, and owner-to-restaurant ownership checks.
- `Branding Service`: extract branding if branding becomes a more complex domain with themes, templates, and presets.
- `Menu Service`: move menu items and public menu projections.
- `Media Service`: centralize upload validation, storage lifecycle, and CDN URLs.
- `Ordering Service`: build ordering against restaurant and menu contracts without mixing with branding or owner profile concerns.

## Practical Split Strategy

1. Keep application interfaces as the first service contracts.
2. Convert repository-backed application services into API clients one module at a time.
3. Replace direct in-process service registration with HTTP or messaging adapters behind the same interfaces.
4. Move public menu reads first, because they are read-heavy and easy to cache.
5. Move media storage next if file volume grows.
6. Move identity last if single sign-on or multi-tenant auth becomes necessary.

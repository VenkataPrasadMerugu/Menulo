# Menulo

Menulo is a modular monolith restaurant digital menu application built with ASP.NET Core 8, MVC, PostgreSQL, Entity Framework Core, ASP.NET Core Identity, and Bootstrap 5.

## Solution Structure

```text
Menulo.sln
|-- Menulo.Web
|-- Menulo.Application
|-- Menulo.Domain
|-- Menulo.Infrastructure
|-- sql
```

## Layer Responsibilities

- `Menulo.Web`: presentation layer, MVC controllers, view models, Razor views, routing, static assets.
- `Menulo.Application`: application layer, DTOs, service contracts, repository contracts, use-case orchestration.
- `Menulo.Domain`: domain layer, restaurant and menu entities, status enum, business state transitions.
- `Menulo.Infrastructure`: infrastructure layer, EF Core, Identity, repositories, slug generation, local file storage, data seeding.

## Modular Boundaries Kept Now

- `Identity`: owner registration and login are isolated behind `IIdentityService`.
- `Restaurant`: restaurant profile and slug ownership sit behind `IRestaurantService`.
- `Branding`: color and logo updates flow through `UpdateBrandingAsync`.
- `Menu`: menu item CRUD and status changes sit behind `IMenuItemService`.
- `Media`: file persistence is behind `IFileStorage`.
- `Ordering`: intentionally absent from MVP, but there is no coupling from menu/public views into ordering concerns.

## Routing

- `/account/register`
- `/account/login`
- `/owner/dashboard`
- `/owner/account`
- `/owner/branding`
- `/owner/menu-items`
- `/owner/menu-items/create`
- `/owner/menu-items/{id}/edit`
- `/{restaurantSlug}/menu`

## Dynamic Theming

The public menu uses restaurant branding values as CSS variables:

```cshtml
<style>
    :root {
        --menu-primary: @Model.PrimaryColor;
        --menu-secondary: @Model.SecondaryColor;
    }
</style>
```

Those values are then consumed by reusable CSS tokens in [`site.css`](/c:/Users/VenkataMerugu/source/repos/Menulo/Menulo.Web/wwwroot/css/site.css).

## Slug-Based Routing Example

Routing is registered in [`Program.cs`](/c:/Users/VenkataMerugu/source/repos/Menulo/Menulo.Web/Program.cs):

```csharp
app.MapControllerRoute(
    name: "public-menu",
    pattern: "{restaurantSlug}/menu",
    defaults: new { controller = "PublicMenu", action = "Index" });
```

Slug generation and uniqueness enforcement live in [`SlugService.cs`](/c:/Users/VenkataMerugu/source/repos/Menulo/Menulo.Infrastructure/Services/SlugService.cs).

## Image Upload Handling Example

The MVC layer maps `IFormFile` into an application storage request in [`FormFileMapper.cs`](/c:/Users/VenkataMerugu/source/repos/Menulo/Menulo.Web/Infrastructure/FormFileMapper.cs), and infrastructure persists it through [`LocalFileStorage.cs`](/c:/Users/VenkataMerugu/source/repos/Menulo/Menulo.Infrastructure/Services/LocalFileStorage.cs).

Uploads are stored under:

- `wwwroot/uploads/logos`
- `wwwroot/uploads/menu-items`

## Database Scripts

- Schema: [`001_schema.sql`](/c:/Users/VenkataMerugu/source/repos/Menulo/sql/001_schema.sql) using DB-first `tbl_` naming and `tbl_MenuItemImage` for multiple item photos
- Demo seed: [`002_seed_demo.sql`](/c:/Users/VenkataMerugu/source/repos/Menulo/sql/002_seed_demo.sql)
- Sample inserts: [`003_sample_inserts.sql`](/c:/Users/VenkataMerugu/source/repos/Menulo/sql/003_sample_inserts.sql)
- EF-generated migration script: [`generated-migration.sql`](/c:/Users/VenkataMerugu/source/repos/Menulo/sql/generated-migration.sql)

## Local Run

1. Install .NET 8 SDK and PostgreSQL.
2. Create a PostgreSQL database named `menulo`, or update the connection string in [`appsettings.json`](/c:/Users/VenkataMerugu/source/repos/Menulo/Menulo.Web/appsettings.json).
3. Apply the schema with `sql/001_schema.sql` and optionally the seed scripts, or let the app create/migrate on startup.
4. Run:

```powershell
dotnet build Menulo.sln
dotnet run --project .\Menulo.Web\Menulo.Web.csproj
```

5. Open `https://localhost:xxxx/`.

Demo owner credentials seeded at startup and in SQL:

- Email: `owner@menulo.local`
- Password: `Demo@12345`

## Future Microservice Extraction Plan

- `Identity Service`: move `IIdentityService`, ASP.NET Identity tables, and auth endpoints into a dedicated service.
- `Restaurant Service`: extract restaurant profile, slug, and dashboard aggregate reads.
- `Branding Service`: split branding updates and theme retrieval if branding evolves independently.
- `Menu Service`: move menu item CRUD, public menu projection, and status transitions.
- `Media Service`: replace `IFileStorage` local disk implementation with object storage and signed URL workflows.
- `Ordering Service`: add ordering without pushing write logic into restaurant or menu modules.

Keep the following contracts stable now:

- Application service interfaces.
- Repository contracts per module.
- DTOs crossing module boundaries.
- Slug and branding lookup APIs.
- Public route convention `/{restaurantSlug}/menu`.

---
name: entity-framework
description: Use when working on Entity Framework changes in this ASP.NET MVC project, including adding or updating EF models, configuring relationships, updating AppDbContext, preparing migrations, and checking that EF-related changes build correctly.
---

# Entity Framework Skill

Use this skill when changing models, relationships, `AppDbContext`, connection strings, dependency injection, or migrations.

## Workflow

1. Inspect the affected model classes in `Filip_Rados_lab2/Models`.
2. Add or verify `[Key]` on the primary key property.
3. Add or verify `[ForeignKey]` on foreign key properties.
4. Use `virtual` navigation properties for related entities.
5. Use `ICollection<T>` for collection navigation properties.
6. Add or update `DbSet<T>` in `Filip_Rados_lab2/Data/AppDbContext.cs`.
7. Configure unclear or multiple relationships in `OnModelCreating`.
8. Register the context in `Program.cs` with `AddDbContext`.
9. Keep the connection string in `appsettings.json` under `ConnectionStrings`.
10. Run `dotnet build` after changes.

## Model Rules

Use this pattern for primary keys:

```csharp
[Key]
public int Id { get; set; }
```

Use this pattern for 1:N relationships:

```csharp
[ForeignKey(nameof(Author))]
public int AuthorId { get; set; }
public virtual User Author { get; set; }
```

On the parent side:

```csharp
public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
```

Use direct collections for simple N:N relationships:

```csharp
public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
```

Use explicit join entities when the relationship has its own meaning, such as `Like` and `Follow`.

## AppDbContext Rules

Every EF table model should have a `DbSet<T>`:

```csharp
public DbSet<Post> Posts { get; set; }
```

Configure multiple relationships to the same table explicitly. Examples:
- `Message.Sender` and `Message.Receiver`
- `Follow.Follower` and `Follow.Following`
- `Report.Reporter`

Use `DeleteBehavior.Restrict` when cascade delete could create SQL Server multiple cascade path problems.

## Migration Commands

Generate an initial migration:

```powershell
dotnet ef migrations add InitialCreate --project .\Filip_Rados_lab2 --startup-project .\Filip_Rados_lab2 --context AppDbContext
```

Apply migrations to the database:

```powershell
dotnet ef database update --project .\Filip_Rados_lab2 --startup-project .\Filip_Rados_lab2 --context AppDbContext
```

If `dotnet ef` is missing:

```powershell
dotnet tool install --global dotnet-ef
```

## Validation

After EF changes, run:

```powershell
dotnet build .\Filip_Rados_lab2\Filip_Rados_lab2.csproj
```

Build warnings about nullable reference properties are acceptable if the build has `0 Error(s)`.

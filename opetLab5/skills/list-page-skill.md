---
name: list-page
description: Use when creating or updating an ASP.NET MVC list/index page in this project, including controller Index actions, Razor Index views, list/table/card layouts, links to Details pages, and user-facing navigation for collections of models.
---

# List Page Skill

Use this skill when adding or updating an MVC page that displays a collection of records, such as posts, users, tags, comments, reports, or messages.

## Workflow

1. Identify the model being listed.
2. Check whether the controller already exists.
3. Add or update an `Index` action.
4. Fetch the collection from EF through `AppDbContext`.
5. Include related data needed by the view with `Include` or `ThenInclude`.
6. Return a strongly typed model to the view.
7. Create or update `Views/<Controller>/Index.cshtml`.
8. Add links to related pages, especially `Details`.
9. Keep the page user-facing, not admin-like, unless the feature is explicitly administrative.
10. Run `dotnet build`.

## Controller Pattern

Use constructor injection for EF:

```csharp
private readonly AppDbContext _context;

public PostsController(AppDbContext context)
{
    _context = context;
}
```

Use an async list action when possible:

```csharp
public async Task<IActionResult> Index()
{
    var posts = await _context.Posts
        .Include(post => post.Author)
        .Include(post => post.Tags)
        .ToListAsync();

    return View(posts);
}
```

## View Pattern

Use a strongly typed model:

```csharp
@model List<Filip_Rados_lab2.Models.Post>
```

For blog/feed style lists, prefer cards over raw admin tables. Each item should include:
- primary title or name
- short supporting metadata
- link to details
- links to related entities when useful

Example:

```html
<a asp-controller="Posts" asp-action="Details" asp-route-id="@post.Id">
    @post.Title
</a>
```

## Navigation Rules

Use tag helpers instead of hardcoded URLs:

```html
<a asp-controller="Users" asp-action="Details" asp-route-id="@post.AuthorId">
    @post.Author?.Username
</a>
```

Include breadcrumbs when the list belongs to a wider navigation flow.

## Validation

Run:

```powershell
dotnet build .\Filip_Rados_lab2\Filip_Rados_lab2.csproj
```

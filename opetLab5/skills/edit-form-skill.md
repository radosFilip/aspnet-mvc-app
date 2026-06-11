---
name: edit-form
description: Use when creating or updating ASP.NET MVC create/edit forms in this project, including GET and POST controller actions, Razor form views, validation, model binding, EF SaveChanges, select lists, and safe redirect behavior.
---

# Edit Form Skill

Use this skill when adding create or edit functionality for an entity.

## Workflow

1. Identify the entity being created or edited.
2. Add GET action for displaying the form.
3. Add POST action for receiving submitted data.
4. Use model binding with `[Bind]` or a view model when appropriate.
5. Validate with `ModelState.IsValid`.
6. Save changes through `AppDbContext`.
7. Redirect after successful POST.
8. Redisplay the form with validation errors if input is invalid.
9. Use Razor tag helpers for form fields.
10. Run `dotnet build`.

## Create Pattern

GET action:

```csharp
public IActionResult Create()
{
    return View();
}
```

POST action:

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Post post)
{
    if (!ModelState.IsValid)
    {
        return View(post);
    }

    _context.Posts.Add(post);
    await _context.SaveChangesAsync();

    return RedirectToAction(nameof(Index));
}
```

## Edit Pattern

GET action:

```csharp
public async Task<IActionResult> Edit(int id)
{
    var post = await _context.Posts.FindAsync(id);
    if (post == null)
    {
        return NotFound();
    }

    return View(post);
}
```

POST action:

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, Post post)
{
    if (id != post.Id)
    {
        return NotFound();
    }

    if (!ModelState.IsValid)
    {
        return View(post);
    }

    _context.Update(post);
    await _context.SaveChangesAsync();

    return RedirectToAction(nameof(Index));
}
```

## Razor Form Pattern

Use tag helpers:

```html
<form asp-action="Create" method="post">
    <div asp-validation-summary="ModelOnly"></div>

    <label asp-for="Title"></label>
    <input asp-for="Title" />
    <span asp-validation-for="Title"></span>

    <button type="submit">Spremi</button>
</form>
```

Add validation scripts when the view uses validation:

```csharp
@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

## Relationship Fields

When the form needs a foreign key, prepare a select list in the controller:

```csharp
ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Username");
```

Then use:

```html
<select asp-for="AuthorId" asp-items="ViewBag.AuthorId"></select>
```

## Safety Rules

Use `[ValidateAntiForgeryToken]` on POST actions.
Redirect after successful POST to avoid duplicate submissions.
Return `NotFound()` when the requested entity does not exist.
Avoid binding fields the user should not control.

## Validation

Run:

```powershell
dotnet build .\Filip_Rados_lab2\Filip_Rados_lab2.csproj
```

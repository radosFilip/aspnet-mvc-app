# Semantic Routing Model

Ovaj dokument opisuje dostupne URL-ove u aplikaciji LifeHack4Life. Aplikacija koristi standardnu MVC rutu definiranu u `Program.cs`:

```text
{controller=Home}/{action=Index}/{id?}
```

To znaci:
- ako URL nema controller, koristi se `HomeController`
- ako URL nema action, koristi se `Index`
- `id` je opcionalan parametar

## Glavne stranice

| URL | Controller | Akcija | View |
| --- | --- | --- | --- |
| `/` | HomeController | Index | `Views/Home/Index.cshtml` |
| `/Home` | HomeController | Index | `Views/Home/Index.cshtml` |
| `/Home/Index` | HomeController | Index | `Views/Home/Index.cshtml` |
| `/Home/Privacy` | HomeController | Privacy | `Views/Home/Privacy.cshtml` |
| `/Home/Error` | HomeController | Error | `Views/Shared/Error.cshtml` |

## Objave

| URL | Controller | Akcija | View |
| --- | --- | --- | --- |
| `/Posts` | PostsController | Index | `Views/Posts/Index.cshtml` |
| `/Posts/Index` | PostsController | Index | `Views/Posts/Index.cshtml` |
| `/Posts/Index?category=Health` | PostsController | Index | `Views/Posts/Index.cshtml` |
| `/Posts/Index?category=Productivity` | PostsController | Index | `Views/Posts/Index.cshtml` |
| `/Posts/Index?category=Finance` | PostsController | Index | `Views/Posts/Index.cshtml` |
| `/Posts/Index?category=Cooking` | PostsController | Index | `Views/Posts/Index.cshtml` |
| `/Posts/Index?category=Technology` | PostsController | Index | `Views/Posts/Index.cshtml` |
| `/Posts/Index?category=Home` | PostsController | Index | `Views/Posts/Index.cshtml` |
| `/Posts/Index?category=Travel` | PostsController | Index | `Views/Posts/Index.cshtml` |
| `/Posts/Index?category=Other` | PostsController | Index | `Views/Posts/Index.cshtml` |
| `/Posts/Details/{id}` | PostsController | Details | `Views/Posts/Details.cshtml` |

Napomena:
- `category` je query parametar koji filtrira objave po vrijednosti enum tipa `PostCategory`
- `{id}` predstavlja identifikator objave
- ako objava s trazenim `id` ne postoji, akcija vraca `NotFound`

## Korisnici

| URL | Controller | Akcija | View |
| --- | --- | --- | --- |
| `/Users` | UsersController | Index | `Views/Users/Index.cshtml` |
| `/Users/Index` | UsersController | Index | `Views/Users/Index.cshtml` |
| `/Users/Details/{id}` | UsersController | Details | `Views/Users/Details.cshtml` |

Napomena:
- `{id}` predstavlja identifikator korisnika
- ako korisnik s trazenim `id` ne postoji, akcija vraca `NotFound`

## Komentari

| URL | Controller | Akcija | View |
| --- | --- | --- | --- |
| `/Comments` | CommentsController | Index | `Views/Comments/Index.cshtml` |
| `/Comments/Index` | CommentsController | Index | `Views/Comments/Index.cshtml` |
| `/Comments/Details/{id}` | CommentsController | Details | `Views/Comments/Details.cshtml` |

Napomena:
- `{id}` predstavlja identifikator komentara
- ako komentar s trazenim `id` ne postoji, akcija vraca `NotFound`

## Tagovi

| URL | Controller | Akcija | View |
| --- | --- | --- | --- |
| `/Tags` | TagsController | Index | `Views/Tags/Index.cshtml` |
| `/Tags/Index` | TagsController | Index | `Views/Tags/Index.cshtml` |
| `/Tags/Details/{id}` | TagsController | Details | `Views/Tags/Details.cshtml` |

Napomena:
- `{id}` predstavlja identifikator taga
- `TagsController.Details` dodatno preko `ViewBag.Posts` salje postove povezane s odabranim tagom
- ako tag s trazenim `id` ne postoji, akcija vraca `NotFound`

## Poruke

| URL | Controller | Akcija | View |
| --- | --- | --- | --- |
| `/Messages` | MessagesController | Index | `Views/Messages/Index.cshtml` |
| `/Messages/Index` | MessagesController | Index | `Views/Messages/Index.cshtml` |
| `/Messages/Details/{id}` | MessagesController | Details | `Views/Messages/Details.cshtml` |

Napomena:
- `{id}` predstavlja identifikator poruke
- ako poruka s trazenim `id` ne postoji, akcija vraca `NotFound`

## Prijave

| URL | Controller | Akcija | View |
| --- | --- | --- | --- |
| `/Reports` | ReportsController | Index | `Views/Reports/Index.cshtml` |
| `/Reports/Index` | ReportsController | Index | `Views/Reports/Index.cshtml` |
| `/Reports/Details/{id}` | ReportsController | Details | `Views/Reports/Details.cshtml` |

Napomena:
- `{id}` predstavlja identifikator prijave
- ako prijava s trazenim `id` ne postoji, akcija vraca `NotFound`

## Zajednicki layout i view datoteke

Ove datoteke nisu samostalni URL-ovi, ali se koriste kod prikaza stranica:

| Datoteka | Namjena |
| --- | --- |
| `Views/Shared/_Layout.cshtml` | glavni layout aplikacije |
| `Views/_ViewStart.cshtml` | definira zadani layout za viewove |
| `Views/_ViewImports.cshtml` | zajednicki Razor importi i tag helperi |
| `Views/Shared/_ValidationScriptsPartial.cshtml` | partial view za validacijske skripte |

## Sazetak

Aplikacija trenutno koristi konvencionalni MVC routing bez posebnih `[Route]` atributa. Svi dostupni URL-ovi proizlaze iz default rute:

```text
/{controller}/{action}/{id?}
```

Primjeri:
- `/Posts/Details/1` poziva `PostsController.Details(1)` i koristi `Views/Posts/Details.cshtml`
- `/Users/Details/1` poziva `UsersController.Details(1)` i koristi `Views/Users/Details.cshtml`
- `/Tags/Details/2` poziva `TagsController.Details(2)` i koristi `Views/Tags/Details.cshtml`

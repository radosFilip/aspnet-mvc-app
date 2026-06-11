# Chat Log

Ovaj dokument sadrzi log glavnih pitanja korisnika i odgovora asistenta tijekom rada na projektu LifeHack4Life.

## 1. Pocetne izmjene web aplikacije

**Korisnik:**  
Potrebno je par izmjena u projektu. Radi se o web aplikaciji izradenoj u ASP.NET. Na pocetnoj index stranici treba staviti feed sa postovima, jednostavna navigacija i klikom na desnoj strani da se otvori stranica za profil. Trenutni dizajn treba zadrzati.

**Asistent:**  
Pregledao sam strukturu ASP.NET MVC projekta, postojece modele, controllere, viewove i CSS. Pocetna stranica vec je imala pripremljene podatke za postove, ali ih nije prikazivala kao feed. Preuredio sam pocetnu stranicu u blog/feed prikaz, dodao navigaciju i povezao desnu ikonu korisnika s profilom. Build je prosao uspjesno uz postojece nullable warninge.

## 2. Redizajn prema korisnickom blog iskustvu

**Korisnik:**  
Stranica je zamisljena kao blog/aplikacija s objavama. Na pocetnoj stranici treba biti feed, ikona covjeka desno treba otvarati profil s pratiteljima, pracenjima, brojem postova i postovima korisnika. U zutoj navigaciji treba maknuti dropdown. Treba podrzati breadcrumbs i povezani sadrzaj, npr. klik na autora posta otvara profil.

**Asistent:**  
Preuredio sam aplikaciju iz admin-like prikaza u korisnicki blog/feed tok. Uklonio sam dropdown iz zute navigacije, dodao obicne korisnicke linkove, preuredio pocetnu stranicu u feed, uredio profil korisnika i detalje posta tako da koriste breadcrumbs i linkove prema povezanim entitetima. Build je prosao bez gresaka.

## 3. Lab3 PDF i EF zadaci

**Korisnik:**  
Ovo mi je zadatak za iducu vjezbu. Prouci datoteku i objasni korake za: konfigurirati EF, dodati anotacije, podesiti virtual/ICollection, instalirati bazu, podesiti connection string, DbContext i DI.

**Asistent:**  
Procitao sam Lab3.pdf. Objasnio sam da treba instalirati EF pakete, pripremiti modele s `[Key]`, `[ForeignKey]`, `virtual` i `ICollection<T>`, dodati `AppDbContext`, connection string, registrirati `DbContext` u `Program.cs`, a kasnije generirati migraciju i prebaciti repository s mock podataka na EF.

## 4. Objasnjenje EF anotacija i veza

**Korisnik:**  
Objasni mi pravila: `[Key]`, `virtual ICollection<T>`, `[ForeignKey]`, N:N veze.

**Asistent:**  
Objasnio sam da `[Key]` oznacava primarni kljuc tablice, `[ForeignKey]` oznacava strani kljuc, `virtual ICollection<T>` predstavlja kolekcijsku navigaciju za 1:N ili N:N veze, a N:N veze se modeliraju kolekcijama na obje strane ili eksplicitnim spojnim tablicama poput `Like` i `Follow`.

## 5. Spremanje ID-eva naspram objekata

**Korisnik:**  
Ako user ima vise postova, zar nije bolje spremiti samo id-eve postova nego cijele objekte u kolekciju?

**Asistent:**  
Objasnio sam da si u pravu iz perspektive baze: u bazi se ne spremaju cijeli objekti u korisniku, nego `Post` tablica ima strani kljuc `AuthorId`. Kolekcija `user.Posts` u C# modelu je samo navigacijsko svojstvo koje EF koristi za lakse kretanje kroz povezane podatke.

## 6. EF anotacije u modelima

**Korisnik:**  
Postavi potrebne tagove/anotacije u svim klasama unutar Models. Pripazi na relacije, moguce je i da negdje ima N:N.

**Asistent:**  
Dodao sam EF anotacije u modele: `[Key]`, `[ForeignKey]`, `virtual` navigacije i `ICollection<T>` kolekcije. Prepoznao sam `Post-Tag` kao N:N vezu, a `Like` i `Follow` kao eksplicitne spojne tablice. Dodao sam i `CollectionExtensions.cs` kako postojeci mock/seeder kod s `.AddRange()` i dalje moze kompajlirati nakon prelaska na `ICollection<T>`.

## 7. Kompletna EF konfiguracija

**Korisnik:**  
Konfigurirati EF u projektu, dodati anotacije, podesiti veze, bazu/connection string, DbContext i DI. Izgeneriraj i izmijeni sto je potrebno, pa objasni kod. Testiraj samo build.

**Asistent:**  
Dodao sam EF pakete, `AppDbContext`, `DbSet<T>` za glavne modele, connection string za LocalDB, registraciju `AppDbContext` u `Program.cs` i konfiguraciju slozenijih veza u `OnModelCreating`. Build je prosao s `0 Error(s)`, uz nullable warninge u modelima.

## 8. Prebacivanje s mock repositoryja na EF i migracija

**Korisnik:**  
Prebaciti app s mock repository na EF repository i generirati inicijalnu migracijsku skriptu. Instalirao sam MSSQL server, objasni sto je potrebno.

**Asistent:**  
Objasnio sam da treba provjeriti connection string, generirati migraciju naredbom `dotnet ef migrations add InitialCreate`, primijeniti je s `dotnet ef database update`, a zatim controllere prebaciti s `MockRepository` na `AppDbContext`. Objasnio sam i da ce baza nakon migracije biti prazna ako se ne dodaju seed podaci.

## 9. Razlike izmedju SQL server opcija

**Korisnik:**  
Koje su razlike izmedju servera?

**Asistent:**  
Objasnio sam razlike izmedju LocalDB, SQL Server Express, SQL Server Developer/obicnog servera i SQL Servera u Dockeru. Preporucio sam LocalDB za najjednostavniji studentski setup ili SQLEXPRESS ako je instaliran SQL Server Express.

## 10. Connection string

**Korisnik:**  
`Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;` jel to za konekciju?

**Asistent:**  
Objasnio sam da je to connection string, ali da aplikacija ne treba koristiti `Database=master`, nego vlastitu bazu, npr. `LifeHack4LifeDb`. U JSON-u treba pisati `localhost\\SQLEXPRESS` zbog escape znaka.

## 11. LocalDB

**Korisnik:**  
LocalDB je.

**Asistent:**  
Objasnio sam da connection string treba biti:

```json
"Server=(localdb)\\MSSQLLocalDB;Database=LifeHack4LifeDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

Zatim treba generirati migraciju i pokrenuti `database update`.

## 12. Semantic DB model

**Korisnik:**  
Izrada semantickog DB modela: `semantic-model.md`. Sto to znaci?

**Asistent:**  
Objasnio sam da je to markdown dokument koji opisuje modele/tablice, glavna svojstva i veze medju tablicama. Nije SQL skripta niti migracija, nego dokumentacija baze.

**Korisnik:**  
Kreiraj tu datoteku.

**Asistent:**  
Kreirao sam `semantic-model.md` u root direktoriju projekta/rjesenja. U dokumentu su opisani modeli, svojstva, 1:N i N:N veze te EF napomena.

## 13. Sitemap / routing model

**Korisnik:**  
Izrada semantickog modela usmjeravanja: `sitemap.md`. Za svaki dostupni URL treba pisati controller, akcija i view.

**Asistent:**  
Kreirao sam `sitemap.md` s popisom dostupnih URL-ova, controllerima, akcijama i viewovima. Objasnio sam da aplikacija koristi default MVC rutu `{controller=Home}/{action=Index}/{id?}`.

## 14. Skills

**Korisnik:**  
Objasni konfiguraciju skill-ova iz zadatka.

**Asistent:**  
Objasnio sam da su skillovi `SKILL.md` datoteke koje opisuju kako AI agent treba raditi odredjeni tip zadatka, npr. Entity Framework promjene, list stranice ili edit/create forme.

**Korisnik:**  
Mozes li izgenerirati sva tri tipa?

**Asistent:**  
Kreirao sam tri skill dokumenta: Entity Framework skill, list-page skill i edit-form skill.

**Korisnik:**  
Moze li da su datoteke `.md` bez dodatnih direktorija?

**Asistent:**  
Prebacio sam ih u root direktorij kao:
- `entity-framework-skill.md`
- `list-page-skill.md`
- `edit-form-skill.md`

Obrisao sam dodatni `skills` direktorij.

## 15. Provjera izvora podataka

**Korisnik:**  
Provjeri jel aplikacija uzima podatke iz modela ili baze podataka. Trebala bi sada raditi s bazom podataka.

**Asistent:**  
Provjerio sam i nasao da controlleri jos koriste `MockRepository`. Prebacio sam controllere na `AppDbContext` i maknuo preostala citanja iz `MockRepository` u viewovima. Nakon toga `rg "MockRepository|DataSeeder" Controllers Views` vise nije pronasao koristenje u web sloju. Build je prosao s `0 Error(s)`.

## 16. HTTPS i konfiguracija

**Korisnik:**  
Pise unable to connect to server https. Zasto?

**Asistent:**  
Objasnio sam da aplikacija ima HTTP i HTTPS profile. Ako browser otvara HTTPS port bez ispravnog dev certifikata ili ako aplikacija radi samo na HTTP portu, javlja se greska. Predlozio sam otvaranje `http://localhost:5033` ili pokretanje `dotnet dev-certs https --trust`.

**Korisnik:**  
Unhandled exception: Failed to load configuration from `appsettings.json`.

**Asistent:**  
Provjerio sam `appsettings.json` i nasao da postoje dva `ConnectionStrings` bloka. Popravio sam datoteku tako da postoji samo jedan ispravan LocalDB connection string. Build je zatim prosao s `0 Error(s)`.

## 17. Prazna baza i SQL seed

**Korisnik:**  
Pokrenuo sam aplikaciju i nista ne prikazuje. Ne ucitavaju se podaci iz baze podataka.

**Asistent:**  
Objasnio sam da je to ocekivano ako je baza prazna nakon prelaska na EF. Aplikacija sada cita stvarnu bazu, pa treba ubaciti podatke.

**Korisnik:**  
Kreiraj mi SQL skriptu da dodam sve podatke koji se nalaze u `MockRepository.cs`.

**Asistent:**  
Kreirao sam `seed-mock-data.sql` koji ubacuje podatke iz `MockRepository.cs` u tablice:
- Users
- Tags
- Posts
- PostTags
- Comments
- Likes
- Follows
- Messages
- Reports

Skripta ima zastitu: ako tablica `Users` vec ima podatke, seed se preskace.

## 18. SQL Server schema i query

**Korisnik:**  
U bazi podataka svaka tablica ispred svog naziva ima `dbo`.

**Asistent:**  
Objasnio sam da je `dbo` default schema u SQL Serveru, npr. `dbo.Users`. Skripta koja koristi `[Users]` radi jer SQL Server to uobicajeno tumaci kao `[dbo].[Users]` ako je `dbo` default schema.

**Korisnik:**  
Kako da koristim query u SSMS? Trebam li prvo odabrati bazu podataka pa pisati query?

**Asistent:**  
Objasnio sam da u SSMS-u treba odabrati bazu `LifeHack4LifeDb` u dropdownu ili na pocetak queryja staviti:

```sql
USE [LifeHack4LifeDb];
GO
```

Zatim se query pokrece s `Execute` ili `F5`.

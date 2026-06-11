# Semantic DB Model

Ovaj dokument opisuje semanticki model baze podataka za aplikaciju LifeHack4Life. Aplikacija je blog/feed sustav u kojem korisnici objavljuju postove, komentiraju, oznacavaju postove tagovima, lajkaju objave, prate druge korisnike, razmjenjuju poruke i prijavljuju sadrzaj.

## Popis modela / tablica

Modeli koji predstavljaju tablice u bazi:
- User
- Post
- Comment
- Tag
- Like
- Follow
- Message
- Notification
- Report

Pomocni model koji nije glavna tablica aplikacije:
- ErrorViewModel: koristi se za prikaz greske u MVC aplikaciji

## User

Predstavlja korisnika aplikacije.

Glavna svojstva:
- Id: primarni kljuc
- FirstName: ime korisnika
- LastName: prezime korisnika
- Username: korisnicko ime
- Email: email adresa
- DateOfBirth: datum rodjenja

Veze:
- User ima vise Post objekata
- User ima vise Comment objekata
- User ima vise Like objekata
- User moze poslati vise Message objekata
- User moze primiti vise Message objekata
- User ima vise Notification objekata
- User moze napraviti vise Report objekata
- User moze pratiti vise drugih korisnika preko Follow tablice
- User moze imati vise pratitelja preko Follow tablice

## Post

Predstavlja objavu na blog/feed stranici.

Glavna svojstva:
- Id: primarni kljuc
- Title: naslov objave
- Content: sadrzaj objave
- Category: kategorija objave
- CreatedAt: datum i vrijeme stvaranja objave
- AuthorId: strani kljuc prema User tablici

Veze:
- Post pripada jednom Useru kao autoru
- Post ima vise Comment objekata
- Post ima vise Like objekata
- Post ima vise Report objekata
- Post ima vise Tag objekata

## Comment

Predstavlja komentar na objavu.

Glavna svojstva:
- Id: primarni kljuc
- Content: tekst komentara
- CreatedAt: datum i vrijeme stvaranja komentara
- AuthorId: strani kljuc prema User tablici
- PostId: strani kljuc prema Post tablici

Veze:
- Comment pripada jednom Useru kao autoru komentara
- Comment pripada jednom Postu
- User moze imati vise Comment objekata
- Post moze imati vise Comment objekata

## Tag

Predstavlja oznaku ili temu objave.

Glavna svojstva:
- Id: primarni kljuc
- Name: naziv taga

Veze:
- Tag moze biti povezan s vise Post objekata
- Post moze imati vise Tag objekata
- Post i Tag su povezani N:N vezom
- EF koristi spojnu tablicu PostTags za ovu vezu

## Like

Predstavlja lajk korisnika na objavi.

Glavna svojstva:
- Id: primarni kljuc
- UserId: strani kljuc prema User tablici
- PostId: strani kljuc prema Post tablici

Veze:
- Like pripada jednom Useru
- Like pripada jednom Postu
- User moze lajkati vise postova
- Post moze imati vise lajkova
- Like je eksplicitna spojna tablica izmedju User i Post modela

## Follow

Predstavlja pracenje jednog korisnika od strane drugog korisnika.

Glavna svojstva:
- Id: primarni kljuc
- FollowerId: strani kljuc prema User tablici, korisnik koji prati
- FollowingId: strani kljuc prema User tablici, korisnik koji je pracen

Veze:
- User moze pratiti vise drugih korisnika
- User moze imati vise pratitelja
- Follow predstavlja N:N vezu izmedju korisnika i korisnika

## Message

Predstavlja privatnu poruku izmedju dva korisnika.

Glavna svojstva:
- Id: primarni kljuc
- Content: tekst poruke
- SentAt: datum i vrijeme slanja poruke
- IsRead: oznaka je li poruka procitana
- SenderId: strani kljuc prema User tablici
- ReceiverId: strani kljuc prema User tablici

Veze:
- Message ima jednog posiljatelja
- Message ima jednog primatelja
- User moze poslati vise poruka
- User moze primiti vise poruka

## Notification

Predstavlja obavijest za korisnika.

Glavna svojstva:
- Id: primarni kljuc
- Message: tekst obavijesti
- CreatedAt: datum i vrijeme stvaranja obavijesti
- RecipientId: strani kljuc prema User tablici

Veze:
- Notification pripada jednom Useru kao primatelju
- User moze imati vise Notification objekata

## Report

Predstavlja prijavu objave zbog neprimjerenog sadrzaja.

Glavna svojstva:
- Id: primarni kljuc
- Reason: razlog prijave
- Status: status prijave
- CreatedAt: datum i vrijeme stvaranja prijave
- ReporterId: strani kljuc prema User tablici
- PostId: strani kljuc prema Post tablici

Veze:
- Report pripada jednom Useru kao prijavitelju
- Report pripada jednom Postu
- User moze napraviti vise prijava
- Post moze imati vise prijava

## Sazetak veza

1:N veze:
- User 1:N Post
- User 1:N Comment
- Post 1:N Comment
- User 1:N Like
- Post 1:N Like
- User 1:N Message kao Sender
- User 1:N Message kao Receiver
- User 1:N Notification
- User 1:N Report
- Post 1:N Report

N:N veze:
- Post N:N Tag preko spojne tablice PostTags
- User N:N Post preko eksplicitne tablice Like
- User N:N User preko eksplicitne tablice Follow

## Napomena o EF konfiguraciji

Modeli su pripremljeni za Entity Framework:
- svaka glavna tablica ima Id oznacen s [Key]
- strani kljucevi su oznaceni s [ForeignKey]
- navigacijska svojstva su virtual
- kolekcijska svojstva su ICollection<T>
- slozenije veze dodatno su podesene u AppDbContext klasi

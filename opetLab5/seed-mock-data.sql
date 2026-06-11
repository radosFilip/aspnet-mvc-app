USE [LifeHack4LifeDb];
GO

IF EXISTS (SELECT 1 FROM [Users])
BEGIN
    PRINT 'Seed skipped: Users table already contains data.';
    RETURN;
END
GO

BEGIN TRANSACTION;

SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] ([Id], [FirstName], [LastName], [Username], [Email], [DateOfBirth])
VALUES
    (1, N'Ana', N'Horvat', N'ana_h', N'ana@mail.com', '1995-03-12'),
    (2, N'Marko', N'Novak', N'marko_n', N'marko@mail.com', '1990-07-25'),
    (3, N'Petra', N'Kovač', N'petra_k', N'petra@mail.com', '2000-01-08');
SET IDENTITY_INSERT [Users] OFF;

SET IDENTITY_INSERT [Tags] ON;
INSERT INTO [Tags] ([Id], [Name])
VALUES
    (1, N'Produktivnost'),
    (2, N'Zdravlje'),
    (3, N'Financije');
SET IDENTITY_INSERT [Tags] OFF;

SET IDENTITY_INSERT [Posts] ON;
INSERT INTO [Posts] ([Id], [Title], [Content], [Category], [CreatedAt], [AuthorId])
VALUES
    (1, N'5 trikova za bolji san', N'Dobar san ključan je za zdravlje i produktivnost.', 0, '2026-01-10', 1),
    (2, N'Kako uštedjeti 200€ mjesečno', N'Jednostavne financijske navike mogu napraviti veliku razliku.', 2, '2026-01-15', 1),
    (3, N'Pomodoro tehnika', N'Radi 25 minuta, odmori 5 — povećaj fokus i produktivnost.', 1, '2026-02-01', 2),
    (4, N'Jutarnja rutina za energiju', N'Prvih 30 minuta ujutro određuju ton cijelog dana.', 0, '2026-02-10', 2),
    (5, N'Besplatni alati za organizaciju', N'Notion, Trello i TickTick — sve što trebaš, besplatno.', 4, '2026-03-05', 3),
    (6, N'Meal prep za cijeli tjedan', N'Pripremi obroke unaprijed i uštedi i novac i vrijeme.', 3, '2026-03-20', 3),
    (7, N'Kako piti više vode svaki dan', N'Dehidracija utječe na koncentraciju i raspoloženje.', 0, '2026-02-20', 1),
    (8, N'Top 5 aplikacija za meditaciju', N'Headspace, Calm i Insight Timer — briga za mentalno zdravlje.', 4, '2026-03-10', 2),
    (9, N'Kako smanjiti troškove hrane', N'Planiranje obroka i kupovina na akcijama štedi do 30%.', 2, '2026-03-25', 3),
    (10, N'Stretching rutina za uredski posao', N'10 minuta stretchinga dnevno za zdravija leđa i vrat.', 0, '2026-01-25', 1),
    (11, N'Kako organizirati radni stol', N'Uredan stol = uredan um. 5 praktičnih savjeta.', 1, '2026-02-15', 2),
    (12, N'Recepti s 5 sastojaka', N'Brza i zdravi jela koja ne zahtijevaju puno truda.', 3, '2026-03-28', 3),
    (13, N'Kako brže zaspati bez telefona', N'Digitalni detoks pred spavanje poboljšava kvalitetu sna.', 0, '2026-04-01', 2);
SET IDENTITY_INSERT [Posts] OFF;

INSERT INTO [PostTags] ([PostsId], [TagsId])
VALUES
    (1, 2),
    (2, 3),
    (3, 1),
    (4, 1),
    (4, 2),
    (5, 1),
    (6, 2),
    (7, 2),
    (8, 1),
    (9, 3),
    (10, 1),
    (10, 2),
    (11, 1),
    (12, 2);

SET IDENTITY_INSERT [Comments] ON;
INSERT INTO [Comments] ([Id], [Content], [CreatedAt], [AuthorId], [PostId])
VALUES
    (1, N'Odlični savjeti!', '2026-01-11', 2, 1),
    (2, N'Probala sam, stvarno radi!', '2026-01-12', 3, 1),
    (3, N'Super tehnika!', '2026-02-02', 1, 3),
    (4, N'Koristim ovo godinama.', '2026-02-11', 3, 4),
    (5, N'Hvala na preporuci!', '2026-03-06', 1, 5),
    (6, N'Baš mi je pomoglo!', '2026-02-21', 2, 7),
    (7, N'Preporučujem svima!', '2026-03-11', 3, 8),
    (8, N'Odlična aplikacija!', '2026-03-12', 1, 8);
SET IDENTITY_INSERT [Comments] OFF;

SET IDENTITY_INSERT [Likes] ON;
INSERT INTO [Likes] ([Id], [UserId], [PostId])
VALUES
    (1, 2, 1),
    (2, 3, 1),
    (3, 1, 3),
    (4, 3, 4),
    (5, 2, 7),
    (6, 1, 8);
SET IDENTITY_INSERT [Likes] OFF;

SET IDENTITY_INSERT [Follows] ON;
INSERT INTO [Follows] ([Id], [FollowerId], [FollowingId])
VALUES
    (1, 2, 1),
    (2, 3, 1),
    (3, 1, 2);
SET IDENTITY_INSERT [Follows] OFF;

SET IDENTITY_INSERT [Messages] ON;
INSERT INTO [Messages] ([Id], [Content], [SentAt], [IsRead], [SenderId], [ReceiverId])
VALUES
    (1, N'Hej, odličan post!', '2026-01-12', 1, 2, 1),
    (2, N'Hvala!', '2026-01-12', 0, 1, 2),
    (3, N'Možeš li napisati više o financijama?', '2026-02-05', 0, 3, 1);
SET IDENTITY_INSERT [Messages] OFF;

SET IDENTITY_INSERT [Reports] ON;
INSERT INTO [Reports] ([Id], [Reason], [Status], [CreatedAt], [ReporterId], [PostId])
VALUES
    (1, 0, 0, '2026-03-01', 2, 6);
SET IDENTITY_INSERT [Reports] OFF;

COMMIT TRANSACTION;

PRINT 'Seed completed successfully.';
GO

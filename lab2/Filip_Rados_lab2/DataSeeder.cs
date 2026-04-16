using Filip_Rados_lab2.Models;

namespace Filip_Rados_lab2
{
    public class DataSeeder
    {
        public static void Run()
        {
            // ===== KREIRANJE KORISNIKA =====
            var user1 = new User { Id = 1, FirstName = "Ana", LastName = "Horvat", Username = "ana_h", Email = "ana@mail.com", DateOfBirth = new DateTime(1995, 3, 12) };
            var user2 = new User { Id = 2, FirstName = "Marko", LastName = "Novak", Username = "marko_n", Email = "marko@mail.com", DateOfBirth = new DateTime(1990, 7, 25) };
            var user3 = new User { Id = 3, FirstName = "Petra", LastName = "Kovač", Username = "petra_k", Email = "petra@mail.com", DateOfBirth = new DateTime(2000, 1, 8) };

            // ===== KREIRANJE TAGOVA =====
            var tag1 = new Tag { Id = 1, Name = "Produktivnost" };
            var tag2 = new Tag { Id = 2, Name = "Zdravlje" };
            var tag3 = new Tag { Id = 3, Name = "Financije" };

            // ===== KREIRANJE POSTOVA =====
            var post1 = new Post { Id = 1, Title = "5 trikova za bolji san", Content = "...", Category = PostCategory.Health, CreatedAt = new DateTime(2026, 1, 10), AuthorId = 1, Author = user1 };
            var post2 = new Post { Id = 2, Title = "Kako uštedjeti 200€ mjesečno", Content = "...", Category = PostCategory.Finance, CreatedAt = new DateTime(2026, 1, 15), AuthorId = 1, Author = user1 };
            var post3 = new Post { Id = 3, Title = "Pomodoro tehnika", Content = "...", Category = PostCategory.Productivity, CreatedAt = new DateTime(2026, 2, 1), AuthorId = 2, Author = user2 };
            var post4 = new Post { Id = 4, Title = "Jutarnja rutina za energiju", Content = "...", Category = PostCategory.Health, CreatedAt = new DateTime(2026, 2, 10), AuthorId = 2, Author = user2 };
            var post5 = new Post { Id = 5, Title = "Besplatni alati za organizaciju", Content = "...", Category = PostCategory.Technology, CreatedAt = new DateTime(2026, 3, 5), AuthorId = 3, Author = user3 };
            var post6 = new Post { Id = 6, Title = "Meal prep za cijeli tjedan", Content = "...", Category = PostCategory.Cooking, CreatedAt = new DateTime(2026, 3, 20), AuthorId = 3, Author = user3 };
            var post7 = new Post { Id = 7, Title = "Kako piti više vode svaki dan", Content = "...", Category = PostCategory.Health, CreatedAt = new DateTime(2026, 2, 20), AuthorId = 1, Author = user1 };
            var post8 = new Post { Id = 8, Title = "Top 5 aplikacija za meditaciju", Content = "...", Category = PostCategory.Technology, CreatedAt = new DateTime(2026, 3, 10), AuthorId = 2, Author = user2 };
            var post9 = new Post { Id = 9, Title = "Kako smanjiti troškove hrane", Content = "...", Category = PostCategory.Finance, CreatedAt = new DateTime(2026, 3, 25), AuthorId = 3, Author = user3 };
            var post10 = new Post { Id = 10, Title = "Stretching rutina za uredski posao", Content = "...", Category = PostCategory.Health, CreatedAt = new DateTime(2026, 1, 25), AuthorId = 1, Author = user1 };
            var post11 = new Post { Id = 11, Title = "Kako organizirati radni stol", Content = "...", Category = PostCategory.Productivity, CreatedAt = new DateTime(2026, 2, 15), AuthorId = 2, Author = user2 };
            var post12 = new Post { Id = 12, Title = "Recepti s 5 sastojaka", Content = "...", Category = PostCategory.Cooking, CreatedAt = new DateTime(2026, 3, 28), AuthorId = 3, Author = user3 };
            var post13 = new Post { Id = 13, Title = "Kako brže zaspati bez telefona", Content = "...", Category = PostCategory.Health, CreatedAt = new DateTime(2026, 4, 1), AuthorId = 2, Author = user2 };

            // ===== DODAVANJE TAGOVA NA POSTOVE =====
            post1.Tags.AddRange(new[] { tag2 });
            post2.Tags.AddRange(new[] { tag3 });
            post3.Tags.AddRange(new[] { tag1 });
            post4.Tags.AddRange(new[] { tag1, tag2 });
            post5.Tags.AddRange(new[] { tag1 });
            post6.Tags.AddRange(new[] { tag2 });
            post7.Tags.AddRange(new[] { tag2 });
            post8.Tags.AddRange(new[] { tag1 });
            post9.Tags.AddRange(new[] { tag3 });
            post10.Tags.AddRange(new[] { tag1, tag2 });
            post11.Tags.AddRange(new[] { tag1 });
            post12.Tags.AddRange(new[] { tag2 });


            tag1.Posts.AddRange(new[] { post3, post4, post5, post8, post10, post11 });
            tag2.Posts.AddRange(new[] { post1, post4, post6, post7, post10, post12 });
            tag3.Posts.AddRange(new[] { post2, post9 });

            // ===== KREIRANJE KOMENTARA =====
            var comment1 = new Comment { Id = 1, Content = "Odlični savjeti!", CreatedAt = new DateTime(2026, 1, 11), AuthorId = 2, Author = user2, PostId = 1, Post = post1 };
            var comment2 = new Comment { Id = 2, Content = "Probala sam, stvarno radi!", CreatedAt = new DateTime(2026, 1, 12), AuthorId = 3, Author = user3, PostId = 1, Post = post1 };
            var comment3 = new Comment { Id = 3, Content = "Super tehnika!", CreatedAt = new DateTime(2026, 2, 2), AuthorId = 1, Author = user1, PostId = 3, Post = post3 };
            var comment4 = new Comment { Id = 4, Content = "Koristim ovo godinama.", CreatedAt = new DateTime(2026, 2, 11), AuthorId = 3, Author = user3, PostId = 4, Post = post4 };
            var comment5 = new Comment { Id = 5, Content = "Hvala na preporuci!", CreatedAt = new DateTime(2026, 3, 6), AuthorId = 1, Author = user1, PostId = 5, Post = post5 };
            var comment6 = new Comment { Id = 6, Content = "Baš mi je pomoglo!", CreatedAt = new DateTime(2026, 2, 21), AuthorId = 2, Author = user2, PostId = 7, Post = post7 };
            var comment7 = new Comment { Id = 7, Content = "Preporučujem svima!", CreatedAt = new DateTime(2026, 3, 11), AuthorId = 3, Author = user3, PostId = 8, Post = post8 };
            var comment8 = new Comment { Id = 8, Content = "Odlična aplikacija!", CreatedAt = new DateTime(2026, 3, 12), AuthorId = 1, Author = user1, PostId = 8, Post = post8 };

            post1.Comments.AddRange(new[] { comment1, comment2 });
            post3.Comments.Add(comment3);
            post4.Comments.Add(comment4);
            post5.Comments.Add(comment5);
            post7.Comments.Add(comment6);
            post8.Comments.AddRange(new[] { comment7, comment8 });

            // ===== DODAVANJE POSTOVA I KOMENTARA NA KORISNIKE =====
            user1.Posts.AddRange(new[] { post1, post2, post7, post10 });
            user2.Posts.AddRange(new[] { post3, post4, post8, post11, post13 });
            user3.Posts.AddRange(new[] { post5, post6, post9, post12 });

            user1.Comments.AddRange(new[] { comment3, comment5, comment8 });
            user2.Comments.AddRange(new[] { comment1, comment6 });
            user3.Comments.AddRange(new[] { comment2, comment4, comment7 });

            // ===== KREIRANJE LAJKOVA =====
            var like1 = new Like { Id = 1, UserId = 2, User = user2, PostId = 1, Post = post1 };
            var like2 = new Like { Id = 2, UserId = 3, User = user3, PostId = 1, Post = post1 };
            var like3 = new Like { Id = 3, UserId = 1, User = user1, PostId = 3, Post = post3 };
            var like4 = new Like { Id = 4, UserId = 3, User = user3, PostId = 4, Post = post4 };
            var like5 = new Like { Id = 5, UserId = 2, User = user2, PostId = 7, Post = post7 };
            var like6 = new Like { Id = 6, UserId = 1, User = user1, PostId = 8, Post = post8 };

            // ===== KREIRANJE FOLLOWOVA =====
            var follow1 = new Follow { Id = 1, FollowerId = 2, Follower = user2, FollowingId = 1, Following = user1 };
            var follow2 = new Follow { Id = 2, FollowerId = 3, Follower = user3, FollowingId = 1, Following = user1 };
            var follow3 = new Follow { Id = 3, FollowerId = 1, Follower = user1, FollowingId = 2, Following = user2 };

            // ===== KREIRANJE PORUKA =====
            var message1 = new Message { Id = 1, Content = "Hej, odličan post!", SentAt = new DateTime(2026, 1, 12), IsRead = true, SenderId = 2, Sender = user2, ReceiverId = 1, Receiver = user1 };
            var message2 = new Message { Id = 2, Content = "Hvala!", SentAt = new DateTime(2026, 1, 12), IsRead = false, SenderId = 1, Sender = user1, ReceiverId = 2, Receiver = user2 };
            var message3 = new Message { Id = 3, Content = "Možeš li napisati više o financijama?", SentAt = new DateTime(2026, 2, 5), IsRead = false, SenderId = 3, Sender = user3, ReceiverId = 1, Receiver = user1 };

            // ===== KREIRANJE REPORTOVA =====
            var report1 = new Report { Id = 1, Reason = ReportReason.Spam, Status = ReportStatus.Pending, CreatedAt = new DateTime(2026, 3, 1), ReporterId = 2, Reporter = user2, PostId = 6, Post = post6 };

            // ===== LISTA SVIH OBJEKATA =====
            var allUsers = new List<User> { user1, user2, user3 };
            var allPosts = new List<Post> { post1, post2, post3, post4, post5, post6, post7, post8, post9, post10, post11, post12, post13 };
            var allComments = new List<Comment> { comment1, comment2, comment3, comment4, comment5, comment6, comment7, comment8 };
            var allLikes = new List<Like> { like1, like2, like3, like4, like5, like6 };
            var allFollows = new List<Follow> { follow1, follow2, follow3 };
            var allMessages = new List<Message> { message1, message2, message3 };
            var allTags = new List<Tag> { tag1, tag2, tag3 };
            var allReports = new List<Report> { report1 };

            // ===== LINQ UPITI =====

            // 1. Svi postovi kategorije Health, sortirani od najnovijeg
            var healthPosts = allPosts
                .Where(p => p.Category == PostCategory.Health)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            // 2. Korisnik s najviše postova
            var mostActiveUser = allUsers
                .OrderByDescending(u => u.Posts.Count)
                .FirstOrDefault();

            // 3. Top 3 najpopularnija posta po broju komentara
            var popularPosts = allPosts
                .Where(p => p.Comments.Count > 1)
                .OrderByDescending(p => p.Comments.Count)
                .Take(3)
                .ToList();

            // 4. Sve nepročitane poruke za user1
            var unreadMessages = allMessages
                .Where(m => m.ReceiverId == user1.Id && !m.IsRead)
                .ToList();

            // 5. Tagovi s brojem postova, sortirani silazno
            var tagStats = allTags
                .Select(t => new { t.Name, PostCount = t.Posts.Count })
                .OrderByDescending(t => t.PostCount)
                .ToList();

            // 6. Korisnici koje user1 prati (Following)
            var user1Following = allFollows
                .Where(f => f.FollowerId == user1.Id)
                .Select(f => f.Following)
                .ToList();

            // 7. Prosječan broj komentara po postu
            var avgComments = allPosts.Average(p => p.Comments.Count);

            // 8. Postovi s tagom "Zdravlje"
            var healthTagPosts = allPosts
                .Where(p => p.Tags.Any(t => t.Name == "Zdravlje"))
                .ToList();

            // 9. Reportovi s statusom Pending
            var pendingReports = allReports
                .Where(r => r.Status == ReportStatus.Pending)
                .ToList();

            // 10. Broj lajkova po postu
            var likesPerPost = allPosts
                .Select(p => new
                {
                    p.Title,
                    LikeCount = allLikes.Count(l => l.PostId == p.Id)
                })
                .OrderByDescending(p => p.LikeCount)
                .ToList();

            // ===== ISPIS =====
            Console.WriteLine("=== Health postovi ===");
            healthPosts.ForEach(p => Console.WriteLine($"- {p.Title} ({p.CreatedAt:dd.MM.yyyy})"));

            Console.WriteLine($"\n=== Najaktivniji korisnik: {mostActiveUser?.Username} ({mostActiveUser?.Posts.Count} postova) ===");

            Console.WriteLine("\n=== Top 3 popularna posta ===");
            popularPosts.ForEach(p => Console.WriteLine($"- {p.Title}: {p.Comments.Count} komentara"));

            Console.WriteLine($"\n=== Nepročitane poruke za {user1.Username}: {unreadMessages.Count} ===");

            Console.WriteLine("\n=== Tag statistike ===");
            tagStats.ForEach(t => Console.WriteLine($"- {t.Name}: {t.PostCount} postova"));

            Console.WriteLine($"\n=== Prosječan broj komentara po postu: {avgComments:F1} ===");

            Console.WriteLine("\n=== Lajkovi po postu ===");
            likesPerPost.ForEach(p => Console.WriteLine($"- {p.Title}: {p.LikeCount} lajkova"));

            Console.WriteLine("\n=== Korisnici koje user1 prati ===");
            user1Following.ForEach(u => Console.WriteLine($"- {u.Username}"));

            Console.WriteLine("\n=== Postovi s tagom 'Zdravlje' ===");
            healthTagPosts.ForEach(p => Console.WriteLine($"- {p.Title}"));

            Console.WriteLine("\n=== Pending reportovi ===");
            pendingReports.ForEach(r => Console.WriteLine($"- Report ID: {r.Id}, Post: {r.Post.Title}, Reporter: {r.Reporter.Username}, Razlog: {r.Reason}"));
        }
    }
}
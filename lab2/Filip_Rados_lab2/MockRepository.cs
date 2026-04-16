using Filip_Rados_lab2.Models;

namespace Filip_Rados_lab2
{
    public static class MockRepository
    {
        // ===== PODACI =====
        public static readonly List<User> Users;
        public static readonly List<Post> Posts;
        public static readonly List<Comment> Comments;
        public static readonly List<Tag> Tags;
        public static readonly List<Like> Likes;
        public static readonly List<Follow> Follows;
        public static readonly List<Message> Messages;
        public static readonly List<Report> Reports;

        static MockRepository()
        {
            // ── Korisnici ──────────────────────────────────────────────
            var user1 = new User { Id = 1, FirstName = "Ana", LastName = "Horvat", Username = "ana_h", Email = "ana@mail.com", DateOfBirth = new DateTime(1995, 3, 12) };
            var user2 = new User { Id = 2, FirstName = "Marko", LastName = "Novak", Username = "marko_n", Email = "marko@mail.com", DateOfBirth = new DateTime(1990, 7, 25) };
            var user3 = new User { Id = 3, FirstName = "Petra", LastName = "Kovač", Username = "petra_k", Email = "petra@mail.com", DateOfBirth = new DateTime(2000, 1, 8) };

            // ── Tagovi ─────────────────────────────────────────────────
            var tag1 = new Tag { Id = 1, Name = "Produktivnost" };
            var tag2 = new Tag { Id = 2, Name = "Zdravlje" };
            var tag3 = new Tag { Id = 3, Name = "Financije" };

            // ── Postovi ────────────────────────────────────────────────
            var post1 = new Post { Id = 1, Title = "5 trikova za bolji san", Content = "Dobar san ključan je za zdravlje i produktivnost.", Category = PostCategory.Health, CreatedAt = new DateTime(2026, 1, 10), AuthorId = 1, Author = user1 };
            var post2 = new Post { Id = 2, Title = "Kako uštedjeti 200€ mjesečno", Content = "Jednostavne financijske navike mogu napraviti veliku razliku.", Category = PostCategory.Finance, CreatedAt = new DateTime(2026, 1, 15), AuthorId = 1, Author = user1 };
            var post3 = new Post { Id = 3, Title = "Pomodoro tehnika", Content = "Radi 25 minuta, odmori 5 — povećaj fokus i produktivnost.", Category = PostCategory.Productivity, CreatedAt = new DateTime(2026, 2, 1), AuthorId = 2, Author = user2 };
            var post4 = new Post { Id = 4, Title = "Jutarnja rutina za energiju", Content = "Prvih 30 minuta ujutro određuju ton cijelog dana.", Category = PostCategory.Health, CreatedAt = new DateTime(2026, 2, 10), AuthorId = 2, Author = user2 };
            var post5 = new Post { Id = 5, Title = "Besplatni alati za organizaciju", Content = "Notion, Trello i TickTick — sve što trebaš, besplatno.", Category = PostCategory.Technology, CreatedAt = new DateTime(2026, 3, 5), AuthorId = 3, Author = user3 };
            var post6 = new Post { Id = 6, Title = "Meal prep za cijeli tjedan", Content = "Pripremi obroke unaprijed i uštedi i novac i vrijeme.", Category = PostCategory.Cooking, CreatedAt = new DateTime(2026, 3, 20), AuthorId = 3, Author = user3 };
            var post7 = new Post { Id = 7, Title = "Kako piti više vode svaki dan", Content = "Dehidracija utječe na koncentraciju i raspoloženje.", Category = PostCategory.Health, CreatedAt = new DateTime(2026, 2, 20), AuthorId = 1, Author = user1 };
            var post8 = new Post { Id = 8, Title = "Top 5 aplikacija za meditaciju", Content = "Headspace, Calm i Insight Timer — briga za mentalno zdravlje.", Category = PostCategory.Technology, CreatedAt = new DateTime(2026, 3, 10), AuthorId = 2, Author = user2 };
            var post9 = new Post { Id = 9, Title = "Kako smanjiti troškove hrane", Content = "Planiranje obroka i kupovina na akcijama štedi do 30%.", Category = PostCategory.Finance, CreatedAt = new DateTime(2026, 3, 25), AuthorId = 3, Author = user3 };
            var post10 = new Post { Id = 10, Title = "Stretching rutina za uredski posao", Content = "10 minuta stretchinga dnevno za zdravija leđa i vrat.", Category = PostCategory.Health, CreatedAt = new DateTime(2026, 1, 25), AuthorId = 1, Author = user1 };
            var post11 = new Post { Id = 11, Title = "Kako organizirati radni stol", Content = "Uredan stol = uredan um. 5 praktičnih savjeta.", Category = PostCategory.Productivity, CreatedAt = new DateTime(2026, 2, 15), AuthorId = 2, Author = user2 };
            var post12 = new Post { Id = 12, Title = "Recepti s 5 sastojaka", Content = "Brza i zdravi jela koja ne zahtijevaju puno truda.", Category = PostCategory.Cooking, CreatedAt = new DateTime(2026, 3, 28), AuthorId = 3, Author = user3 };
            var post13 = new Post { Id = 13, Title = "Kako brže zaspati bez telefona", Content = "Digitalni detoks pred spavanje poboljšava kvalitetu sna.", Category = PostCategory.Health, CreatedAt = new DateTime(2026, 4, 1), AuthorId = 2, Author = user2 };

            // ── Tagovi na postovima ────────────────────────────────────
            post1.Tags.Add(tag2);
            post2.Tags.Add(tag3);
            post3.Tags.Add(tag1);
            post4.Tags.AddRange(new[] { tag1, tag2 });
            post5.Tags.Add(tag1);
            post6.Tags.Add(tag2);
            post7.Tags.Add(tag2);
            post8.Tags.Add(tag1);
            post9.Tags.Add(tag3);
            post10.Tags.AddRange(new[] { tag1, tag2 });
            post11.Tags.Add(tag1);
            post12.Tags.Add(tag2);

            tag1.Posts.AddRange(new[] { post3, post4, post5, post8, post10, post11 });
            tag2.Posts.AddRange(new[] { post1, post4, post6, post7, post10, post12 });
            tag3.Posts.AddRange(new[] { post2, post9 });

            // ── Komentari ──────────────────────────────────────────────
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

            user1.Posts.AddRange(new[] { post1, post2, post7, post10 });
            user2.Posts.AddRange(new[] { post3, post4, post8, post11, post13 });
            user3.Posts.AddRange(new[] { post5, post6, post9, post12 });

            user1.Comments.AddRange(new[] { comment3, comment5, comment8 });
            user2.Comments.AddRange(new[] { comment1, comment6 });
            user3.Comments.AddRange(new[] { comment2, comment4, comment7 });

            // ── Likeovi ────────────────────────────────────────────────
            var like1 = new Like { Id = 1, UserId = 2, User = user2, PostId = 1, Post = post1 };
            var like2 = new Like { Id = 2, UserId = 3, User = user3, PostId = 1, Post = post1 };
            var like3 = new Like { Id = 3, UserId = 1, User = user1, PostId = 3, Post = post3 };
            var like4 = new Like { Id = 4, UserId = 3, User = user3, PostId = 4, Post = post4 };
            var like5 = new Like { Id = 5, UserId = 2, User = user2, PostId = 7, Post = post7 };
            var like6 = new Like { Id = 6, UserId = 1, User = user1, PostId = 8, Post = post8 };

            // ── Followovi ──────────────────────────────────────────────
            var follow1 = new Follow { Id = 1, FollowerId = 2, Follower = user2, FollowingId = 1, Following = user1 };
            var follow2 = new Follow { Id = 2, FollowerId = 3, Follower = user3, FollowingId = 1, Following = user1 };
            var follow3 = new Follow { Id = 3, FollowerId = 1, Follower = user1, FollowingId = 2, Following = user2 };

            // ── Poruke ─────────────────────────────────────────────────
            var message1 = new Message { Id = 1, Content = "Hej, odličan post!", SentAt = new DateTime(2026, 1, 12), IsRead = true, SenderId = 2, Sender = user2, ReceiverId = 1, Receiver = user1 };
            var message2 = new Message { Id = 2, Content = "Hvala!", SentAt = new DateTime(2026, 1, 12), IsRead = false, SenderId = 1, Sender = user1, ReceiverId = 2, Receiver = user2 };
            var message3 = new Message { Id = 3, Content = "Možeš li napisati više o financijama?", SentAt = new DateTime(2026, 2, 5), IsRead = false, SenderId = 3, Sender = user3, ReceiverId = 1, Receiver = user1 };

            // ── Reportovi ──────────────────────────────────────────────
            var report1 = new Report { Id = 1, Reason = ReportReason.Spam, Status = ReportStatus.Pending, CreatedAt = new DateTime(2026, 3, 1), ReporterId = 2, Reporter = user2, PostId = 6, Post = post6 };

            // ── Finalne liste ──────────────────────────────────────────
            Users = new List<User> { user1, user2, user3 };
            Posts = new List<Post> { post1, post2, post3, post4, post5, post6, post7, post8, post9, post10, post11, post12, post13 };
            Comments = new List<Comment> { comment1, comment2, comment3, comment4, comment5, comment6, comment7, comment8 };
            Tags = new List<Tag> { tag1, tag2, tag3 };
            Likes = new List<Like> { like1, like2, like3, like4, like5, like6 };
            Follows = new List<Follow> { follow1, follow2, follow3 };
            Messages = new List<Message> { message1, message2, message3 };
            Reports = new List<Report> { report1 };
        }

        // ── Users ──────────────────────────────────────────────────────
        public static List<User> GetAllUsers() => Users;
        public static User? GetUserById(int id) => Users.FirstOrDefault(u => u.Id == id);

        // ── Posts ──────────────────────────────────────────────────────
        public static List<Post> GetAllPosts() => Posts;
        public static Post? GetPostById(int id) => Posts.FirstOrDefault(p => p.Id == id);
        public static List<Post> GetPostsByCategory(PostCategory category) => Posts.Where(p => p.Category == category).ToList();
        public static List<Post> GetPostsByUser(int userId) => Posts.Where(p => p.AuthorId == userId).ToList();
        public static List<Post> GetPostsByTag(int tagId) => Posts.Where(p => p.Tags.Any(t => t.Id == tagId)).ToList();

        // ── Comments ───────────────────────────────────────────────────
        public static List<Comment> GetAllComments() => Comments;
        public static Comment? GetCommentById(int id) => Comments.FirstOrDefault(c => c.Id == id);
        public static List<Comment> GetCommentsByPost(int postId) => Comments.Where(c => c.PostId == postId).ToList();
        public static List<Comment> GetCommentsByUser(int userId) => Comments.Where(c => c.AuthorId == userId).ToList();

        // ── Tags ───────────────────────────────────────────────────────
        public static List<Tag> GetAllTags() => Tags;
        public static Tag? GetTagById(int id) => Tags.FirstOrDefault(t => t.Id == id);

        // ── Likes ──────────────────────────────────────────────────────
        public static List<Like> GetAllLikes() => Likes;
        public static List<Like> GetLikesByPost(int postId) => Likes.Where(l => l.PostId == postId).ToList();
        public static int GetLikeCountForPost(int postId) => Likes.Count(l => l.PostId == postId);

        // ── Follows ────────────────────────────────────────────────────
        public static List<Follow> GetAllFollows() => Follows;
        public static List<User> GetFollowing(int userId) => Follows.Where(f => f.FollowerId == userId).Select(f => f.Following).ToList();
        public static List<User> GetFollowers(int userId) => Follows.Where(f => f.FollowingId == userId).Select(f => f.Follower).ToList();

        // ── Messages ───────────────────────────────────────────────────
        public static List<Message> GetAllMessages() => Messages;
        public static Message? GetMessageById(int id) => Messages.FirstOrDefault(m => m.Id == id);
        public static List<Message> GetMessagesByReceiver(int userId) => Messages.Where(m => m.ReceiverId == userId).ToList();
        public static List<Message> GetMessagesBySender(int userId) => Messages.Where(m => m.SenderId == userId).ToList();
        public static List<Message> GetUnreadMessages(int userId) => Messages.Where(m => m.ReceiverId == userId && !m.IsRead).ToList();

        // ── Reports ────────────────────────────────────────────────────
        public static List<Report> GetAllReports() => Reports;
        public static Report? GetReportById(int id) => Reports.FirstOrDefault(r => r.Id == id);
        public static List<Report> GetReportsByStatus(ReportStatus status) => Reports.Where(r => r.Status == status).ToList();
        public static List<Report> GetReportsByPost(int postId) => Reports.Where(r => r.PostId == postId).ToList();
    }
}
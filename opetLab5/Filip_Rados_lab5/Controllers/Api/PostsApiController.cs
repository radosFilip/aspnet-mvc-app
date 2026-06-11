using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Dtos;
using Filip_Rados_lab5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filip_Rados_lab5.Controllers.Api
{
    [Route("api/posts")]
    [ApiController]
    public class PostsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostsApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> Get(string? q, PostCategory? category)
        {
            q = q?.Trim();

            var query = BuildPostQuery();

            if (category.HasValue)
            {
                query = query.Where(post => post.Category == category.Value);
            }

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(post =>
                    EF.Functions.Like(post.Title, $"%{q}%") ||
                    EF.Functions.Like(post.Content, $"%{q}%") ||
                    EF.Functions.Like(post.Author.UserName, $"%{q}%") ||
                    post.Tags.Any(tag => EF.Functions.Like(tag.Name, $"%{q}%")));
            }

            var posts = await query
                .OrderByDescending(post => post.CreatedAt)
                .ToListAsync();

            return Ok(posts.Select(post => post.ToDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PostDto>> Get(int id)
        {
            var post = await BuildPostQuery()
                .FirstOrDefaultAsync(item => item.Id == id);

            if (post == null) return NotFound();

            return Ok(post.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<PostDto>> Post(PostCreateUpdateDto dto)
        {
            await ValidatePost(dto);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var post = new Post();
            await MapToEntity(dto, post);

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            var createdPost = await BuildPostQuery().FirstAsync(item => item.Id == post.Id);
            return CreatedAtAction(nameof(Get), new { id = post.Id }, createdPost.ToDto());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<PostDto>> Put(int id, PostCreateUpdateDto dto)
        {
            var post = await _context.Posts
                .Include(item => item.Tags)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (post == null) return NotFound();

            await ValidatePost(dto);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            await MapToEntity(dto, post);
            await _context.SaveChangesAsync();

            var updatedPost = await BuildPostQuery().FirstAsync(item => item.Id == id);
            return Ok(updatedPost.ToDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private IQueryable<Post> BuildPostQuery()
        {
            return _context.Posts
                .Include(post => post.Author)
                .Include(post => post.Tags)
                .Include(post => post.Comments)
                .Include(post => post.Likes)
                .Include(post => post.Videos);
        }

        private async Task ValidatePost(PostCreateUpdateDto dto)
        {
            if (dto.AuthorId.HasValue &&
                !await _context.Users.AnyAsync(user => user.Id == dto.AuthorId.Value))
            {
                ModelState.AddModelError(nameof(dto.AuthorId), "Odabrani autor ne postoji.");
            }

            var distinctTagIds = dto.TagIds.Distinct().ToList();
            if (distinctTagIds.Count != dto.TagIds.Count)
            {
                ModelState.AddModelError(nameof(dto.TagIds), "Tagovi ne smiju biti duplicirani.");
            }

            if (distinctTagIds.Any())
            {
                var existingTagCount = await _context.Tags.CountAsync(tag => distinctTagIds.Contains(tag.Id));
                if (existingTagCount != distinctTagIds.Count)
                {
                    ModelState.AddModelError(nameof(dto.TagIds), "Jedan ili vise odabranih tagova ne postoji.");
                }
            }
        }

        private async Task MapToEntity(PostCreateUpdateDto dto, Post post)
        {
            post.Title = dto.Title.Trim();
            post.Content = dto.Content.Trim();
            post.Category = dto.Category!.Value;
            post.CreatedAt = dto.CreatedAt ?? DateTime.Now;
            post.AuthorId = dto.AuthorId!.Value;

            post.Tags.Clear();

            if (dto.TagIds.Any())
            {
                var tagIds = dto.TagIds.Distinct().ToList();
                var tags = await _context.Tags
                    .Where(tag => tagIds.Contains(tag.Id))
                    .ToListAsync();

                foreach (var tag in tags)
                {
                    post.Tags.Add(tag);
                }
            }
        }
    }
}

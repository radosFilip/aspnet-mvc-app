using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Dtos;
using Filip_Rados_lab5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filip_Rados_lab5.Controllers.Api
{
    [Route("api/comments")]
    [ApiController]
    public class CommentsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentsApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDto>>> Get(string? q)
        {
            q = q?.Trim();

            var query = BuildCommentQuery();

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(comment =>
                    EF.Functions.Like(comment.Content, $"%{q}%") ||
                    EF.Functions.Like(comment.Author.UserName, $"%{q}%") ||
                    EF.Functions.Like(comment.Post.Title, $"%{q}%"));
            }

            var comments = await query
                .OrderByDescending(comment => comment.CreatedAt)
                .ToListAsync();

            return Ok(comments.Select(comment => comment.ToDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CommentDto>> Get(int id)
        {
            var comment = await BuildCommentQuery()
                .FirstOrDefaultAsync(item => item.Id == id);

            if (comment == null) return NotFound();

            return Ok(comment.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<CommentDto>> Post(CommentCreateUpdateDto dto)
        {
            await ValidateComment(dto);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var comment = new Comment();
            MapToEntity(dto, comment);

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var createdComment = await BuildCommentQuery().FirstAsync(item => item.Id == comment.Id);
            return CreatedAtAction(nameof(Get), new { id = comment.Id }, createdComment.ToDto());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CommentDto>> Put(int id, CommentCreateUpdateDto dto)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return NotFound();

            await ValidateComment(dto);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            MapToEntity(dto, comment);
            await _context.SaveChangesAsync();

            var updatedComment = await BuildCommentQuery().FirstAsync(item => item.Id == id);
            return Ok(updatedComment.ToDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return NotFound();

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private IQueryable<Comment> BuildCommentQuery()
        {
            return _context.Comments
                .Include(comment => comment.Author)
                .Include(comment => comment.Post);
        }

        private async Task ValidateComment(CommentCreateUpdateDto dto)
        {
            if (dto.AuthorId.HasValue &&
                !await _context.Users.AnyAsync(user => user.Id == dto.AuthorId.Value))
            {
                ModelState.AddModelError(nameof(dto.AuthorId), "Odabrani autor ne postoji.");
            }

            if (dto.PostId.HasValue &&
                !await _context.Posts.AnyAsync(post => post.Id == dto.PostId.Value))
            {
                ModelState.AddModelError(nameof(dto.PostId), "Odabrani post ne postoji.");
            }
        }

        private static void MapToEntity(CommentCreateUpdateDto dto, Comment comment)
        {
            comment.Content = dto.Content.Trim();
            comment.CreatedAt = dto.CreatedAt ?? DateTime.Now;
            comment.AuthorId = dto.AuthorId!.Value;
            comment.PostId = dto.PostId!.Value;
        }
    }
}

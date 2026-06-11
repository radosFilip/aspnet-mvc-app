using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Dtos;
using Filip_Rados_lab5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filip_Rados_lab5.Controllers.Api
{
    [Route("api/likes")]
    [ApiController]
    public class LikesApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LikesApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> Get(string? q)
        {
            q = q?.Trim();

            var query = BuildLikeQuery();

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(like =>
                    EF.Functions.Like(like.User.UserName, $"%{q}%") ||
                    EF.Functions.Like(like.Post.Title, $"%{q}%"));
            }

            var likes = await query
                .OrderBy(like => like.User.UserName)
                .ThenBy(like => like.Post.Title)
                .ToListAsync();

            return Ok(likes.Select(like => like.ToDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LikeDto>> Get(int id)
        {
            var like = await BuildLikeQuery()
                .FirstOrDefaultAsync(item => item.Id == id);

            if (like == null) return NotFound();

            return Ok(like.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<LikeDto>> Post(LikeCreateUpdateDto dto)
        {
            await ValidateLike(dto);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var like = new Like
            {
                UserId = dto.UserId!.Value,
                PostId = dto.PostId!.Value
            };

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            var createdLike = await BuildLikeQuery().FirstAsync(item => item.Id == like.Id);
            return CreatedAtAction(nameof(Get), new { id = like.Id }, createdLike.ToDto());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<LikeDto>> Put(int id, LikeCreateUpdateDto dto)
        {
            var like = await _context.Likes.FindAsync(id);
            if (like == null) return NotFound();

            await ValidateLike(dto, id);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            like.UserId = dto.UserId!.Value;
            like.PostId = dto.PostId!.Value;
            await _context.SaveChangesAsync();

            var updatedLike = await BuildLikeQuery().FirstAsync(item => item.Id == id);
            return Ok(updatedLike.ToDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var like = await _context.Likes.FindAsync(id);
            if (like == null) return NotFound();

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private IQueryable<Like> BuildLikeQuery()
        {
            return _context.Likes
                .Include(like => like.User)
                .Include(like => like.Post);
        }

        private async Task ValidateLike(LikeCreateUpdateDto dto, int? likeId = null)
        {
            if (dto.UserId.HasValue &&
                !await _context.Users.AnyAsync(user => user.Id == dto.UserId.Value))
            {
                ModelState.AddModelError(nameof(dto.UserId), "Odabrani korisnik ne postoji.");
            }

            if (dto.PostId.HasValue &&
                !await _context.Posts.AnyAsync(post => post.Id == dto.PostId.Value))
            {
                ModelState.AddModelError(nameof(dto.PostId), "Odabrani post ne postoji.");
            }

            if (dto.UserId.HasValue && dto.PostId.HasValue &&
                await _context.Likes.AnyAsync(like =>
                    like.Id != likeId &&
                    like.UserId == dto.UserId.Value &&
                    like.PostId == dto.PostId.Value))
            {
                ModelState.AddModelError(nameof(dto.PostId), "Taj korisnik je vec lajkao odabrani post.");
            }
        }
    }
}

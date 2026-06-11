using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Dtos;
using Filip_Rados_lab5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filip_Rados_lab5.Controllers.Api
{
    [Route("api/follows")]
    [ApiController]
    public class FollowsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FollowsApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FollowDto>>> Get(string? q)
        {
            q = q?.Trim();

            var query = BuildFollowQuery();

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(follow =>
                    EF.Functions.Like(follow.Follower.UserName, $"%{q}%") ||
                    EF.Functions.Like(follow.Following.UserName, $"%{q}%"));
            }

            var follows = await query
                .OrderBy(follow => follow.Follower.UserName)
                .ThenBy(follow => follow.Following.UserName)
                .ToListAsync();

            return Ok(follows.Select(follow => follow.ToDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<FollowDto>> Get(int id)
        {
            var follow = await BuildFollowQuery()
                .FirstOrDefaultAsync(item => item.Id == id);

            if (follow == null) return NotFound();

            return Ok(follow.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<FollowDto>> Post(FollowCreateUpdateDto dto)
        {
            await ValidateFollow(dto);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var follow = new Follow
            {
                FollowerId = dto.FollowerId!.Value,
                FollowingId = dto.FollowingId!.Value
            };

            _context.Follows.Add(follow);
            await _context.SaveChangesAsync();

            var createdFollow = await BuildFollowQuery().FirstAsync(item => item.Id == follow.Id);
            return CreatedAtAction(nameof(Get), new { id = follow.Id }, createdFollow.ToDto());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<FollowDto>> Put(int id, FollowCreateUpdateDto dto)
        {
            var follow = await _context.Follows.FindAsync(id);
            if (follow == null) return NotFound();

            await ValidateFollow(dto, id);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            follow.FollowerId = dto.FollowerId!.Value;
            follow.FollowingId = dto.FollowingId!.Value;
            await _context.SaveChangesAsync();

            var updatedFollow = await BuildFollowQuery().FirstAsync(item => item.Id == id);
            return Ok(updatedFollow.ToDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var follow = await _context.Follows.FindAsync(id);
            if (follow == null) return NotFound();

            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private IQueryable<Follow> BuildFollowQuery()
        {
            return _context.Follows
                .Include(follow => follow.Follower)
                .Include(follow => follow.Following);
        }

        private async Task ValidateFollow(FollowCreateUpdateDto dto, int? followId = null)
        {
            if (dto.FollowerId.HasValue &&
                !await _context.Users.AnyAsync(user => user.Id == dto.FollowerId.Value))
            {
                ModelState.AddModelError(nameof(dto.FollowerId), "Korisnik koji prati ne postoji.");
            }

            if (dto.FollowingId.HasValue &&
                !await _context.Users.AnyAsync(user => user.Id == dto.FollowingId.Value))
            {
                ModelState.AddModelError(nameof(dto.FollowingId), "Korisnik koji se prati ne postoji.");
            }

            if (dto.FollowerId.HasValue &&
                dto.FollowingId.HasValue &&
                dto.FollowerId.Value == dto.FollowingId.Value)
            {
                ModelState.AddModelError(nameof(dto.FollowingId), "Korisnik ne moze pratiti sam sebe.");
            }

            if (dto.FollowerId.HasValue && dto.FollowingId.HasValue &&
                await _context.Follows.AnyAsync(follow =>
                    follow.Id != followId &&
                    follow.FollowerId == dto.FollowerId.Value &&
                    follow.FollowingId == dto.FollowingId.Value))
            {
                ModelState.AddModelError(nameof(dto.FollowingId), "Takvo pracenje vec postoji.");
            }
        }
    }
}

using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Dtos;
using Filip_Rados_lab5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filip_Rados_lab5.Controllers.Api
{
    [Route("api/users")]
    [ApiController]
    public class UsersApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> Get(string? q)
        {
            q = q?.Trim();

            var query = _context.Users
                .Include(user => user.Posts)
                .Include(user => user.Followers)
                .Include(user => user.Following)
                .Include(user => user.ProfileImages)
                .AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(user =>
                    EF.Functions.Like(user.UserName, $"%{q}%") ||
                    EF.Functions.Like(user.FirstName, $"%{q}%") ||
                    EF.Functions.Like(user.LastName, $"%{q}%") ||
                    EF.Functions.Like(user.Email, $"%{q}%"));
            }

            var users = await query
                .OrderBy(user => user.UserName)
                .ToListAsync();

            return Ok(users.Select(user => user.ToDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> Get(int id)
        {
            var user = await _context.Users
                .Include(item => item.Posts)
                .Include(item => item.Followers)
                .Include(item => item.Following)
                .Include(item => item.ProfileImages)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (user == null) return NotFound();

            return Ok(user.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Post(UserCreateUpdateDto dto)
        {
            await ValidateUniqueUser(dto);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var user = new User();
            MapToEntity(dto, user);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = user.Id }, user.ToDto());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserDto>> Put(int id, UserCreateUpdateDto dto)
        {
            var user = await _context.Users
                .Include(item => item.Posts)
                .Include(item => item.Followers)
                .Include(item => item.Following)
                .Include(item => item.ProfileImages)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (user == null) return NotFound();

            await ValidateUniqueUser(dto, id);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            MapToEntity(dto, user);
            await _context.SaveChangesAsync();

            return Ok(user.ToDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var hasRelatedData =
                await _context.Posts.AnyAsync(post => post.AuthorId == id) ||
                await _context.Comments.AnyAsync(comment => comment.AuthorId == id) ||
                await _context.Messages.AnyAsync(message => message.SenderId == id || message.ReceiverId == id) ||
                await _context.Reports.AnyAsync(report => report.ReporterId == id) ||
                await _context.Likes.AnyAsync(like => like.UserId == id) ||
                await _context.Follows.AnyAsync(follow => follow.FollowerId == id || follow.FollowingId == id) ||
                await _context.Notifications.AnyAsync(notification => notification.RecipientId == id) ||
                await _context.UserProfileImages.AnyAsync(image => image.UserId == id);

            if (hasRelatedData)
            {
                return Conflict(new { message = "Korisnik ima povezane zapise pa ga nije moguce obrisati bez gubitka podataka." });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task ValidateUniqueUser(UserCreateUpdateDto dto, int? userId = null)
        {
            if (await _context.Users.AnyAsync(user => user.Id != userId && user.UserName == dto.Username))
            {
                ModelState.AddModelError(nameof(dto.Username), "Korisnicko ime je vec zauzeto.");
            }

            if (await _context.Users.AnyAsync(user => user.Id != userId && user.Email == dto.Email))
            {
                ModelState.AddModelError(nameof(dto.Email), "Email je vec zauzet.");
            }
        }

        private static void MapToEntity(UserCreateUpdateDto dto, User user)
        {
            user.FirstName = dto.FirstName.Trim();
            user.LastName = dto.LastName.Trim();
            user.UserName = dto.Username.Trim();
            user.Email = dto.Email.Trim();
            user.DateOfBirth = dto.DateOfBirth!.Value;
        }
    }
}

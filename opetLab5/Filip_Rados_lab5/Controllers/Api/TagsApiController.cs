using Filip_Rados_lab5.Data;
using Filip_Rados_lab5.Dtos;
using Filip_Rados_lab5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filip_Rados_lab5.Controllers.Api
{
    [Route("api/tags")]
    [ApiController]
    public class TagsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TagsApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> Get(string? q)
        {
            q = q?.Trim();

            var query = _context.Tags
                .Include(tag => tag.Posts)
                .AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(tag => EF.Functions.Like(tag.Name, $"%{q}%"));
            }

            var tags = await query
                .OrderBy(tag => tag.Name)
                .ToListAsync();

            return Ok(tags.Select(tag => tag.ToDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TagDto>> Get(int id)
        {
            var tag = await _context.Tags
                .Include(item => item.Posts)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (tag == null) return NotFound();

            return Ok(tag.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<TagDto>> Post(TagCreateUpdateDto dto)
        {
            await ValidateUniqueTag(dto);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var tag = new Tag { Name = dto.Name.Trim() };
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = tag.Id }, tag.ToDto());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TagDto>> Put(int id, TagCreateUpdateDto dto)
        {
            var tag = await _context.Tags
                .Include(item => item.Posts)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (tag == null) return NotFound();

            await ValidateUniqueTag(dto, id);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            tag.Name = dto.Name.Trim();
            await _context.SaveChangesAsync();

            return Ok(tag.ToDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tag = await _context.Tags
                .Include(item => item.Posts)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (tag == null) return NotFound();

            if (tag.Posts.Any())
            {
                return Conflict(new { message = "Tag je povezan s objavama pa ga nije moguce obrisati." });
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task ValidateUniqueTag(TagCreateUpdateDto dto, int? tagId = null)
        {
            if (await _context.Tags.AnyAsync(tag => tag.Id != tagId && tag.Name == dto.Name))
            {
                ModelState.AddModelError(nameof(dto.Name), "Tag s tim nazivom vec postoji.");
            }
        }
    }
}

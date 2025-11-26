using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using WebDiaryAPI.Data;
using WebDiaryAPI.Models;

namespace WebDiaryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiaryEntriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public DiaryEntriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiaryEntry>>> GetDiaryEntries()
        {
            return await _context.DiaryEntries.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DiaryEntry>> GetDiaryEntry(int id)
        {
            // Database 
            var diaryEntry = await _context.DiaryEntries.FindAsync(id);

            if (diaryEntry == null)
            {
                return NotFound();
            }

            return diaryEntry;
        }

        [HttpPost]
        public async Task<ActionResult<DiaryEntry>> PostDiaryEntry(DiaryEntry diaryEntry)
        {
            diaryEntry.Id = 0; // Ensure the ID is set to 0 for new entries (automaticaly sets the good id)

            _context.DiaryEntries.Add(diaryEntry);
            await _context.SaveChangesAsync();

            var resourceUrl = Url.Action(nameof(GetDiaryEntry), new { id = diaryEntry.Id });

            return Created(resourceUrl, diaryEntry);
        }

        // put something on e.g. /api/DiaryEntries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiaryEntry(int id, [FromBody] DiaryEntry diaryEntry)
        {
            if (id != diaryEntry.Id)
            {
                // returns 400 response when id do not match
                return BadRequest();
            }

            // mark the entity as modified
            _context.Entry(diaryEntry).State = EntityState.Modified; // tells EF to update this entity

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiaryEntryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // returns 204 response
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiaryEntry(int id)
        {
           var diaryEntry = await _context.DiaryEntries.FindAsync(id);
            if (diaryEntry == null)
            {
                return NotFound();
            }
            _context.DiaryEntries.Remove(diaryEntry);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool DiaryEntryExists(int id)
        {
            return _context.DiaryEntries.Any(e => e.Id == id);
        }
    }
}

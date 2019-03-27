using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trySample.Entities;
using trySample.Helpers;

namespace trySample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly MyDbContext _context;

        public SampleController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Sample
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sample>>> GetSamples()
        {
            return await _context.Samples.ToListAsync();
        }

        // GET: api/Sample/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sample>> GetSample(Guid id)
        {
            var sample = await _context.Samples.FindAsync(id);

            if (sample == null)
            {
                return NotFound();
            }

            return sample;
        }

        // PUT: api/Sample/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSample(Guid id, [FromForm]Sample sample)
        {
            if (id != sample.Id)
            {
                return BadRequest();
            }

            _context.Entry(sample).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SampleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Sample
        [HttpPost]
        public async Task<ActionResult<Sample>> PostSample([FromForm]Sample sample)
        {
            _context.Samples.Add(sample);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSample", new { id = sample.Id }, sample);
        }

        // DELETE: api/Sample/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Sample>> DeleteSample(Guid id)
        {
            var sample = await _context.Samples.FindAsync(id);
            if (sample == null)
            {
                return NotFound();
            }

            _context.Samples.Remove(sample);
            await _context.SaveChangesAsync();

            return sample;
        }

        private bool SampleExists(Guid id)
        {
            return _context.Samples.Any(e => e.Id == id);
        }
    }
}

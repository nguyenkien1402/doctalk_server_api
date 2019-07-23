using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocTalk_Dev_API.Models;
using Microsoft.AspNetCore.Authorization;

namespace DocTalk_Dev_API.Controllers
{
    [Route("api/professionals")]
    [ApiController]
    public class ProfessionalsController : ControllerBase
    {
        private readonly DocTalkDevContext _context;

        public ProfessionalsController(DocTalkDevContext context)
        {
            _context = context;
        }

        // GET: api/Professionals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Professional>>> GetProfessional()
        {
            return await _context.Professional.ToListAsync();
        }

        // GET: api/Professionals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Professional>> GetProfessional(int id)
        {
            var professional = await _context.Professional.FindAsync(id);

            if (professional == null)
            {
                return NotFound();
            }

            return professional;
        }

        // PUT: api/Professionals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProfessional(int id, Professional professional)
        {
            if (id != professional.Id)
            {
                return BadRequest();
            }

            _context.Entry(professional).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfessionalExists(id))
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

        // POST: api/Professionals
        [HttpPost]
        public async Task<ActionResult<Professional>> PostProfessional(Professional professional)
        {
            _context.Professional.Add(professional);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProfessional", new { id = professional.Id }, professional);
        }

        // DELETE: api/Professionals/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Professional>> DeleteProfessional(int id)
        {
            var professional = await _context.Professional.FindAsync(id);
            if (professional == null)
            {
                return NotFound();
            }

            _context.Professional.Remove(professional);
            await _context.SaveChangesAsync();

            return professional;
        }

        private bool ProfessionalExists(int id)
        {
            return _context.Professional.Any(e => e.Id == id);
        }
    }
}

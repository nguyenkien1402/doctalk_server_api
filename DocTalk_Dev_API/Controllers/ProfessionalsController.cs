using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocTalk_Dev_API.Models;
using Microsoft.AspNetCore.Authorization;
using DocTalk_Dev_API.Views;

namespace DocTalk_Dev_API.Controllers
{
    [Route("api/professionals/")]
    [ApiController]
    [Authorize]
    public class ProfessionalsController : Controller
    {
        private readonly DocTalkDevContext _context;

        public ProfessionalsController(DocTalkDevContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("doctor/addpro")]
        public ActionResult AddProfessional([FromBody]DoctorProfessionalView model)
        {
            try
            {
                foreach (int professId in model.ProfessionalId)
                {
                    if (_context.DoctorProfessional.Where(dp => dp.DoctorId == model.DoctorId && dp.ProfessionalId == professId).Count() == 0)
                    {
                        DoctorProfessional doctorProfessional = new DoctorProfessional { DoctorId = model.DoctorId, ProfessionalId = professId };
                        _context.DoctorProfessional.Add(doctorProfessional);
                    }
                    else
                    {
                        Console.WriteLine("The doctor already have this professional - exist: " + professId);
                    }
                }
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                return Conflict();
            }

            var result = new
            {
                Meta = new { Status = "OK", Message = "Add Professioanl Successfully" }
            };
            return Ok(result);
        }

        // GET: api/Professionals
        [HttpGet]
        [AllowAnonymous]
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

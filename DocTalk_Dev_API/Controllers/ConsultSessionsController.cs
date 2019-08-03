using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocTalk_Dev_API.Models;

namespace DocTalk_Dev_API.Controllers
{
    [Route("api/consultsession/")]
    [ApiController]
    public class ConsultSessionsController : Controller
    {
        private readonly DocTalkDevContext _context;

        public ConsultSessionsController(DocTalkDevContext context)
        {
            _context = context;
        }

        // GET: api/ConsultSessions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConsultSession>>> GetConsultSession()
        {
            return await _context.ConsultSession.ToListAsync();
        }

        // GET: api/ConsultSessions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ConsultSession>> GetConsultSession(int id)
        {
            var consultSession = await _context.ConsultSession.FindAsync(id);

            if (consultSession == null)
            {
                return NotFound();
            }

            return consultSession;
        }

        // PUT: api/ConsultSessions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConsultSession(int id, ConsultSession consultSession)
        {
            if (id != consultSession.Id)
            {
                return BadRequest();
            }

            _context.Entry(consultSession).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConsultSessionExists(id))
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

        // POST: api/ConsultSessions
        [HttpPost]
        public async Task<ActionResult<ConsultSession>> PostConsultSession(ConsultSession consultSession)
        {
            Console.WriteLine("Consult Session: " + consultSession.DoctorId + " - " + consultSession.RequestConsultId);
            try
            {
                _context.ConsultSession.Add(consultSession);
                await _context.SaveChangesAsync();
                ConsultSession c = new ConsultSession {
                    Id = consultSession.Id,
                    RequestConsultId = consultSession.RequestConsultId,
                };
                return Ok(c);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return BadRequest("Cannot create a new consult session");

        }

        // DELETE: api/ConsultSessions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ConsultSession>> DeleteConsultSession(int id)
        {
            var consultSession = await _context.ConsultSession.FindAsync(id);
            if (consultSession == null)
            {
                return NotFound();
            }

            _context.ConsultSession.Remove(consultSession);
            await _context.SaveChangesAsync();

            return consultSession;
        }

        private bool ConsultSessionExists(int id)
        {
            return _context.ConsultSession.Any(e => e.Id == id);
        }
    }
}

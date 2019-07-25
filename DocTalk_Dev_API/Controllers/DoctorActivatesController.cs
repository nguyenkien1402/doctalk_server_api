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
    [Route("api/doctor/session/")]
    [ApiController]
    [Authorize]
    public class DoctorActivatesController : Controller
    {
        private readonly DocTalkDevContext _context;

        public DoctorActivatesController(DocTalkDevContext context)
        {
            _context = context;
        }


        [HttpGet("activate")]
        public ActionResult DoctorActivateDeactivateSession(int doctorId, int state)
        {
            if(_context.DoctorActivate.Where(p => p.Activate == true & p.DoctorId == doctorId & p.EndTime == null).Count() >= 1 && state == 1)
            {
                var result = new { meta = "Ok", message = "Doctor is already activate" };
                return Ok(result);
            }
            if(_context.Doctor.Find(doctorId) != null)
            {
                if (state == 1)
                {
                    try
                    {
                        var doctorActivate = new DoctorActivate { Activate = true, DoctorId = doctorId };
                        _context.DoctorActivate.Add(doctorActivate);
                        _context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return BadRequest();
                    }
                    var result = new { status = "OK", message = "Doctor Activate Session" };
                    return Ok(result);
                }
                else
                {
                    if (_context.DoctorActivate.Where(p => p.DoctorId == doctorId && p.Activate == true).Count() >= 1)
                    {
                        var doctorDeactivate = _context.DoctorActivate.Where(p => p.DoctorId == doctorId && p.Activate == true).Single();
                        doctorDeactivate.Activate = false;
                        doctorDeactivate.EndTime = DateTime.Now;
                        _context.Entry(doctorDeactivate).State = EntityState.Modified;
                        _context.SaveChanges();
                        var result = new { status = "OK", message = "Deactivate session successfully" };
                        return Ok(result);
                    }
                    return BadRequest();
                }
            }
            else
            {
                var result = new { status = "Bad", message = "Incorrect doctorID" };
                return BadRequest(result);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorActivate>>> GetDoctorActivate()
        {
            return await _context.DoctorActivate.ToListAsync();
        }

        // GET: api/DoctorActivates/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorActivate>> GetDoctorActivate(int id)
        {
            var doctorActivate = await _context.DoctorActivate.FindAsync(id);

            if (doctorActivate == null)
            {
                return NotFound();
            }

            return doctorActivate;
        }

        // PUT: api/DoctorActivates/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctorActivate(int id, DoctorActivate doctorActivate)
        {
            if (id != doctorActivate.Id)
            {
                return BadRequest();
            }

            _context.Entry(doctorActivate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorActivateExists(id))
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

        // POST: api/DoctorActivates
        [HttpPost]
        public async Task<ActionResult<DoctorActivate>> PostDoctorActivate(DoctorActivate doctorActivate)
        {
            _context.DoctorActivate.Add(doctorActivate);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDoctorActivate", new { id = doctorActivate.Id }, doctorActivate);
        }

        // DELETE: api/DoctorActivates/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DoctorActivate>> DeleteDoctorActivate(int id)
        {
            var doctorActivate = await _context.DoctorActivate.FindAsync(id);
            if (doctorActivate == null)
            {
                return NotFound();
            }

            _context.DoctorActivate.Remove(doctorActivate);
            await _context.SaveChangesAsync();

            return doctorActivate;
        }

        private bool DoctorActivateExists(int id)
        {
            return _context.DoctorActivate.Any(e => e.Id == id);
        }
    }
}

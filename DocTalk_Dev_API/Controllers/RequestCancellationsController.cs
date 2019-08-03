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
    [Route("api/requestcancel/")]
    [ApiController]
    [Authorize]
    public class RequestCancellationsController : Controller
    {
        private readonly DocTalkDevContext _context;

        public RequestCancellationsController(DocTalkDevContext context)
        {
            _context = context;
        }


        /*
         * When doctor cancel the request from patient
         */
        [HttpGet("cancelby")]
        public async Task<ActionResult> CancellationRequest(int doctorId, int requestId)
        {
            try {
                RequestCancellation requestCancellation = new RequestCancellation { DoctorId = doctorId, RequestConsultId = requestId, Reason = "No Reason" };
                _context.RequestCancellation.Add(requestCancellation);
                await _context.SaveChangesAsync();
                var result = new
                {

                    isOk = true,
                    docId = doctorId,
                    reqId = requestId,
                    reason = "No reason"
                };
                return Ok(result);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return Conflict();
            }
        }


        // GET: api/RequestCancellations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestCancellation>>> GetRequestCancellation()
        {
            return await _context.RequestCancellation.ToListAsync();
        }

        // GET: api/RequestCancellations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestCancellation>> GetRequestCancellation(int id)
        {
            var requestCancellation = await _context.RequestCancellation.FindAsync(id);

            if (requestCancellation == null)
            {
                return NotFound();
            }

            return requestCancellation;
        }

        // PUT: api/RequestCancellations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequestCancellation(int id, RequestCancellation requestCancellation)
        {
            if (id != requestCancellation.Id)
            {
                return BadRequest();
            }

            _context.Entry(requestCancellation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestCancellationExists(id))
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

        // POST: api/RequestCancellations
        [HttpPost]
        public async Task<ActionResult<RequestCancellation>> PostRequestCancellation(RequestCancellation requestCancellation)
        {
            _context.RequestCancellation.Add(requestCancellation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequestCancellation", new { id = requestCancellation.Id }, requestCancellation);
        }

        // DELETE: api/RequestCancellations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RequestCancellation>> DeleteRequestCancellation(int id)
        {
            var requestCancellation = await _context.RequestCancellation.FindAsync(id);
            if (requestCancellation == null)
            {
                return NotFound();
            }

            _context.RequestCancellation.Remove(requestCancellation);
            await _context.SaveChangesAsync();

            return requestCancellation;
        }

        private bool RequestCancellationExists(int id)
        {
            return _context.RequestCancellation.Any(e => e.Id == id);
        }
    }
}

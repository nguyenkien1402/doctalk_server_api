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
    [Route("api/requestconsult/")]
    [ApiController]
    [Authorize]
    public class RequestConsultsController : Controller
    {
        private readonly DocTalkDevContext _context;

        public RequestConsultsController(DocTalkDevContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> RequestConsultSession(RequestConsult model)
        {
            try
            {
                _context.RequestConsult.Add(model);
                await _context.SaveChangesAsync();
                var resul = new
                {
                    meta = new { status = "OK", Message = "Post Question Successfully " },
                    data = model
                };
                return Ok(model);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest();
            }
        }

        [HttpGet("searchingdoctor/{requestid}")]
        public async Task<ActionResult> SearchingDoctor(int requestId)
        {

            if (_context.RequestConsult.Find(requestId) == null)
            {
                return BadRequest();
            }
            var request = _context.RequestConsult.Find(requestId);
            List<string> specifications = request.Specification.Split(",").ToList();
            // Find all the needed specifications
            var professional_list = _context.Professional.Where(p => specifications.Contains(p.Code));
            
            // Find all the potential doctors
            List<int> professionalsId = new List<int>();
            foreach(var profess in professional_list)
            {
                Console.WriteLine("Profess ID:" + profess.Id);
                professionalsId.Add(profess.Id);
            }
            var potentialDoctors = _context.DoctorProfessional.Where(p => professionalsId.Contains(p.ProfessionalId))
                                            .Include(p => p.Doctor);
            List<int> doctorsId = new List<int>();
            foreach(var doctor in potentialDoctors)
            {
                Console.WriteLine("Doctor ID: " + doctor.DoctorId);
                doctorsId.Add(doctor.DoctorId);
            }
            // Find all the potential doctors with activate status
            var activateDoctors = _context.DoctorActivate.Where(p => p.Activate == true && doctorsId.Contains(p.DoctorId))
                                                            .Include(p => p.Doctor);

            foreach (var doctor in activateDoctors)
            {
                Console.WriteLine("doctor id: " + doctor.DoctorId);
            }
            List<DoctorActivate> listActivateDoctors = activateDoctors.ToList();
            // Then choose a doctor base on few condition.
            // And return the value of few doctor will reponse for the question
            // Transfer the question to the first priority doctor
            // If the doctor doesn't reponse in a neat of time, then redirect the question to another doctor.
            Doctor sendBackDoctor = null;
            bool isDoctorOk = false;
            while(listActivateDoctors.Count() > 0)
            {
                // Choose a doctor base on condition
                //**** Doctor selectedDoctor = SelectedDoctor(activateDoctors) ****//
                // But for now, we are going to use the random number
                Random r = new Random();
                int indexDoctors = r.Next(0, listActivateDoctors.Count());
                sendBackDoctor = listActivateDoctors.ElementAt(indexDoctors).Doctor;
                // Remove the current doctor our of the list
                listActivateDoctors.RemoveAt(indexDoctors);
                // Simultaneously justtify the doctor
                //**** Justify the doctor ****//
                // Send detail information to Doctor
                //**** isDoctorOk = SendRequestDetailDoctor(requestID, selectedDoctor) ****//
                // if isDoctor ok, then break the loop and send detail of doctor to user
                break;
            }
            if(isDoctorOk == true && sendBackDoctor != null)
            {
                return Ok(sendBackDoctor);
            }

            var resultNotOk = new
            {
                meta = new { status = "NOTFOUND", message = "Could not find the doctor" },
                request = request.ToString(),
                message = "Could not find the doctor for the requirement"
            };
            return View(resultNotOk);
        }


        // GET: api/RequestConsults
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestConsult>>> GetRequestConsult()
        {
            return await _context.RequestConsult.ToListAsync();
        }

        // GET: api/RequestConsults/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestConsult>> GetRequestConsult(int id)
        {
            var requestConsult = await _context.RequestConsult.FindAsync(id);

            if (requestConsult == null)
            {
                return NotFound();
            }

            return requestConsult;
        }

        // PUT: api/RequestConsults/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequestConsult(int id, RequestConsult requestConsult)
        {
            if (id != requestConsult.Id)
            {
                return BadRequest();
            }

            _context.Entry(requestConsult).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestConsultExists(id))
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


        // DELETE: api/RequestConsults/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RequestConsult>> DeleteRequestConsult(int id)
        {
            var requestConsult = await _context.RequestConsult.FindAsync(id);
            if (requestConsult == null)
            {
                return NotFound();
            }

            _context.RequestConsult.Remove(requestConsult);
            await _context.SaveChangesAsync();

            return requestConsult;
        }

        private bool RequestConsultExists(int id)
        {
            return _context.RequestConsult.Any(e => e.Id == id);
        }
    }
}

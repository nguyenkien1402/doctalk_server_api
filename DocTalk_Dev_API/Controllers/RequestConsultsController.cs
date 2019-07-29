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
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.SignalR;

namespace DocTalk_Dev_API.Controllers
{
    [Route("api/requestconsult/")]
    [ApiController]
    [Authorize]
    public class RequestConsultsController : Controller
    { 
        private readonly DocTalkDevContext _context;
        private readonly IHttpClientFactory _clientFactory;

        public RequestConsultsController(DocTalkDevContext context, IHttpClientFactory clientFactory)
        {
            _context = context;
            _clientFactory = clientFactory;
        }

        [HttpPost]
        public async Task<ActionResult> RequestConsultSession([FromBody]RequestConsultView model)
        {
            // Save the RequestConsult First
            var requestConsult = new RequestConsult
            {
                BriefOverview = model.BriefOverview,
                Inquiry = model.Inquiry,
                Specification = model.Specification,    
                Urgent = model.Urgent,
                PatientId = model.PatientId
            };
            //await _context.RequestConsult.AddAsync(requestConsult);
            var requestConsultDocument = from document in model.RequestConsultDocument
                                         select new RequestConsultDocument
                                         {
                                             DocumentName = document.DocumentName,
                                             DocumentLink = document.DocumentLink,
                                             DocumentType = document.DocumentType
                                         };
            requestConsult.RequestConsultDocument = requestConsultDocument.ToList();
            await _context.RequestConsult.AddAsync(requestConsult);
            await _context.SaveChangesAsync();
            try
            {
                var result = new
                {
                    status = "OK",
                    RequestId = requestConsult.Id
                    //Model = model
                };
                return Ok(result);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest();
            }
            
        }

        [HttpGet("simpletest")]
        public void Test()
        {
            List<int> result = new List<int>();
            for(int i = 0; i < 10; i++)
            {
                result.Add(i);
            }
            var content = new
            {
                data = new
                {
                    title = "Hey",
                    content = "Check Out This Awesome Game!",
                    imageUrl = "http://h5.4j.com/thumb/Ninja-Run.jpg",
                    gameUrl = "https://h5.4j.com/Ninja-Run/index.php?pubid=noa",
                    userId = result
                },
                to = "/topics/all"
            };

            var json = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            Console.WriteLine("JSON: " + json);

        }

        public async Task<Boolean> SendNotification(List<String> userIds, int requestId)
        {
            Console.WriteLine("UserId", userIds);
            var request = new HttpRequestMessage(HttpMethod.Post,
                "https://fcm.googleapis.com/fcm/send");

            var content = new {
                data = new {
                    id = requestId,
                    title = "Hey",
                    content = "Check Out This Awesome Game!",
                    imageUrl = "http://h5.4j.com/thumb/Ninja-Run.jpg",
                    gameUrl = "https://h5.4j.com/Ninja-Run/index.php?pubid=noa",
                    userId = userIds
                },
                to = "/topics/doctor_"+ userIds[0]
            };

            var json = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            request.Content = stringContent;

            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=AAAAST4PGVw:APA91bHGopJqbpePv79V5qiClVfF4PIm6N0s09MN889BqlfgfXvCQfkO4sSTyeyP0Yr5WCvftz7ftqqoJSJ2SwGS_d44-Rw01PtIqJnuL-p_6oXqY1uKUbffixfXsZiBtbRmn7Do7V2u");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpGet("searchingdoctor/{requestId}")]
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
            foreach (var profess in professional_list)
            {
                Console.WriteLine("Profess ID:" + profess.Id);
                professionalsId.Add(profess.Id);
            }
            var potentialDoctors = _context.DoctorProfessional.Where(p => professionalsId.Contains(p.ProfessionalId))
                                            .Include(p => p.Doctor);
            ;
            var doctorsId = new List<int>();
            foreach (var p in potentialDoctors)
            {
                if (!doctorsId.Contains(p.DoctorId))
                {
                    Console.WriteLine("DoctorId: " + p.DoctorId);
                    doctorsId.Add(p.DoctorId);
                }

            }
            // Find all the potential doctors with activate status
            var activateDoctors = _context.DoctorActivate.Where(p => p.Activate == true && doctorsId.Contains(p.DoctorId))
                                                            .Include(p => p.Doctor);

            foreach (var doctor in activateDoctors)
            {
                Console.WriteLine("doctor id: " + doctor.DoctorId);
            }
            List<DoctorActivate> listActivateDoctors = activateDoctors.ToList();
            List<Doctor> listDoctors = new List<Doctor>();
            foreach (DoctorActivate da in listActivateDoctors)
            {
                listDoctors.Add(new Doctor
                {
                    Id = da.Doctor.Id,
                    FirstName = da.Doctor.FirstName,
                    LastName = da.Doctor.LastName,
                    PreferName = da.Doctor.PreferName,
                    JoinedDate = da.Doctor.JoinedDate,
                    UserId = da.Doctor.UserId
                });
            }
            // Then choose a doctor base on few condition.
            // And return the value of few doctor will reponse for the question
            // Transfer the question to the first priority doctor
            // If the doctor doesn't reponse in a neat of time, then redirect the question to another doctor. ( This will be done in the doctorapp )
            // Sort List of doctor to get the best match the doctor at first, second match later.
            // ***** List<Doctor> selectedDoctors = GetTheBestMatchDoctor(listDoctors); ****//
            // Then get the UserId of doctor to send push notification
            if(listDoctors.Count() > 0)
            {
                return Ok(listDoctors);
            }
            else
            {
                return NotFound("Not Found Any Potential Doctor");
            }
 
        }


        /*[HttpGet("searchingdoctor/{requestId}")]
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
            ;
            var doctorsId = new List<int>();
            foreach(var p in potentialDoctors)
            {
                if (!doctorsId.Contains(p.DoctorId))
                {
                    Console.WriteLine("DoctorId: " + p.DoctorId);
                    doctorsId.Add(p.DoctorId);
                }
                
            }
            // Find all the potential doctors with activate status
            var activateDoctors = _context.DoctorActivate.Where(p => p.Activate == true && doctorsId.Contains(p.DoctorId))
                                                            .Include(p => p.Doctor);

            foreach (var doctor in activateDoctors)
            {
                Console.WriteLine("doctor id: " + doctor.DoctorId);
            }
            List<DoctorActivate> listActivateDoctors = activateDoctors.ToList();
            List<Doctor> listDoctors = new List<Doctor>();
            foreach(DoctorActivate da in listActivateDoctors)
            {
                listDoctors.Add(da.Doctor);
            }
            // Then choose a doctor base on few condition.
            // And return the value of few doctor will reponse for the question
            // Transfer the question to the first priority doctor
            // If the doctor doesn't reponse in a neat of time, then redirect the question to another doctor. ( This will be done in the doctorapp )
            // Sort List of doctor to get the best match the doctor at first, second match later.
            // ***** List<Doctor> selectedDoctors = GetTheBestMatchDoctor(listDoctors); ****
            // Then get the UserId of doctor to send push notification
            List<String> userIds = new List<String>();
            foreach(Doctor d in listDoctors)
            {
                userIds.Add(d.UserId);
            }
            var isFirstSendOk = false;
            isFirstSendOk = await SendNotification(userIds, requestId);
            if(isFirstSendOk)
            {
                var resultOk = new { status = "OK", message = "Finding the doctor" };
                return Ok(resultOk);
            }
            else
            {
                var resultNotOk = new
                {
                    status = "NOTFOUND",
                    request = request.ToString(),
                    message = "Could not find the doctor for the requirement"
                };
                return NotFound(resultNotOk);
            }

        }*/


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

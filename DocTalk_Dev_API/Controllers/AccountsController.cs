using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DocTalk_Dev_API.Models;
using System.Collections;
using Microsoft.AspNetCore.Authorization;
using DocTalk_Dev_API.Models;
using DocTalk_Dev_API.Views;
using IdentityModel.Client;
using System.Net.Http;

namespace DocTalk_Dev_API.Controllers
{
    [Route("api/account/")]
    [ApiController]
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly DocTalkDevContext _context;

        public AccountsController(DocTalkDevContext context)
        {
            _context = context;
        }


        [HttpPost]
        [Route("register/doctor")]
        public async Task<ActionResult> RegisterAsDoctor([FromBody]DoctorRegisterView model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var check_duplicated = _context.Doctor.Any(e => e.UserId == model.UserId);
            if (check_duplicated == true)
            {
                return Conflict();
            }
            var user = await _context.AspNetUsers.FindAsync(model.UserId);
            if(user == null)
            {
                return NotFound();
            }
            
            var doctor = new Doctor
            {
                UserId = model.UserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PreferName = model.PreferName,
                ClinicAddress = model.ClinicAddress,
                ClinicPostCode = model.ClinicPostCode,
                ClinicState = model.ClinicState,
                ClinicSuburb = model.ClinicSuburb,
            };

            try
            {
                // Save the Role 
                // var role = _context.AspNetRoles.Where(r => r.Name == "Doctor").Single();
                var user_role = new AspNetUserRoles { UserId = model.UserId, RoleId = "1" };
                await _context.AspNetUserRoles.AddAsync(user_role);
                doctor.User = user;
                await _context.Doctor.AddAsync(doctor);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Conflict();
            }
            var result = new
            {
                Meta = new { Status = "OK", Message = "Register as A Doctor" },
                DoctorId = doctor.Id,
                Doctor = model
            };
            return Ok(result);


        }

        [HttpPost]
        [Route("register/patient")]
        public async Task<ActionResult> RegisterAsPatient([FromBody]PatientRegisterView model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var check_duplicated = _context.Patient.Any(e => e.UserId == model.UserId);
            if (check_duplicated == true)
            {
                return Conflict();
            }
            var user = await _context.AspNetUsers.FindAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }
            var patient = new Patient
            {
                UserId = model.UserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Paddress = model.Paddress,
                PreferName = model.PreferName,
                PostCode = model.PostCode,
                Suburb = model.Suburb,
                Pstate = model.Pstate
                //User = user
            };
            try
            {
                await _context.Patient.AddAsync(patient);
                await _context.SaveChangesAsync();
            }catch(Exception e)
            {
                return Conflict();
               
            }
            
            var result = new { Meta = new { Status = "OK", Message = "Register as A Patient" },
                                PatientId = patient.Id,
                                Patient = model};
            return Ok(result);
        }

        [HttpPost]
        [Route("addprofessional/")]
        public ActionResult AddProfessional([FromBody]DoctorProfessionalView model)
        {
            try
            {
                foreach (int professId in model.ProfessionalId)
                {
                    if(_context.DoctorProfessional.Where(dp => dp.DoctorId == model.DoctorId && dp.ProfessionalId == professId).Count() == 0)
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
            }catch(Exception e)
            {
                return Conflict();
            }

            var result = new
            {
                Meta = new { Status = "OK", Message = "Add Professioanl Successfully" }
            };
            return Ok(result);
        }

        [HttpGet("token/{username}/{password}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetTokenForMobileAppAsync(String username, String password)
        {
            Console.WriteLine("username: " + username);
            Console.WriteLine("password: " + password);
            // Call API to get the token
            var apiClientCredentials = new PasswordTokenRequest
            {
                Address = "http://192.168.132.1:5000/connect/token",

                ClientId = "ro.client",
                ClientSecret = "secret",
                // This is the scope our Protected API requires. 
                Scope = "openid profile doctalk_auth_api",
                UserName = username,
                Password = password
            };

            var client = new HttpClient();

            // 1. Authenticates and get an access token from Identity Server
            var tokenResponse = await client.RequestPasswordTokenAsync(apiClientCredentials);
            var result_token = tokenResponse.Json;
            if (tokenResponse.IsError)
            {
                Console.WriteLine("Cannot get the data");
                return StatusCode(500);
            }

            return Ok(result_token);
        }
    }
}
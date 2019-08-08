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
using DocTalk_Dev_API.Utilities;

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

        

        [HttpGet("token/{username}/{password}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetTokenForMobileAppAsync(String username, String password)
        {
            Boolean isDoctor = false;
            AspNetUsers user = _context.AspNetUsers.Where(a => a.UserName == username).FirstOrDefault();
            //Get userID first.
            if (user != null)
            {
                isDoctor = true;
            }
            // Call API to get the token
            var apiClientCredentials = new PasswordTokenRequest
            {
                Address = Messages.AUTH_IP_ADDRESS_URL+"/connect/token",

                ClientId = "ro.client",
                ClientSecret = "secret",
                // This is the scope our Protected API requires. 
                Scope = "openid email profile doctalk_auth_api",
                UserName = username,
                Password = password
            };

            var client = new HttpClient();

            // 1. Authenticates and get an access token from Identity Server
            var tokenResponse = await client.RequestPasswordTokenAsync(apiClientCredentials);
            var result_token = tokenResponse.Json;
            result_token.Add("isDoctor", isDoctor);
            if (tokenResponse.IsError)
            {
                Console.WriteLine("Cannot get the data");
                return StatusCode(500);
            }

            return Ok(result_token);
        }

        [HttpGet("doctor/token/{username}/{password}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetTokenForDoctorMobileAppAsync(String username, String password)
        {
            // Check if user is doctor
            Boolean isDoctor = false;
            AspNetUsers user = _context.AspNetUsers.Where(a => a.UserName == username).FirstOrDefault();
            //Get userID first.
            if (user != null)
            {
                // Check if user is doctor
                Doctor doctor = _context.Doctor.Where(d => d.UserId == user.Id).FirstOrDefault();
                if (doctor != null)
                {
                    if (doctor.ConfirmDoctor == true)
                    {
                        isDoctor = true;
                    }
                    else
                    {
                        var result = new { status = "Bad Request", message = "Doctor has not activated yet" };
                        return BadRequest(result);
                    }
                }
            }
            else
            {
                var result = new { status = "Bad Request", message = "Incorrect Username" };
                return NotFound(result);
            }

            // Call API to get the token
            var apiClientCredentials = new PasswordTokenRequest
            {
                Address = "http://192.168.132.1:5000/connect/token",

                ClientId = "ro.client",
                ClientSecret = "secret",
                // This is the scope our Protected API requires. 
                Scope = "openid email profile doctalk_auth_api",
                UserName = username,
                Password = password
            };

            var client = new HttpClient();

            // 1. Authenticates and get an access token from Identity Server
            var tokenResponse = await client.RequestPasswordTokenAsync(apiClientCredentials);
            var result_token = tokenResponse.Json;
            result_token.Add("isDoctor", isDoctor);
            if (tokenResponse.IsError)
            {
                Console.WriteLine("Cannot get the data");
                return StatusCode(500);
            }

            return Ok(result_token);
        }
    }
}
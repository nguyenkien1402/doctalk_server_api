using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace DocTalk_Dev_API.Hubs
{
    public class SearchingDoctorHub : Hub
    {
        /*private readonly IHttpClientFactory _clientFactory;

        public SearchingDoctorHub(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }*/
        /*public async Task SearchingAvailableDoctor(int requestId, String patientId, String doctorId)
        {
            Console.WriteLine("Call Hub: "+requestId + " " + patientId + " " +doctorId);
            var request = new HttpRequestMessage(HttpMethod.Post,
                "https://fcm.googleapis.com/fcm/send");

            var content = new
            {
                data = new
                {
                    id = requestId,
                    title = "Hey",
                    content = "Check Out This Awesome Game!",
                    imageUrl = "http://h5.4j.com/thumb/Ninja-Run.jpg",
                    gameUrl = "https://h5.4j.com/Ninja-Run/index.php?pubid=noa",
                    patientId = patientId
                },
                to = "/topics/doctor_" + doctorId
            };

            var json = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            request.Content = stringContent;

            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=AAAAST4PGVw:APA91bHGopJqbpePv79V5qiClVfF4PIm6N0s09MN889BqlfgfXvCQfkO4sSTyeyP0Yr5WCvftz7ftqqoJSJ2SwGS_d44-Rw01PtIqJnuL-p_6oXqY1uKUbffixfXsZiBtbRmn7Do7V2u");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
            var response = await client.SendAsync(request);
        }*/


        public async Task GettingDoctorResponse(int requestId, string patientId, string doctorId, int code)
        {
            // code == 0 mean reject, code == 1 mean accept
            Console.WriteLine("Call Hub: " + requestId + "/ " + patientId + "/ " + doctorId +"/"+code);
            await Clients.Others.SendAsync(patientId, code+"");
        }
    }
}

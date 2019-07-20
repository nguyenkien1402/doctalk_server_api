using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DocTalk_Dev_API.Views
{
    public class PatientRegisterView
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PreferName { get; set; }
        public string Paddress { get; set; }
        public string Suburb { get; set; }
        public string Pstate { get; set; }
        public int PostCode { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocTalk_Dev_API.Views
{
    public class DoctorRegisterView
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PreferName { get; set; }
        public string ClinicName { get; set;    }
        public string ClinicAddress { get; set; }
        public string ClinicSuburb { get; set; }
        public string ClinicState { get; set; }
        public int? ClinicPostCode { get; set; }
    }
}

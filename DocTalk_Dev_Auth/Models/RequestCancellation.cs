using System;
using System.Collections.Generic;

namespace DocTalk_Dev_Auth.Models
{
    public partial class RequestCancellation
    {
        public int Id { get; set; }
        public int RequestConsultId { get; set; }
        public int DoctorId { get; set; }
        public string Reason { get; set; }

        public Doctor Doctor { get; set; }
        public RequestConsult RequestConsult { get; set; }
    }
}

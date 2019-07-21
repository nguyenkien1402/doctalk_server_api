using System;
using System.Collections.Generic;

namespace DocTalk_Dev_Auth.Models
{
    public partial class RequestConsult
    {
        public RequestConsult()
        {
            ConsultSession = new HashSet<ConsultSession>();
            RequestCancellation = new HashSet<RequestCancellation>();
        }

        public int Id { get; set; }
        public string BriefOverview { get; set; }
        public string Inquiry { get; set; }
        public DateTime? InquiryTime { get; set; }
        public int? Urgent { get; set; }
        public int PatientId { get; set; }
        public string Specification { get; set; }

        public Patient Patient { get; set; }
        public ICollection<ConsultSession> ConsultSession { get; set; }
        public ICollection<RequestCancellation> RequestCancellation { get; set; }
    }
}

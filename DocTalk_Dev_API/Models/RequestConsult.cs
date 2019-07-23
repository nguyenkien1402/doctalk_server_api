using System;
using System.Collections.Generic;

namespace DocTalk_Dev_API.Models
{
    public partial class RequestConsult
    {
        public RequestConsult()
        {
            ConsultSession = new HashSet<ConsultSession>();
            RequestCancellation = new HashSet<RequestCancellation>();
            RequestConsultDocument = new HashSet<RequestConsultDocument>();
        }

        public int Id { get; set; }
        public string BriefOverview { get; set; }
        public string Inquiry { get; set; }
        public DateTime? InquiryTime { get; set; }
        public int? Urgent { get; set; }
        public int PatientId { get; set; }
        public string Specification { get; set; }

        public virtual Patient Patient { get; set; }
        public virtual ICollection<ConsultSession> ConsultSession { get; set; }
        public virtual ICollection<RequestCancellation> RequestCancellation { get; set; }
        public virtual ICollection<RequestConsultDocument> RequestConsultDocument { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocTalk_Dev_API.Models;

namespace DocTalk_Dev_API.Views
{
    public class RequestConsultView
    {
        public string BriefOverview { get; set; }
        public string Inquiry { get; set; }
        public int? Urgent { get; set; }
        public int PatientId { get; set; }
        public string Specification { get; set; }
        public virtual ICollection<RequestConsultDocument> RequestConsultDocument { get; set; }
    }
}

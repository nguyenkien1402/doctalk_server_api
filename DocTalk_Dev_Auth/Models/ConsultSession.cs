using System;
using System.Collections.Generic;

namespace DocTalk_Dev_Auth.Models
{
    public partial class ConsultSession
    {
        public int Id { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public int RequestConsultId { get; set; }
        public int DoctorId { get; set; }
        public int? StarRating { get; set; }
        public string Comment { get; set; }

        public Doctor Doctor { get; set; }
        public RequestConsult RequestConsult { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DocTalk_Dev_Auth.Models
{
    public partial class DoctorDocuments
    {
        public int Id { get; set; }
        public string DocumentType { get; set; }
        public string DocumentName { get; set; }
        public string DocumentLink { get; set; }
        public bool? DocumentConfirm { get; set; }
        public int DoctorId { get; set; }

        public Doctor Doctor { get; set; }
    }
}

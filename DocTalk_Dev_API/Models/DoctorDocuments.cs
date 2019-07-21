using System;
using System.Collections.Generic;

namespace DocTalk_Dev_API.Models
{
    public partial class DoctorDocuments
    {
        public int Id { get; set; }
        public string DocumentType { get; set; }
        public string DocumentName { get; set; }
        public string DocumentLink { get; set; }
        public bool? DocumentConfirm { get; set; }
        public int DoctorId { get; set; }

        public virtual Doctor Doctor { get; set; }
    }
}

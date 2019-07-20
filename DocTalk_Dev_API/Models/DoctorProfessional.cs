using System;
using System.Collections.Generic;

namespace DocTalk_Dev_API.Models
{
    public partial class DoctorProfessional
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int ProfessionalId { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual Professional Professional { get; set; }
    }
}

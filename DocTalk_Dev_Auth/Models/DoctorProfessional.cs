using System;
using System.Collections.Generic;

namespace DocTalk_Dev_Auth.Models
{
    public partial class DoctorProfessional
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int ProfessionalId { get; set; }

        public Doctor Doctor { get; set; }
        public Professional Professional { get; set; }
    }
}

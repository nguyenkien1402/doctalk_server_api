using System;
using System.Collections.Generic;

namespace DocTalk_Dev_Auth.Models
{
    public partial class DoctorActivate
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public bool Activate { get; set; }
        public DateTime? StartedTime { get; set; }
        public DateTime? EndTime { get; set; }

        public Doctor Doctor { get; set; }
    }
}

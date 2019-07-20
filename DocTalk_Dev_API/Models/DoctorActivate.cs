using System;
using System.Collections.Generic;

namespace DocTalk_Dev_API.Models
{
    public partial class DoctorActivate
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public bool Activate { get; set; }
        public DateTime? StartedTime { get; set; }
        public DateTime? EndTime { get; set; }

        public virtual Doctor Doctor { get; set; }
    }
}

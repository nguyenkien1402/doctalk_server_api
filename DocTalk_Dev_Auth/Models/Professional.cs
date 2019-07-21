using System;
using System.Collections.Generic;

namespace DocTalk_Dev_Auth.Models
{
    public partial class Professional
    {
        public Professional()
        {
            DoctorProfessional = new HashSet<DoctorProfessional>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }

        public ICollection<DoctorProfessional> DoctorProfessional { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DocTalk_Dev_API.Models
{
    public partial class Professional
    {
        public Professional()
        {
            DoctorProfessional = new HashSet<DoctorProfessional>();
            RequestConsult = new HashSet<RequestConsult>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }

        public virtual ICollection<DoctorProfessional> DoctorProfessional { get; set; }
        public virtual ICollection<RequestConsult> RequestConsult { get; set; }
    }
}

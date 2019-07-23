using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocTalk_Dev_API.Views
{
    public class DoctorProfessionalView
    {
        public int DoctorId { get; set; }
        public ICollection<int> ProfessionalId { get; set; }

        public ICollection<string> ProfessionalName { get; set; }
    }
}

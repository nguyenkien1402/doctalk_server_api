using System;
using System.Collections.Generic;

namespace DocTalk_Dev_Auth.Models
{
    public partial class Doctor
    {
        public Doctor()
        {
            ConsultSession = new HashSet<ConsultSession>();
            DoctorActivate = new HashSet<DoctorActivate>();
            DoctorDocuments = new HashSet<DoctorDocuments>();
            DoctorProfessional = new HashSet<DoctorProfessional>();
            RequestCancellation = new HashSet<RequestCancellation>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PreferName { get; set; }
        public string ClinicAddress { get; set; }
        public string ClinicSuburb { get; set; }
        public string ClinicState { get; set; }
        public int? ClinicPostCode { get; set; }
        public DateTime? JoinedDate { get; set; }
        public string UserId { get; set; }
        public string ClinicName { get; set; }
        public bool? ConfirmDoctor { get; set; }

        public AspNetUsers User { get; set; }
        public ICollection<ConsultSession> ConsultSession { get; set; }
        public ICollection<DoctorActivate> DoctorActivate { get; set; }
        public ICollection<DoctorDocuments> DoctorDocuments { get; set; }
        public ICollection<DoctorProfessional> DoctorProfessional { get; set; }
        public ICollection<RequestCancellation> RequestCancellation { get; set; }
    }
}

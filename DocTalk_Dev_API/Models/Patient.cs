using System;
using System.Collections.Generic;

namespace DocTalk_Dev_API.Models
{
    public partial class Patient
    {
        public Patient()
        {
            RequestConsult = new HashSet<RequestConsult>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PreferName { get; set; }
        public string Paddress { get; set; }
        public string Suburb { get; set; }
        public string Pstate { get; set; }
        public int PostCode { get; set; }
        public DateTime? JoinedDate { get; set; }
        public string UserId { get; set; }

        public virtual AspNetUsers User { get; set; }
        public virtual ICollection<RequestConsult> RequestConsult { get; set; }
    }
}

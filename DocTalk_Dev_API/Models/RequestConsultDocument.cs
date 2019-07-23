using System;
using System.Collections.Generic;

namespace DocTalk_Dev_API.Models
{
    public partial class RequestConsultDocument
    {
        public int Id { get; set; }
        public string DocumentType { get; set; }
        public string DocumentName { get; set; }
        public string DocumentLink { get; set; }
        public int RequestConsultId { get; set; }

        public virtual RequestConsult RequestConsult { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace PM.Entity.Models
{
    public partial class StatusType
    {
        public StatusType()
        {
            Request = new HashSet<Request>();
        }

        public int Id { get; set; }
        public string StatusTypeName { get; set; }

        public virtual ICollection<Request> Request { get; set; }
    }
}

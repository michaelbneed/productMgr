using System;
using System.Collections.Generic;

namespace PM.DatabaseOperations.Models
{
    public partial class StatusType
    {
        public StatusType()
        {
            Request = new HashSet<Request>();
        }

        public int StatusTypeId { get; set; }
        public string StatusType1 { get; set; }

        public virtual ICollection<Request> Request { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace PM.DatabaseOperations.Models
{
    public partial class RequestType
    {
        public RequestType()
        {
            Request = new HashSet<Request>();
        }

        public int RequestTypeId { get; set; }
        public string RequestType1 { get; set; }

        public virtual ICollection<Request> Request { get; set; }
    }
}

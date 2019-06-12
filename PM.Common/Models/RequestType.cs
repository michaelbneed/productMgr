using System;
using System.Collections.Generic;

namespace PM.Entity.Models
{
    public partial class RequestType
    {
        public RequestType()
        {
            Request = new HashSet<Request>();
        }

        public int Id { get; set; }
        public string RequestTypeName { get; set; }

        public virtual ICollection<Request> Request { get; set; }
    }
}

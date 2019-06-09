﻿using System;
using System.Collections.Generic;

namespace PM.DatabaseOperations.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            Request = new HashSet<Request>();
            User = new HashSet<User>();
        }

        public int Id { get; set; }
        public string SupplierName { get; set; }
        public string SupplierEmail { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<Request> Request { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}

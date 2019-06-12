using System;
using System.Collections.Generic;

namespace PM.Entity.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public int? SupplierId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string AuthId { get; set; }
        public bool? Enabled { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public virtual Supplier Supplier { get; set; }
    }
}

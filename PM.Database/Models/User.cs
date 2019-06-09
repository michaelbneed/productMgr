using System;
using System.Collections.Generic;

namespace PM.DatabaseOperations.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public int? SupplierId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public Guid? AuthId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public virtual Supplier Supplier { get; set; }
    }
}

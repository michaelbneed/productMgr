using System;
using System.Collections.Generic;

namespace PM.Entity.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            ProductPackageType = new HashSet<ProductPackageType>();
            Request = new HashSet<Request>();
            User = new HashSet<User>();
        }

        public int Id { get; set; }
        public string SupplierName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<ProductPackageType> ProductPackageType { get; set; }
        public virtual ICollection<Request> Request { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}

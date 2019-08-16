using System;
using System.Collections.Generic;

namespace PM.Entity.Models
{
    public partial class Store
    {
        public Store()
        {
            ProductStoreSpecific = new HashSet<ProductStoreSpecific>();
            Request = new HashSet<Request>();
        }

        public int Id { get; set; }
        public string StoreName { get; set; }
        public string StoreSupervisorName { get; set; }
        public string StoreSupervisorEmail { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<ProductStoreSpecific> ProductStoreSpecific { get; set; }
        public virtual ICollection<Request> Request { get; set; }
    }
}

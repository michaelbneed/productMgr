using System;
using System.Collections.Generic;

namespace PM.DatabaseOperations.Models
{
    public partial class Product
    {
        public Product()
        {
            Package = new HashSet<Package>();
            Request = new HashSet<Request>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string Upccode { get; set; }
        public string ProductLocation { get; set; }
        public decimal? ProductCost { get; set; }
        public decimal? ProductPrice { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<Package> Package { get; set; }
        public virtual ICollection<Request> Request { get; set; }
    }
}

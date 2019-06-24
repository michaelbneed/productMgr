using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PM.Entity.Models
{
    public partial class ProductPackageType
    {
        public int Id { get; set; }
        public decimal? Quantity { get; set; }
        public string Unit { get; set; }
		[Required]
        public string AlternateProductName { get; set; }
        public string AlternateProductUpccode { get; set; }
        public string SupplierData { get; set; }
        public int? SupplierId { get; set; }
        public decimal? AlternateProductPrice { get; set; }
        public decimal? AlternateProductCost { get; set; }
        public int? ProductId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public virtual Product Product { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}

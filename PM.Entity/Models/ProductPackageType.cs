﻿using System;
using System.Collections.Generic;

namespace PM.Entity.Models
{
    public partial class ProductPackageType
    {
        public ProductPackageType()
        {
            ProductStoreSpecific = new HashSet<ProductStoreSpecific>();
        }

        public int Id { get; set; }
        public decimal? Quantity { get; set; }
        public string Unit { get; set; }
        public string AlternateProductName { get; set; }
        public string AlternateProductUpccode { get; set; }
        public string SupplierData { get; set; }
        public int? SupplierId { get; set; }
        public decimal? AlternateProductPrice { get; set; }
        public decimal? AlternateProductCost { get; set; }
        public decimal? AlternateSuggestedPrice { get; set; }
        public int? ProductId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public virtual Product Product { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<ProductStoreSpecific> ProductStoreSpecific { get; set; }
    }
}

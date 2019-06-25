using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PM.Entity.Models
{
    public partial class ProductStoreSpecific
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? PackageTypeId { get; set; }
        [Required]
		public string StoreName { get; set; }
        public decimal? StorePrice { get; set; }
        public decimal? StoreCost { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ProductPackageType PackageType { get; set; }
        public virtual Product Product { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PM.Entity.Models
{
    public partial class Product
    {
        public Product()
        {
            ProductPackageType = new HashSet<ProductPackageType>();
            ProductStoreSpecific = new HashSet<ProductStoreSpecific>();
            Request = new HashSet<Request>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Product Name is required")]
        [MaxLength(100, ErrorMessage = "Product Name must be 100 characters or less")]
        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        [MaxLength(50, ErrorMessage = "UPC Code must be 50 characters or less")]
        public string Upccode { get; set; }

        public string ProductLocation { get; set; }

        public decimal? ProductCost { get; set; }

        public decimal? ProductPrice { get; set; }

        public decimal? SuggestedPrice { get; set; }

        [MaxLength(50, ErrorMessage = "Package Size must be 50 characters or less")]
        public string PackageSize { get; set; }

        public int? ContainerSizeTypeId { get; set; }

        public int? ContainerTypeId { get; set; }

        public string OrderWeek { get; set; }

        public int? CategoryId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }

        public int? UnitsPerCase { get; set; }

		public string SupplierData { get; set; }

		public virtual Category Category { get; set; }

        public virtual ContainerSizeType ContainerSizeType { get; set; }

        public virtual ContainerType ContainerType { get; set; }

        public virtual ICollection<ProductPackageType> ProductPackageType { get; set; }

        public virtual ICollection<ProductStoreSpecific> ProductStoreSpecific { get; set; }

        public virtual ICollection<Request> Request { get; set; }
    }
}

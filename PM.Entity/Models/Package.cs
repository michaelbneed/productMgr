using System;
using System.Collections.Generic;

namespace PM.Entity.Models
{
    public partial class Package
    {
        public int Id { get; set; }
        public string PackageName { get; set; }
        public decimal? PackagePrice { get; set; }
        public decimal? PackageSize { get; set; }
        public int? ProductId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public virtual Product Product { get; set; }
    }
}

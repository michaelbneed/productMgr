﻿using System;
using System.Collections.Generic;

namespace PM.DatabaseOperations.Models
{
    public partial class Package
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public decimal? PackagePrice { get; set; }
        public int? ProductId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public virtual Product Product { get; set; }
    }
}

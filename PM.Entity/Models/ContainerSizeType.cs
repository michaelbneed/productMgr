using System;
using System.Collections.Generic;

namespace PM.Entity.Models
{
    public partial class ContainerSizeType
    {
        public ContainerSizeType()
        {
            Product = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string ContainerSizeTypeName { get; set; }

        public virtual ICollection<Product> Product { get; set; }
    }
}

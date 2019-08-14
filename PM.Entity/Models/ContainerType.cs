using System;
using System.Collections.Generic;

namespace PM.Entity.Models
{
    public partial class ContainerType
    {
        public ContainerType()
        {
            Product = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string ContainerTypeName { get; set; }

        public virtual ICollection<Product> Product { get; set; }
    }
}

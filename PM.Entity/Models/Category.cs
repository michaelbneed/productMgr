using System;
using System.Collections.Generic;

namespace PM.Entity.Models
{
    public partial class Category
    {
        public Category()
        {
            Product = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Product { get; set; }
    }
}

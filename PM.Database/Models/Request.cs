using System;
using System.Collections.Generic;

namespace PM.DatabaseOperations.Models
{
    public partial class Request
    {
        public Request()
        {
            Note = new HashSet<Note>();
        }

        public int Id { get; set; }
        public string RequestDescription { get; set; }
        public int? RequestTypeId { get; set; }
        public int? StatusTypeId { get; set; }
        public int? UserId { get; set; }
        public int? ProductId { get; set; }
        public int? SupplierId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public virtual Product Product { get; set; }
        public virtual RequestType RequestType { get; set; }
        public virtual StatusType StatusType { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<Note> Note { get; set; }
    }
}

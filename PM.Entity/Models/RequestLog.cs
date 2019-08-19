using System;
using System.Collections.Generic;

namespace PM.Entity.Models
{
    public partial class RequestLog
    {
        public int Id { get; set; }
        public int? RequestId { get; set; }
        public string RequestDescription { get; set; }
        public int? RequestTypeId { get; set; }
        public int? StatusTypeId { get; set; }
        public string UserId { get; set; }
        public int? ProductId { get; set; }
        public int? SupplierId { get; set; }
        public int? StoreId { get; set; }
        public string ChangeNote { get; set; }
        public DateTime? OriginalCreatedOnDate { get; set; }
        public string OriginalCreatedByUser { get; set; }
		public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}

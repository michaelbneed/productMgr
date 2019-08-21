using System;
using System.Collections.Generic;
using System.Text;

namespace PM.Entity.ViewModels
{
	public class RequestLogFull
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
		public string RequestTypeName { get; set; }
		public string StatusTypeName { get; set; }
		public string StoreName { get; set; }
		public string ProductName { get; set; }
		public string SupplierName { get; set; }
	}
}

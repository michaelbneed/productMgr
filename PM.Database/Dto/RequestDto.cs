using System;

namespace PM.DatabaseOperations.DtoModels
{
	public class RequestDto
	{
		public int RequestId { get; set; }

		public string RequestDescription { get; set; }

		public int UserId { get; set; }

		public string UserName { get; set; }

		public int ProductId { get; set; }

		public string ProductDescription { get; set; }

		public int RequestTypeId { get; set; }

		public string RequestType { get; set; }

		public DateTime CreateDate { get; set; }

		public DateTime UpdateDate { get; set; }

		public int StatusTypeId { get; set; }

		public string StatusType { get; set; }
	}
}

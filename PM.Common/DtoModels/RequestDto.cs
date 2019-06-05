using System;
using System.Collections.Generic;
using System.Text;

namespace PM.Common.DtoModels
{
	public class RequestDto
	{
		public int RequestId { get; set; }

		public int UserId { get; set; }

		public string UserName { get; set; }

		public int ProductId { get; set; }

		public int RequestTypeId { get; set; }

		public string RequestType { get; set; }

		public List<RequestTypesDto> RequestTypes { get; set; }

		public DateTime CreateDate { get; set; }

		public DateTime UpdateDate { get; set; }

		public int StatusTypeId { get; set; }

		public string StatusType { get; set; }

		public List<StatusTypesDto> StatusTypes { get; set; }

		public bool IsIssue { get; set; }
	}
}

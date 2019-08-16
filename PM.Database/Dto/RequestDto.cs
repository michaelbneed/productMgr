using System;

namespace PM.Business.Dto
{
	public static class RequestDto
	{
		public static int? RequestId { get; set; }
		public static string RequestDescription { get; set; }
		public static int? SupplierId { get; set; }
		public static int? StatusId { get; set; }
	}
}

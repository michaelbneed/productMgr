using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using PM.Entity.Models;
using PM.Entity.Services;

namespace PM.Business.RequestLogging
{
	public static class RequestLogHelper
	{
		private static IDbWriteService _dbWriteService;

		public static void LogRequestChange(Request request, VandivierProductManagerContext context, [Optional] string changeNote)
		{
			_dbWriteService = new DbWriteService(context);
			//TODO: Rescaffold and add RequestLog
			//_dbWriteService.Add<RequestLog>(request);
			_dbWriteService.SaveChangesAsync();
		}
	}

	public static class RequestLogConstants
	{
		public const string RequestAddByStaff = "Request Created by Staff";
		public const string RequestEditByStaff = "Request Edited by Staff";
		public const string RequestDeletedByStaff = "Request Deleted by Staff";
		public const string RequestAddByVendor = "Request Created by Vendor";
		public const string RequestEditByVendor = "Request Edited by Vendor";
		public const string RequestApproved = "Request Approved";
		public const string RequestDenied = "Request Denied";
		public const string RequestComplete = "Request Complete";
		public const string ProductAddByStaff = "Product Added to Request by Staff";
		public const string ProductAddByVendor = "Product Added to Request by Vendor";
		public const string ProductAndPackageAddByStaff = "Product and Package Added to Request by Staff";
		public const string ProductAndPackageAddByVendor = "Product and Package Added to Request by Vendor";
	}
}

using System;
using System.Runtime.InteropServices;
using PM.Business.Dto;
using PM.Entity.Models;
using PM.Entity.Services;
using PM.Entity.ViewModels;

namespace PM.Business.RequestLogging
{
	public  class RequestLogHelper
	{
		public RequestLogHelper(){}

		public void LogRequestChange(Request request, VandivierProductManagerContext context, [Optional] string changeNote)
		{
			//VandivierProductManagerContext context = new VandivierProductManagerContext();
			IDbWriteService dbWriteService = new DbWriteService(context);

			RequestLog requestToLog = null;
			requestToLog = new RequestLog();
			requestToLog.RequestId = request.Id;

			if (request.ProductId != null)
			{
				requestToLog.ProductId = request.ProductId;
			}
			
			requestToLog.ChangeNote = changeNote;
			requestToLog.RequestDescription = request.RequestDescription;
			requestToLog.RequestTypeId = request.RequestTypeId;
			requestToLog.StatusTypeId = request.StatusTypeId;
			requestToLog.StoreId = request.StoreId;
			requestToLog.SupplierId = request.SupplierId;

			requestToLog.UserId = request.UserId ?? string.Empty;

			requestToLog.OriginalCreatedOnDate = request.CreatedOn;
			requestToLog.OriginalCreatedByUser = request.CreatedBy;
			requestToLog.CreatedOn = DateTime.Now;
			requestToLog.CreatedBy = UserDto.UserId;

			dbWriteService.Add<RequestLog>(requestToLog);
			dbWriteService.SaveChangesAsync();
		}
	}

	public static class RequestLogConstants
	{
		public const string RequestAddByStaff = "New Staff Request";
		public const string RequestDeletedByStaff = "Request Deleted";
		public const string RequestAddByVendor = "New Vendor Request";
		public const string RequestApproved = "Request Approved";
		public const string RequestDenied = "Request Denied";
		public const string RequestComplete = "Request Complete";
		public const string ProductAddByStaff = "New Staff Product";
		public const string ProductAddByVendor = "New Vendor Product";
		public const string ProductAndPackageAddByStaff = "New Product and Package Staff";
		public const string ProductAndPackageAddByVendor = "New Product and Package Vendor";
	}
}

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
		//private IDbReadService _dbReadService;
		//private IDbWriteService _dbWriteService;
		//private readonly VandivierProductManagerContext _context;

		public RequestLogHelper()
		{
			//_dbReadService = dbReadService;
			//_dbWriteService = dbWriteService;
			//_context = context;
		}

		public void LogRequestChange(Request request, [Optional] string changeNote)
		{
			VandivierProductManagerContext context = new VandivierProductManagerContext();
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
		public const string RequestAddByStaff = "Request Created by Staff";
		public const string RequestDeletedByStaff = "Request Deleted by Staff";
		public const string RequestAddByVendor = "Request Created by Vendor";
		public const string RequestApproved = "Request Approved";
		public const string RequestDenied = "Request Denied";
		public const string RequestComplete = "Request Complete";
		public const string ProductAddByStaff = "Product Added to Request by Staff";
		public const string ProductAddByVendor = "Product Added to Request by Vendor";
		public const string ProductAndPackageAddByStaff = "Product and Package Added to Request by Staff";
		public const string ProductAndPackageAddByVendor = "Product and Package Added to Request by Vendor";
	}
}

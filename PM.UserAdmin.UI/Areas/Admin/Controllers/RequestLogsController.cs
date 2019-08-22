using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PM.Business.Dto;
using PM.Business.Security;
using PM.Entity.Models;
using PM.Entity.Services;
using PM.Entity.ViewModels;

namespace PM.UserAdmin.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RequestLogsController : Controller
    {
		private readonly VandivierProductManagerContext _context;
		private readonly IDbReadService _dbReadService;
		private readonly IDbWriteService _dbWriteService;
		private readonly IConfiguration _configuration;

		public RequestLogsController(IDbReadService dbReadService, IDbWriteService dbWriteService,
			VandivierProductManagerContext context, IConfiguration configuration)
		{
			_dbReadService = dbReadService;
			_dbWriteService = dbWriteService;
			_context = context;
			_configuration = configuration;
		}

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public async Task<IActionResult> Index(string sort, string search)
		{
			RequestDto.RequestDescription = string.Empty;

			if (search != null)
			{
				Regex rgx = new Regex("[^a-zA-Z0-9 -]");
				search = rgx.Replace(search, "").ToUpper();
			}


			// Intercept sort data
			ViewData["RequestIdParam"] = sort == "RequestId" ? "requestId_desc" : "RequestId";
			ViewData["RequestDescriptionParam"] = sort == "RequestDescription" ? "requestDescription_desc" : "RequestDescription";
			ViewData["ChangeNoteParam"] = sort == "ChangeNote" ? "changeNote_desc" : "ChangeNote";
			ViewData["StoreNameParam"] = sort == "StoreName" ? "store_desc" : "StoreName";
			ViewData["StatusTypeParam"] = sort == "StatusTypeName" ? "statusTypeName_desc" : "StatusTypeName";
			ViewData["RequestTypeParam"] = sort == "RequestTypeParam" ? "requestType_desc" : "RequestTypeParam";
			ViewData["ProductNameParam"] = sort == "ProductNameParam" ? "productName_desc" : "ProductNameParam";
			ViewData["SupplierNameParam"] = sort == "SupplierNameParam" ? "supplierName_desc" : "SupplierNameParam";
			ViewData["RequestDateParam"] = sort == "RequestDateParam" ? "requestDate_desc" : "RequestDateParam";
			ViewData["RequesterNameParam"] = sort == "RequesterNameParam" ? "requesterName_desc" : "RequesterNameParam";
			ViewData["CreatedOnParam"] = sort == "CreatedOnParam" ? "createdOn_desc" : "CreatedOnParam";
			ViewData["CreatedByParam"] = sort == "CreatedByParam" ? "createdBy_desc" : "CreatedByParam";
			ViewData["RequestLogIdParam"] = sort == "RequestLogIdParam" ? "requestLogId_Desc" : "RequestLogIdParam";

			// Intercept search term
			ViewData["FilterParam"] = search;

			// Map log data to view model with readable names
			IEnumerable<RequestLog> logs = await _dbReadService.GetAllRecordsAsync<RequestLog>();
			List<RequestLogFull> requestLogFullList = new List<RequestLogFull>();

			foreach (var item in logs)
			{
				RequestLogFull requestLogFull = null;
				requestLogFull = new RequestLogFull();

				requestLogFull.RequestId = item.RequestId;
				requestLogFull.ProductId = item.ProductId;
				requestLogFull.ChangeNote = item.ChangeNote;
				requestLogFull.RequestDescription = item.RequestDescription;
				requestLogFull.RequestTypeId = item.RequestTypeId;
				requestLogFull.StatusTypeId = item.StatusTypeId;
				requestLogFull.StoreId = item.StoreId;
				requestLogFull.SupplierId = item.SupplierId;
				requestLogFull.UserId = item.UserId;
				requestLogFull.OriginalCreatedOnDate = item.OriginalCreatedOnDate;
				requestLogFull.OriginalCreatedByUser = item.OriginalCreatedByUser;
				requestLogFull.CreatedOn = item.CreatedOn;
				requestLogFull.CreatedBy = item.CreatedBy;
				requestLogFull.Id = item.Id;

				var tempProduct = await _dbReadService.GetSingleRecordAsync<Product>(p => item.ProductId != null && p.Id.Equals(item.ProductId));
				if (tempProduct?.ProductName != null)
				{
					requestLogFull.ProductName = tempProduct.ProductName;
				}

				var tempSupplier = await _dbReadService.GetSingleRecordAsync<Supplier>(p => item.SupplierId != null && p.Id.Equals(item.SupplierId));
				if (tempSupplier?.SupplierName != null)
				{
					requestLogFull.SupplierName = tempSupplier.SupplierName;
				}

				var tempStore = await _dbReadService.GetSingleRecordAsync<Store>(p => item.StoreId != null && p.Id.Equals(item.StoreId));
				if (tempStore?.StoreName != null)
				{
					requestLogFull.StoreName = tempStore.StoreName;
				}

				var tempStatus = await _dbReadService.GetSingleRecordAsync<StatusType>(p => item.StatusTypeId != null && p.Id.Equals(item.StatusTypeId));
				if (tempStatus?.StatusTypeName != null)
				{
					requestLogFull.StatusTypeName = tempStatus.StatusTypeName;
				}

				var tempRequestType = await _dbReadService.GetSingleRecordAsync<RequestType>(p => p.Id.Equals(item.RequestTypeId));
				if (tempRequestType?.RequestTypeName != null)
				{
					requestLogFull.RequestTypeName = tempRequestType.RequestTypeName;
				}

				requestLogFullList.Add(requestLogFull);
			}

			var requestFullEnumerable = requestLogFullList.AsEnumerable();

			// Execute the search term filter
			if (!String.IsNullOrEmpty(search))
			{
				requestFullEnumerable = requestLogFullList.Where(s => s.RequestDescription != null && s.RequestDescription.ToUpper().Contains(search)
																	  || s.ChangeNote != null && s.ChangeNote.ToUpper().Contains(search)
																	  
																	  || s.StatusTypeName != null && s.StatusTypeName.ToUpper().Contains(search)
																	  
																	  || s.RequestTypeName != null && s.RequestTypeName.ToUpper().Contains(search)
																	  
																	  || s.SupplierName != null && s.SupplierName.ToUpper().Contains(search)
																	  
																	  || s.StoreName != null && s.StoreName.ToUpper().Contains(search)
																	  
																	  || s.ProductName != null && s.ProductName.ToUpper().Contains(search)
																	  
																	  || s.UserId != null && s.UserId.ToUpper().Contains(search)

																	  || s.RequestId != null && s.RequestId.ToString().Contains(search)

																	  || s.CreatedBy != null && s.CreatedBy.ToUpper().Contains(search)
																	  
																	  || s.OriginalCreatedByUser != null && s.OriginalCreatedByUser.ToUpper().Contains(search).Equals(null)
																	  
																	  || s.StatusTypeName != null && s.StatusTypeName.ToUpper().Contains(search));
			}

			// Execute sorting columns
			switch (sort)
			{
				case "requestId_desc":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.RequestId != null).OrderByDescending(s => s.RequestId.ToString()));
					break;
				case "RequestId":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.RequestId != null).OrderBy(s => s.RequestId.ToString()));
					break;
				case "requestDescription_desc":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.RequestDescription != null).OrderByDescending(s => s.RequestDescription.ToString()));
					break;
				case "RequestDescription":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.RequestDescription != null).OrderBy(s => s.RequestDescription.ToString()));
					break;
				case "changeNote_desc":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.ChangeNote != null).OrderByDescending(s => s.ChangeNote.ToString()));
					break;
				case "ChangeNote":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.ChangeNote != null).OrderBy(s => s.ChangeNote.ToString()));
					break;
				case "store_desc":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.StoreName != null).OrderByDescending(s => s.StoreName.ToString()));
					break;
				case "StoreName":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.StoreName != null).OrderBy(s => s.StoreName.ToString()));
					break;
				case "statusTypeName_desc":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.StatusTypeName != null).OrderByDescending(s => s.StatusTypeName.ToString()));
					break;
				case "StatusTypeName":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.StatusTypeName != null).OrderBy(s => s.StatusTypeName.ToString()));
					break;
				case "requestType_desc":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.RequestTypeName != null).OrderByDescending(s => s.RequestTypeName.ToString()));
					break;
				case "RequestTypeParam":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.RequestTypeName != null).OrderBy(s => s.RequestTypeName.ToString()));
					break;
				case "productName_desc":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.ProductName != null).OrderByDescending(s => s.ProductName.ToString()));
					break;
				case "ProductNameParam":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.ProductName != null).OrderBy(s => s.ProductName.ToString()));
					break;
				case "supplierName_desc":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.SupplierName != null).OrderByDescending(s => s.SupplierName.ToString()));
					break;
				case "SupplierNameParam":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.SupplierName != null).OrderBy(s => s.SupplierName.ToString()));
					break;
				case "requestDate_desc":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.OriginalCreatedOnDate != null).OrderByDescending(s => s.OriginalCreatedOnDate.ToString()));
					break;
				case "RequestDateParam":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.OriginalCreatedOnDate != null).OrderBy(s => s.OriginalCreatedOnDate.ToString()));
					break;
				case "requesterName_desc":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.OriginalCreatedByUser != null).OrderByDescending(s => s.OriginalCreatedByUser.ToString()));
					break;
				case "RequesterNameParam":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.OriginalCreatedByUser != null).OrderBy(s => s.OriginalCreatedByUser.ToString()));
					break;
				case "createdOn_desc":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.CreatedOn != null).OrderByDescending(s => s.CreatedOn.ToString()));
					break;
				case "CreatedOnParam":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.CreatedOn != null).OrderBy(s => s.CreatedOn.ToString()));
					break;
				case "createdBy_desc":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.CreatedBy != null).OrderByDescending(s => s.CreatedBy.ToString()));
					break;
				case "CreatedByParam":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.CreatedBy != null).OrderBy(s => s.CreatedBy.ToString()));
					break;
				case "requestLogId_desc":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.RequestId != null).OrderByDescending(s => s.Id.ToString()));
					break;
				case "RequestLogIdParam":
					if (requestFullEnumerable != null)
						requestFullEnumerable =
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.RequestId != null).OrderBy(s => int.Parse(s.Id.ToString())));
					break;
				default:
					if (requestFullEnumerable != null)
						requestFullEnumerable = 
							new List<RequestLogFull>(requestFullEnumerable.Where(s => s.Id != null).OrderByDescending(s => int.Parse(s.Id.ToString())));
					break;
			}

			return View(requestFullEnumerable.ToList());
		}
    }
}
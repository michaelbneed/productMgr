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
using PM.Business.Security;
using PM.Entity.Models;
using PM.Entity.Services;
using PM.Entity.ViewModels;

namespace PM.UserAdmin.UI.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class RequestsAdminController : Controller
	{
		private readonly VandivierProductManagerContext _context;
		private readonly IDbReadService _dbReadService;
		private readonly IDbWriteService _dbWriteService;
		private readonly IConfiguration _configuration;

		public RequestsAdminController(IDbReadService dbReadService, IDbWriteService dbWriteService,
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
			if (search != null)
			{
				Regex rgx = new Regex("[^a-zA-Z0-9 -]");
				search = rgx.Replace(search, "").ToUpper();
			}


			// Intercept sort data
			ViewData["RequestIdParam"] = sort == "RequestId" ? "requestId_desc" : "RequestId";
			ViewData["RequestDescriptionParam"] = sort == "RequestDescription" ? "requestDescription_desc" : "RequestDescription";
			ViewData["StoreNameParam"] = sort == "StoreName" ? "store_desc" : "StoreName";
			ViewData["StatusTypeParam"] = sort == "StatusTypeName" ? "statusTypeName_desc" : "StatusTypeName";
			ViewData["RequestTypeParam"] = sort == "RequestTypeParam" ? "requestType_desc" : "RequestTypeParam";
			ViewData["ProductNameParam"] = sort == "ProductNameParam" ? "productName_desc" : "ProductNameParam";
			ViewData["SupplierNameParam"] = sort == "SupplierNameParam" ? "supplierName_desc" : "SupplierNameParam";
			ViewData["RequestDateParam"] = sort == "RequestDateParam" ? "requestDate_desc" : "RequestDateParam";
			ViewData["RequesterNameParam"] = sort == "RequesterNameParam" ? "requesterName_desc" : "RequesterNameParam";
			ViewData["CreatedOnParam"] = sort == "CreatedOnParam" ? "createdOn_desc" : "CreatedOnParam";
			ViewData["CreatedByParam"] = sort == "CreatedByParam" ? "createdBy_desc" : "CreatedByParam";

			// Intercept search term
			ViewData["FilterParam"] = search;


			_dbReadService.IncludeEntityNavigation<Product>();
			_dbReadService.IncludeEntityNavigation<Store>();
			_dbReadService.IncludeEntityNavigation<RequestType>();
			_dbReadService.IncludeEntityNavigation<StatusType>();
			_dbReadService.IncludeEntityNavigation<Supplier>();

			var requests = await _dbReadService.GetAllRecordsAsync<Request>();
			requests.Reverse();

			var requestEnumerable = requests.AsEnumerable();

			// Execute the search term filter
			if (!String.IsNullOrEmpty(search))
			{
				requestEnumerable = requests.Where(s =>
					s.RequestDescription != null && s.RequestDescription.ToUpper().Contains(search)

					|| s.StatusType.StatusTypeName != null && s.StatusType.StatusTypeName.ToUpper().Contains(search)

					|| s.RequestType.RequestTypeName != null && s.RequestType.RequestTypeName.ToUpper().Contains(search)

					|| s.Store.StoreName != null && s.Store.StoreName.ToUpper().Contains(search)

					|| s.Product.ProductName != null && s.Product.ProductName.ToUpper().Contains(search)

					|| s.Supplier.SupplierName != null && s.Supplier.SupplierName.ToUpper().Contains(search)

					|| s.Id.ToString().StartsWith(search)

					|| s.UserId != null && s.UserId.ToUpper().Contains(search));
			}


			switch (sort)
			{
				case "requestId_desc":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.OrderByDescending(s => s.Id.ToString()));
					break;
				case "RequestId":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.OrderBy(s => s.Id.ToString()));
					break;
				case "requestDescription_desc":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.RequestDescription != null).OrderByDescending(s => s.RequestDescription.ToString()));
					break;
				case "RequestDescription":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.RequestDescription != null).OrderBy(s => s.RequestDescription.ToString()));
					break;
				case "store_desc":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.Store.StoreName != null).OrderByDescending(s => s.Store.StoreName.ToString()));
					break;
				case "StoreName":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.Store.StoreName != null).OrderBy(s => s.Store.StoreName.ToString()));
					break;
				case "statusTypeName_desc":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.StatusType.StatusTypeName != null).OrderByDescending(s => s.StatusType.StatusTypeName.ToString()));
					break;
				case "StatusTypeName":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.StatusType.StatusTypeName != null).OrderBy(s => s.StatusType.StatusTypeName.ToString()));
					break;
				case "requestType_desc":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.RequestType.RequestTypeName != null).OrderByDescending(s => s.RequestType.RequestTypeName.ToString()));
					break;
				case "RequestTypeParam":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.RequestType.RequestTypeName != null).OrderBy(s => s.RequestType.RequestTypeName.ToString()));
					break;
				case "productName_desc":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.Product.ProductName != null).OrderByDescending(s => s.Product.ProductName.ToString()));
					break;
				case "ProductNameParam":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.Product.ProductName != null).OrderBy(s => s.Product.ProductName.ToString()));
					break;
				case "supplierName_desc":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.Supplier.SupplierName != null).OrderByDescending(s => s.Supplier.ToString()));
					break;
				case "SupplierNameParam":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.Supplier != null).OrderBy(s => s.Supplier.ToString()));
					break;
				case "requestDate_desc":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.CreatedOn != null).OrderByDescending(s => s.CreatedOn.ToString()));
					break;
				case "RequestDateParam":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.CreatedOn != null).OrderBy(s => s.CreatedOn.ToString()));
					break;
				case "requesterName_desc":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.CreatedBy != null).OrderByDescending(s => s.CreatedBy.ToString()));
					break;
				case "RequesterNameParam":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.CreatedBy != null).OrderBy(s => s.CreatedBy.ToString()));
					break;
				case "createdOn_desc":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.CreatedOn != null).OrderByDescending(s => s.CreatedOn.ToString()));
					break;
				case "CreatedOnParam":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.CreatedOn != null).OrderBy(s => s.CreatedOn.ToString()));
					break;
				case "createdBy_desc":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.CreatedBy != null).OrderByDescending(s => s.CreatedBy.ToString()));
					break;
				case "CreatedByParam":
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.Where(s => s.CreatedBy != null).OrderBy(s => s.CreatedBy.ToString()));
					break;
				default:
					if (requestEnumerable != null)
						requestEnumerable =
							new List<Request>(requestEnumerable.OrderByDescending(s => s.CreatedOn));
					break;
			}

			return View(requestEnumerable);
		}
	}
}

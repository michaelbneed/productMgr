using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PM.Auth.GraphApi;
using PM.Business.Dto;
using PM.Business.Email;
using PM.Business.RequestLogging;
using PM.Business.Security;
using PM.Entity.Models;
using PM.Entity.Services;

namespace PM.UserAdmin.UI.Controllers
{
    public class RequestsController : Controller
    {
        private readonly VandivierProductManagerContext _context;
        private readonly IDbReadService _dbReadService;
        private readonly IDbWriteService _dbWriteService;
        private readonly IConfiguration _configuration;
        
		public RequestsController(IDbReadService dbReadService, IDbWriteService dbWriteService, 
			VandivierProductManagerContext context, IConfiguration configuration)
		{
			_dbReadService = dbReadService;
			_dbWriteService = dbWriteService;
	        _context = context;
	        _configuration = configuration;
		}

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		public async Task<IActionResult> Index(string search)
		{
			if (search != null)
			{
				Regex rgx = new Regex("[^a-zA-Z0-9 -]");
				search = rgx.Replace(search, "").ToUpper();
			}

			ViewData["FilterParam"] = search;

			UserDto.SetUserRole(User.FindFirstValue("groups"), _configuration);
			UserDto.UserId = User.Identity.Name;

			_dbReadService.IncludeEntityNavigation<Product>();
	        _dbReadService.IncludeEntityNavigation<Store>();
			_dbReadService.IncludeEntityNavigation<RequestType>();
	        _dbReadService.IncludeEntityNavigation<StatusType>();
	        _dbReadService.IncludeEntityNavigation<Supplier>();

			var requests = await _dbReadService.GetAllRecordsAsync<Request>();
			requests.Reverse();

			var requestsEnumerable = requests.AsEnumerable();

			if (!String.IsNullOrEmpty(search))
			{
				requestsEnumerable = requests.Where(s => s.RequestDescription != null && s.RequestDescription.ToUpper().Contains(search)
				
														|| s.StatusType.StatusTypeName != null && s.StatusType.StatusTypeName.ToUpper().Contains(search)

														|| s.RequestType.RequestTypeName != null && s.RequestType.RequestTypeName.ToUpper().Contains(search)

														|| s.Store.StoreName != null && s.Store.StoreName.ToUpper().Contains(search)

														|| s.ProductId != null && s.Product.ProductName.ToUpper().Contains(search)

														|| s.Id.ToString().StartsWith(search)

														|| s.Id.ToString().Contains(search)

														|| s.CreatedBy != null && s.CreatedBy.ToUpper().Contains(search));
			}

			RequestDto.RequestId = null;
			RequestDto.RequestDescription = null;

			return View(requestsEnumerable.ToList());
		}

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		public async Task<IActionResult> Details(int? id)
        {
	        UserDto.SetUserRole(User.FindFirstValue("groups"), _configuration);

	        var roleForDebug = UserDto.Role;

			if (!id.HasValue)
            {
                return NotFound();
            }

			_dbReadService.IncludeEntityNavigation<Product>();
			_dbReadService.IncludeEntityNavigation<Store>();
			_dbReadService.IncludeEntityNavigation<RequestType>();
			_dbReadService.IncludeEntityNavigation<StatusType>();
			_dbReadService.IncludeEntityNavigation<Supplier>();

			var request = await _dbReadService.GetSingleRecordAsync<Request>(s => s.Id.Equals(id));

			if (request == null)
            {
                return NotFound();
            }

			RequestDto.RequestId = request.Id;
			RequestDto.RequestDescription = request.RequestDescription;

			var notes = await _dbReadService.GetAllRecordsAsync<Note>(s => s.RequestId.Equals(id));
			if (notes != null)
			{
				ViewData["NoteId"] = notes;
			}
			
			return View(request);
        }

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		public IActionResult CreateRequest()
		{
			ViewData["RequestTypeId"] = new SelectList(_context.RequestType, "Id", "RequestTypeName");
			ViewData["StatusTypeId"] = new SelectList(_context.StatusType, "Id", "StatusTypeName");
			ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName");
			ViewData["StoreId"] = new SelectList(_context.Store, "Id", "StoreSupervisorName");

			return View();
		}

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateRequest([Bind("Id,RequestDescription,RequestTypeId,StatusTypeId,UserId,SupplierId,StoreId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] Request request)
		{
			if (ModelState.IsValid)
			{
				if (User != null)
				{
					string claimTypeEmailAddress = $"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
					var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"name").Value;
					request.UserId = User.Claims.FirstOrDefault(x => x.Type == claimTypeEmailAddress).Value;
					request.CreatedBy = userFullName;
				}

				request.CreatedOn = DateTime.Now;

				var status = await _dbReadService.GetSingleRecordAsync<StatusType>(s => s.StatusTypeName.Equals("New Request"));
				request.StatusTypeId = status.Id;

				_dbWriteService.Add(request);
				await _dbWriteService.SaveChangesAsync();
			}

			RequestDto.RequestId = request.Id;

			return RedirectToAction("CreateProduct", "Products", new { id = request.Id });
		}

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		public async Task<IActionResult> Edit(int? id)
        { 
			if (id == null)
            {
                return NotFound();
            }

            var request = await _dbReadService.GetSingleRecordAsync<Request>(s => s.Id.Equals(id));
            RequestDto.StatusId = request.StatusTypeId;

			if (request == null)
            {
                return NotFound();
            }

			ViewData["ProductId"] = new SelectList(_context.Product, "Id", "ProductName", selectedValue: request.ProductId);
			ViewData["RequestTypeId"] = new SelectList(_context.RequestType, "Id", "RequestTypeName");
			ViewData["StatusTypeId"] = new SelectList(_context.StatusType, "Id", "StatusTypeName");
			ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName");
			ViewData["StoreId"] = new SelectList(_context.Store, "Id", "StoreSupervisorName");

			RequestDto.RequestId = request.Id;
			RequestDto.RequestDescription = request.RequestDescription;

			var note = await _dbReadService.GetSingleRecordAsync<Note>(s => s.RequestId.Equals(id));
			if (note != null)
			{
				ViewData["NoteId"] = note.Id;
			}

			return View(request);
        }

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RequestDescription,RequestTypeId,StatusTypeId,UserId,ProductId,SupplierId,StoreId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] Request request)
        {
	        if (id != request.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
	                if (User != null)
	                {
		                string claimTypeEmailAddress = $"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
		                var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"name").Value;
		                request.UserId = User.Claims.FirstOrDefault(x => x.Type == claimTypeEmailAddress).Value;
		                request.UpdatedBy = userFullName;
	                }

					request.UpdatedOn = DateTime.Now;

					request.ProductId = _dbReadService.GetSingleRecordAsync<Request>(p => p.Id.Equals(request.Id)).Result.ProductId;

					_dbWriteService.Update(request);
					await _dbWriteService.SaveChangesAsync();

					var status = await _dbReadService.GetSingleRecordAsync<StatusType>(s => s.Id.Equals(request.StatusTypeId));
					if (request.StatusTypeId != RequestDto.StatusId)
					{
						switch (status.StatusTypeName)
						{
							case "New Request":
								break;
							case "Approved":
								RequestLogHelper logHelperApproved = new RequestLogHelper();
								logHelperApproved.LogRequestChange(request, _context, RequestLogConstants.RequestApproved);

								RequestEmail requestEmailManager = new RequestEmail(_configuration, _dbReadService);
								requestEmailManager.SendApprovedRequestEmailToHeadQuarters(request);
								break;

							case "Denied":
								RequestLogHelper logHelperDenied = new RequestLogHelper();
								logHelperDenied.LogRequestChange(request, _context, RequestLogConstants.RequestDenied);

								RequestEmail requestEmailOriginator = new RequestEmail(_configuration, _dbReadService);
								requestEmailOriginator.SendDeniedRequestEmailToOriginatingUser(request);

								break;

							case "Complete":
								RequestLogHelper logHelperComplete = new RequestLogHelper();
								logHelperComplete.LogRequestChange(request, _context, RequestLogConstants.RequestComplete);

								RequestEmail requestEmailCompletedStatus = new RequestEmail(_configuration, _dbReadService);
								requestEmailCompletedStatus.SendRequestCompletedToGroup(request);
								break;
						}
					}
                }
				catch (DbUpdateConcurrencyException)
                {
	                bool result = await RequestExists(request.Id);
	                if (!result)
					{
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            RequestDto.StatusId = null;
            return View(request);
        }

        [Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			_dbReadService.IncludeEntityNavigation<Product>();
			_dbReadService.IncludeEntityNavigation<RequestType>();
			_dbReadService.IncludeEntityNavigation<StatusType>();
			_dbReadService.IncludeEntityNavigation<Supplier>();
			_dbReadService.IncludeEntityNavigation<Store>();

			var request = await _dbReadService.GetSingleRecordAsync<Request>(s => s.Id.Equals(id));
			
            if (request == null)
            {
	            return NotFound();
            }

            return View(request);
		}

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _dbReadService.GetSingleRecordAsync<Request>(s => s.Id.Equals(id));

            _dbWriteService.Delete(request);

			var response = await _dbWriteService.SaveChangesAsync();
			if (!response)
			{
				TempData["notifyUser"] = "This action could not be performed due to data constraints.";
			}

			RequestLogHelper logHelper = new RequestLogHelper();
			logHelper.LogRequestChange(request, _context, RequestLogConstants.RequestDeletedByStaff);

			return RedirectToAction(nameof(Index));
        }

		private async Task<bool> RequestExists(int id)
		{
			var request = _dbReadService.GetSingleRecordAsync<Request>(s => s.Id.Equals(id));
			return await _dbReadService.DoesRecordExist<Request>(e => request.Id == id);
		}
    }
}

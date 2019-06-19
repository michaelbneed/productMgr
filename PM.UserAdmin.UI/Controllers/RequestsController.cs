using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PM.Business.Dto;
using PM.Entity.Models;
using PM.Entity.Services;
using PM.UserAdmin.UI.Security;

namespace PM.UserAdmin.UI.Controllers
{
    public class RequestsController : Controller
    {
        private readonly VandivierProductManagerContext _context;
        private readonly IDbReadService _dbReadService;
        private readonly IDbWriteService _dbWriteService;

		public RequestsController(IDbReadService dbReadService, IDbWriteService dbWriteService, VandivierProductManagerContext context)
		{
			_dbReadService = dbReadService;
			_dbWriteService = dbWriteService;
	        _context = context;
        }

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		public async Task<IActionResult> Index()
        {
	        _dbReadService.IncludeEntityNavigation<RequestType>();
	        _dbReadService.IncludeEntityNavigation<StatusType>();
	        _dbReadService.IncludeEntityNavigation<Supplier>();

			var requests = await _dbReadService.GetAllRecordsAsync<Request>();
			requests.Reverse();

			RequestDto.RequestId = null;
			RequestDto.RequestDescription = null;

			return View(requests);
		}

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			_dbReadService.IncludeEntityNavigation<Product>();
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

			return View(request);
        }

		public IActionResult CreateRequest()
		{
			ViewData["RequestTypeId"] = new SelectList(_context.RequestType, "Id", "RequestTypeName");
			ViewData["StatusTypeId"] = new SelectList(_context.StatusType, "Id", "StatusTypeName");
			ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateRequest([Bind("Id,RequestDescription,RequestTypeId,StatusTypeId,UserId,ProductId,SupplierId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] Request request)
		{
			if (ModelState.IsValid)
			{
				if (User != null)
				{
					var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"name").Value;
					request.CreatedBy = userFullName;
				}

				request.CreatedOn = DateTime.Now;

				_dbWriteService.Add(request);

				await _dbWriteService.SaveChangesAsync();
			}

			ViewData["RequestTypeId"] = new SelectList(_context.RequestType, "Id", "RequestTypeName", request.RequestTypeId);
			ViewData["StatusTypeId"] = new SelectList(_context.StatusType, "Id", "StatusTypeName", request.StatusTypeId);
			ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName", request.SupplierId);

			return RedirectToAction("CreateProduct", "Products", new { id = request.Id });
		}

		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _dbReadService.GetSingleRecordAsync<Request>(s => s.Id.Equals(id));

            if (request == null)
            {
                return NotFound();
            }
			ViewData["ProductId"] = new SelectList(_context.Product, "Id", "ProductName", request.ProductId);
			ViewData["RequestTypeId"] = new SelectList(_context.RequestType, "Id", "RequestTypeName", request.RequestTypeId);
			ViewData["StatusTypeId"] = new SelectList(_context.StatusType, "Id", "StatusTypeName", request.StatusTypeId);
			ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName", request.SupplierId);

			RequestDto.RequestId = request.Id;
			RequestDto.RequestDescription = request.RequestDescription;

			return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RequestDescription,RequestTypeId,StatusTypeId,UserId,ProductId,SupplierId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] Request request)
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
						var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"name").Value;
						request.UpdatedBy = userFullName;
					}

					request.UpdatedOn = DateTime.Now;

					_dbWriteService.Update(request);

                    await _dbWriteService.SaveChangesAsync();
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
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "ProductName", request.ProductId);
            ViewData["RequestTypeId"] = new SelectList(_context.RequestType, "Id", "RequestTypeName", request.RequestTypeId);
            ViewData["StatusTypeId"] = new SelectList(_context.StatusType, "Id", "StatusTypeName", request.StatusTypeId);
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName", request.SupplierId);
            return View(request);
        }

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

			var request = await _dbReadService.GetSingleRecordAsync<Request>(s => s.Id.Equals(id));

            if (request == null)
            {
	            return NotFound();
            }

            return View(request);
		}

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

			return RedirectToAction(nameof(Index));
        }

		private async Task<bool> RequestExists(int id)
		{
			var request = _dbReadService.GetSingleRecordAsync<Request>(s => s.Id.Equals(id));
			return await _dbReadService.DoesRecordExist<Request>(e => request.Id == id);
		}
    }
}

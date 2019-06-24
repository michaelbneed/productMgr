using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PM.Business.Dto;
using PM.Business.Security;
using PM.Entity.Models;
using PM.Entity.Services;

namespace PM.Vendor.UI.Controllers
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

		[Authorize]
		public async Task<IActionResult> Index()
        {
			var userData = await _dbReadService.GetAllRecordsAsync<User>();
			var loggedInUser = userData.FirstOrDefault();
			var loggedInSupplierId = loggedInUser.SupplierId;
			var userClaim = User.Claims;
			var b2cUser = userClaim.ToList();

			// TODO Filter by user!!!

			_dbReadService.IncludeEntityNavigation<Product>();
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

			var note = await _dbReadService.GetSingleRecordAsync<Note>(s => s.RequestId.Equals(id));

			if (note != null)
			{
				ViewData["NoteId"] = note.Id;
			}
			
			return View(request);
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

			var note = await _dbReadService.GetSingleRecordAsync<Note>(s => s.RequestId.Equals(id));

			if (note != null)
			{
				ViewData["NoteId"] = note.Id;
			}

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
						var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"emails").Value;
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

		private async Task<bool> RequestExists(int id)
		{
			var request = _dbReadService.GetSingleRecordAsync<Request>(s => s.Id.Equals(id));
			return await _dbReadService.DoesRecordExist<Request>(e => request.Id == id);
		}
    }
}

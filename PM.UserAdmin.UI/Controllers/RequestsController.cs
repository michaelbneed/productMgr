﻿using System;
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

namespace PM.UserAdmin.UI.Controllers
{
    public class RequestsController : Controller
    {
        private readonly VandivierProductManagerContext _context;
        private readonly IDbReadService _dbReadService;
        private readonly IDbWriteService _dbWriteService;

        private int? requestStatusBeforeEdit;

		public RequestsController(IDbReadService dbReadService, IDbWriteService dbWriteService, VandivierProductManagerContext context)
		{
			_dbReadService = dbReadService;
			_dbWriteService = dbWriteService;
	        _context = context;
        }

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		public async Task<IActionResult> Index()
        {
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

			var notes = await _dbReadService.GetAllRecordsAsync<Note>(s => s.RequestId.Equals(id));
			if (notes != null)
			{
				ViewData["NoteId"] = notes;
			}
			
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

			ViewData["RequestTypeId"] = new SelectList(_context.RequestType, "Id", "RequestTypeName", request.RequestTypeId);
			ViewData["StatusTypeId"] = new SelectList(_context.StatusType, "Id", "StatusTypeName", request.StatusTypeId).SelectedValue;
			ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName", request.SupplierId);

			RequestDto.RequestId = request.Id;

			return RedirectToAction("CreateProduct", "Products", new { id = request.Id });
		}

		public async Task<IActionResult> Edit(int? id)
        { 
			if (id == null)
            {
                return NotFound();
            }

            var request = await _dbReadService.GetSingleRecordAsync<Request>(s => s.Id.Equals(id));
            requestStatusBeforeEdit = request.StatusTypeId;

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
						var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"name").Value;
						request.UpdatedBy = userFullName;
					}

					request.UpdatedOn = DateTime.Now;

					_dbWriteService.Update(request);
					await _dbWriteService.SaveChangesAsync();

					//TODO Send email on completed status
					//if (request.StatusTypeId != requestStatusBeforeEdit && request.StatusTypeId == 5)
					//{

					//}
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

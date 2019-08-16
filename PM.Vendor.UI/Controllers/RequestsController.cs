﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI.Internal;
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
			_dbReadService.IncludeEntityNavigation<Product>();
			_dbReadService.IncludeEntityNavigation<Store>();
			_dbReadService.IncludeEntityNavigation<RequestType>();
			_dbReadService.IncludeEntityNavigation<StatusType>();
			_dbReadService.IncludeEntityNavigation<Supplier>();

			// Restrict by SupplierId
			var b2CUserAuthId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
			var userToEnsure = await _dbReadService.GetSingleRecordAsync<User>(s => s.AuthId.Equals(b2CUserAuthId));
			
			List<Request> requests = new List<Request>();

			if (userToEnsure != null)
			{
				RequestDto.SupplierId = userToEnsure.SupplierId;
			}
			
			if (RequestDto.SupplierId != 0)
	        {
		        ViewData["SupplierData"] = RequestDto.SupplierId;
				requests = await _dbReadService.GetAllRecordsAsync<Request>(s => s.SupplierId.Equals(RequestDto.SupplierId));
		        requests.Reverse();
			}
			else
			{
				TempData["notifyUser"] = "User not assigned to a supplier!";
				ViewData["SupplierData"] = null;
			}

			RequestDto.RequestId = null;

			RequestDto.RequestDescription = null;
			return View(requests);
		}

		[Authorize]
		public async Task<IActionResult> Details(int? id)
        {
	        if (id == null)
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

			var note = await _dbReadService.GetSingleRecordAsync<Note>(s => s.RequestId.Equals(id));

			if (note != null)
			{
				ViewData["NoteId"] = note.Id;
			}
			
			return View(request);
        }

		[Authorize]
		public IActionResult CreateRequest()
        {
			ViewData["RequestTypeId"] = new SelectList(_context.RequestType, "Id", "RequestTypeName");
			ViewData["StatusTypeId"] = new SelectList(_context.StatusType, "Id", "StatusTypeName");
			ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName");
			ViewData["StoreId"] = new SelectList(_context.Store, "Id", "StoreSupervisorName");

			return View();
        }

		[Authorize]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequest([Bind("Id,RequestDescription,RequestTypeId,StatusTypeId,UserId,SupplierId,StoreId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] Request request)
        {
	        if (ModelState.IsValid)
	        {
		        if (User != null)
		        {
			        var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"emails").Value;
			        request.CreatedBy = userFullName;
		        }

		        request.CreatedOn = DateTime.Now;

		        var status = await _dbReadService.GetSingleRecordAsync<StatusType>(s => s.StatusTypeName.Equals("New Request"));
		        request.StatusTypeId = status.Id;
		        request.SupplierId = RequestDto.SupplierId;

				_dbWriteService.Add(request);
		        await _dbWriteService.SaveChangesAsync();
	        }

	        ViewData["RequestTypeId"] = new SelectList(_context.RequestType, "Id", "RequestTypeName", request.RequestTypeId);
	        ViewData["StatusTypeId"] = new SelectList(_context.StatusType, "Id", "StatusTypeName", request.StatusTypeId).SelectedValue;
			
			RequestDto.RequestId = request.Id;
			
	        return RedirectToAction("CreateProduct", "Products", new { id = request.Id });
        }

        [Authorize]
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

		[Authorize]
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
						var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"emails").Value;
						request.UpdatedBy = userFullName;
					}

					request.UpdatedOn = DateTime.Now;
					request.SupplierId = RequestDto.SupplierId;

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

            return View(request);
        }

		private async Task<bool> RequestExists(int id)
		{
			var request = _dbReadService.GetSingleRecordAsync<Request>(s => s.Id.Equals(id));
			return await _dbReadService.DoesRecordExist<Request>(e => request.Id == id);
		}
    }
}

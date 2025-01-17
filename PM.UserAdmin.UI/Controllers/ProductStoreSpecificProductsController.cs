﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PM.Business.Dto;
using PM.Business.Email;
using PM.Business.Security;
using PM.Entity.Models;
using PM.Entity.Services;

namespace PM.UserAdmin.UI.Controllers
{
    public class ProductStoreSpecificProductsController : Controller
    {
        private readonly VandivierProductManagerContext _context;
        private readonly IDbReadService _dbReadService;
        private readonly IDbWriteService _dbWriteService;
        private readonly IConfiguration _configuration;

		public ProductStoreSpecificProductsController(VandivierProductManagerContext context, 
							IDbReadService dbReadService, IDbWriteService dbWriteService, IConfiguration configuration)
        {
            _context = context;
            _dbReadService = dbReadService;
            _dbWriteService = dbWriteService;
            _configuration = configuration;
		}

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		public async Task<IActionResult> Index(int? id)
        {
	        ViewData["ProductId"] = id;

	        var product = await _dbReadService.GetSingleRecordAsync<Product>(s => s.Id.Equals(id));
	        ViewData["ProductName"] = product.ProductName;
	        if (product.ProductPrice != null) ViewData["ProductPrice"] = Math.Round((decimal) product.ProductPrice, 2);

	        _dbReadService.IncludeEntityNavigation<ProductStoreSpecific, Store>();
			var productStoreSpecific = await _dbReadService.GetAllRecordsAsync<ProductStoreSpecific>(s => s.ProductId.Equals(id));
			
			productStoreSpecific.Reverse();

			return View(productStoreSpecific);
		}

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _dbReadService.GetSingleRecordAsync<Note>(s => s.RequestId.Equals(RequestDto.RequestId));
            if (note != null)
            {
	            ViewData["NoteId"] = note.Id;
            }

            _dbReadService.IncludeEntityNavigation<ProductStoreSpecific, Store>();
			var productStoreSpecific = await _dbReadService.GetSingleRecordAsync<ProductStoreSpecific>(p => p.Id.Equals(id));
                
            if (productStoreSpecific == null)
            {
                return NotFound();
            }

            var product = _dbReadService.GetSingleRecordAsync<Product>(s => s.Id.Equals(productStoreSpecific.ProductId)).Result;
            ViewData["ProductName"] = product.ProductName;
            if (product.ProductPrice != null) ViewData["ProductPrice"] = Math.Round((decimal)product.ProductPrice, 2);
			
			return View(productStoreSpecific);
        }

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		public IActionResult Create(int? id)
        {
            ViewData["ProductId"] = id;
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "StoreName");

			var product = _dbReadService.GetSingleRecordAsync<Product>(s => s.Id.Equals(id)).Result;
            ViewData["ProductName"] = product.ProductName;
            if (product.ProductPrice != null) ViewData["ProductPrice"] = Math.Round((decimal)product.ProductPrice, 2);

			return View();
        }

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? id, [Bind("Id,ProductId,PackageTypeId,StoreId,StorePrice,StoreCost,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] ProductStoreSpecific productStoreSpecific)
        {
	        productStoreSpecific.Id = 0;
            if (ModelState.IsValid)
            {
				if (User != null)
				{
					var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"name").Value;
					productStoreSpecific.CreatedBy = userFullName;
				}

				productStoreSpecific.CreatedOn = DateTime.Now;

				productStoreSpecific.ProductId = id;
				_dbWriteService.Add(productStoreSpecific);
                await _dbWriteService.SaveChangesAsync();
                var request = await _dbReadService.GetSingleRecordAsync<Request>(s => s.Id.Equals(RequestDto.RequestId));

                RequestEmail requestEmail = new RequestEmail(_configuration, _dbReadService);
                requestEmail.SendNewRequestToHeadQuarters(request);

				return RedirectToAction("Index", "ProductStoreSpecificProducts", new { id = productStoreSpecific.ProductId});
            }
            
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "ProductName", productStoreSpecific.ProductId);

            return View(productStoreSpecific);
        }

        [Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _dbReadService.IncludeEntityNavigation<ProductStoreSpecific, Store>();
			var productStoreSpecific = await _dbReadService.GetSingleRecordAsync<ProductStoreSpecific>(p => p.Id.Equals(id));
			if (productStoreSpecific == null)
            {
                return NotFound();
            }
            
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "ProductName", productStoreSpecific.ProductId);
			ViewData["StoreId"] = new SelectList(_context.Store, "Id", "StoreName");

			var product = _dbReadService.GetSingleRecordAsync<Product>(s => s.Id.Equals(productStoreSpecific.ProductId)).Result;
            ViewData["ProductName"] = product.ProductName;
            if (product.ProductPrice != null) ViewData["ProductPrice"] = Math.Round((decimal)product.ProductPrice, 2);

			var note = await _dbReadService.GetSingleRecordAsync<Note>(s => s.RequestId.Equals(RequestDto.RequestId));
            if (note != null)
            {
	            ViewData["NoteId"] = note.Id;
            }

			return View(productStoreSpecific);
        }

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,PackageTypeId,StoreId,StorePrice,StoreCost,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] ProductStoreSpecific productStoreSpecific)
        {
            if (id != productStoreSpecific.Id)
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
		                productStoreSpecific.UpdatedBy = userFullName;
	                }

	                productStoreSpecific.UpdatedOn = DateTime.Now;
					_dbWriteService.Update(productStoreSpecific);
                    await _dbWriteService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
					bool result = await ProductStoreSpecificExists(productStoreSpecific.Id);
					if (!result)
					{
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
				return RedirectToAction("Index", "ProductStoreSpecificProducts", new { id = productStoreSpecific.ProductId });
			}
            
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "ProductName", productStoreSpecific.ProductId);

            return View(productStoreSpecific);
        }

        [Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _dbReadService.IncludeEntityNavigation<ProductStoreSpecific, Store>();
			var productStoreSpecific = await _dbReadService.GetSingleRecordAsync<ProductStoreSpecific>(p => p.Id.Equals(id));
			if (productStoreSpecific == null)
            {
                return NotFound();
            }

			var product = _dbReadService.GetSingleRecordAsync<Product>(s => s.Id.Equals(productStoreSpecific.ProductId)).Result;
			ViewData["ProductName"] = product.ProductName;
			if (product.ProductPrice != null) ViewData["ProductPrice"] = Math.Round((decimal)product.ProductPrice, 2);


			return View(productStoreSpecific);
        }

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productStoreSpecific = await _dbReadService.GetSingleRecordAsync<ProductStoreSpecific>(s => s.Id.Equals(id));
            _dbWriteService.Delete(productStoreSpecific);
            await _dbWriteService.SaveChangesAsync();

            return RedirectToAction("Index", "ProductStoreSpecificProducts", new { id = productStoreSpecific.ProductId });
		}

		private async Task<bool> ProductStoreSpecificExists(int id)
		{
			var productStoreSpecific = _dbReadService.GetSingleRecordAsync<ProductStoreSpecific>(s => s.Id.Equals(id));
			return await _dbReadService.DoesRecordExist<ProductStoreSpecific>(e => productStoreSpecific.Id == id);
		}
	}
}

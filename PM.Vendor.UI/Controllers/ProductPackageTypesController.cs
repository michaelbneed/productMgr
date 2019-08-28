using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PM.Business.Dto;
using PM.Entity.Models;
using PM.Entity.Services;

namespace PM.Vendor.UI.Controllers
{
    public class ProductPackageTypesController : Controller
    {
        private readonly VandivierProductManagerContext _context;
        private readonly IDbReadService _dbReadService;
        private readonly IDbWriteService _dbWriteService;
        
        public ProductPackageTypesController(VandivierProductManagerContext context, IDbReadService dbReadService, IDbWriteService dbWriteService)
        {
            _context = context;
            _dbReadService = dbReadService;
            _dbWriteService = dbWriteService;
		}

        [Authorize]
		public async Task<IActionResult> Index(int? id)
        {
	        ViewData["ProductId"] = id;
			_dbReadService.IncludeEntityNavigation<Supplier>();
			_dbReadService.IncludeEntityNavigation<Product>();

			var product = await _dbReadService.GetSingleRecordAsync<Product>(s => s.Id.Equals(id));

			ViewData["ProductName"] = product.ProductName;

            var packages = await _dbReadService.GetAllRecordsAsync<ProductPackageType>(s => s.ProductId.Equals(id));
			packages.Reverse();

            return View(packages);
        }

		[Authorize]
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _dbReadService.IncludeEntityNavigation<Product>();
			_dbReadService.IncludeEntityNavigation<Supplier>();
			var productPackageType = await _dbReadService.GetSingleRecordAsync<ProductPackageType>(p => p.Id.Equals(id));

			if (productPackageType == null)
            {
                return NotFound();
            }

			var note = await _dbReadService.GetSingleRecordAsync<Note>(s => s.RequestId.Equals(RequestDto.RequestId));

			if (note != null)
			{
				ViewData["NoteId"] = note.Id;
			}

			var product = _dbReadService.GetSingleRecordAsync<Product>(p => p.Id.Equals(productPackageType.ProductId)).Result;
			if (product.ProductName != null) ViewData["ProductName"] = product.ProductName;
			if (product.ProductPrice == null)
			{
				ViewData["ProductPrice"] = "No retail price has been entered yet";
			}
			else
			{
				ViewData["ProductPrice"] = "Retail Price: " + Math.Round((decimal)product.ProductPrice, 2);
			}
			

			return View(productPackageType);
        }

		[Authorize]
		public IActionResult Create(int? id)
        {
	        var product = _dbReadService.GetSingleRecordAsync<Product>(p => p.Id.Equals(id)).Result;
	        if (product.ProductName != null) ViewData["ProductName"] = product.ProductName;
	        if (product.ProductPrice != null) ViewData["ProductPrice"] = Math.Round((decimal)product.ProductPrice, 2);

			ViewData["ProductId"] = id;
			ViewData["SupplierId"] = RequestDto.SupplierId;
            return View();
        }

		[Authorize]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, bool multiple, [Bind("Id,Quantity,Unit,AlternateProductName,AlternateProductUpccode,SupplierData,SupplierId,AlternateProductPrice,AlternateProductCost,AlternateSuggestedPrice,ProductId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] ProductPackageType productPackageType)
        {
	        productPackageType.Id = 0;
            if (ModelState.IsValid)
            {
	            if (User != null)
	            {
		            var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"emails").Value;
		            productPackageType.CreatedBy = userFullName;
	            }

				productPackageType.SupplierId = RequestDto.SupplierId;
				productPackageType.CreatedOn = DateTime.Now;

	            productPackageType.ProductId = id;
	            _dbWriteService.Add(productPackageType);

                await _dbWriteService.SaveChangesAsync();
            }

            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "ProductName", productPackageType.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName", productPackageType.SupplierId);
			
            if (multiple == true)
            {
	            TempData["notifyUserSuccess"] = "Alternate Package Saved.";
	            return RedirectToAction("Create", "ProductPackageTypes", new { id = productPackageType.ProductId });
            }

			return RedirectToAction("Index", "ProductPackageTypes", new { id = productPackageType.ProductId });
		}

        [Authorize]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productPackageType = await _dbReadService.GetSingleRecordAsync<ProductPackageType>(p => p.Id.Equals(id));

			if (productPackageType == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "ProductName", productPackageType.ProductId);
            ViewData["SupplierId"] = RequestDto.SupplierId;

			var unitPrice = Math.Round((double)Convert.ToDouble(productPackageType.AlternateProductCost) / Convert.ToDouble(productPackageType.Unit), 2);
            ViewData["UnitCost"] = unitPrice.ToString(CultureInfo.InvariantCulture);

			var product = _dbReadService.GetSingleRecordAsync<Product>(p => p.Id.Equals(productPackageType.ProductId)).Result;
            if (product.ProductPrice == null)
            {
	            ViewData["ProductPrice"] = "No retail price has been entered yet";
            }
            else
            {
	            ViewData["ProductPrice"] = "Retail Price: " + Math.Round((decimal)product.ProductPrice, 2);
            }

			var note = await _dbReadService.GetSingleRecordAsync<Note>(s => s.RequestId.Equals(RequestDto.RequestId));

            if (note != null)
            {
	            ViewData["NoteId"] = note.Id;
            }

			return View(productPackageType);
        }

		[Authorize]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Quantity,Unit,AlternateProductName,AlternateProductUpccode,SupplierData,SupplierId,AlternateProductPrice,AlternateProductCost,AlternateSuggestedPrice,ProductId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] ProductPackageType productPackageType)
        {
            if (id != productPackageType.Id)
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
		                productPackageType.UpdatedBy = userFullName;
	                }

					productPackageType.SupplierId = RequestDto.SupplierId;
					productPackageType.UpdatedOn = DateTime.Now;

	                _dbWriteService.Update(productPackageType);
                    await _dbWriteService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
	                bool result = await ProductPackageTypeExists(productPackageType.Id);
	                if (!result)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            
			return RedirectToAction("Index", "ProductPackageTypes", new { id = productPackageType.ProductId });
		}

        [Authorize]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _dbReadService.IncludeEntityNavigation<Product>();
			_dbReadService.IncludeEntityNavigation<Supplier>();
			var productPackageType = await _dbReadService.GetSingleRecordAsync<ProductPackageType>(p => p.Id.Equals(id));

			if (productPackageType == null)
            {
                return NotFound();
            }

			var product = _dbReadService.GetSingleRecordAsync<Product>(p => p.Id.Equals(productPackageType.ProductId)).Result;
			if (product.ProductName != null) ViewData["ProductName"] = product.ProductName;
			if (product.ProductPrice == null)
			{
				ViewData["ProductPrice"] = "No retail price has been entered yet";
			}
			else
			{
				ViewData["ProductPrice"] = "Retail Price: " + Math.Round((decimal)product.ProductPrice, 2);
			}
			return View(productPackageType);
        }

		[Authorize]
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productPackageType = await _dbReadService.GetSingleRecordAsync<ProductPackageType>(s => s.Id.Equals(id));

			_dbWriteService.Delete(productPackageType);

            await _context.SaveChangesAsync();

			return RedirectToAction("Index", "ProductPackageTypes", new { id = productPackageType.ProductId });
		}

		private async Task<bool> ProductPackageTypeExists(int id)
		{
			var request = _dbReadService.GetSingleRecordAsync<Request>(s => s.Id.Equals(id));
			return await _dbReadService.DoesRecordExist<Request>(e => request.Id == id);
		}
	}
}

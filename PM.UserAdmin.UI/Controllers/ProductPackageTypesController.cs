using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PM.Business.Dto;
using PM.Entity.Models;
using PM.Entity.Services;

namespace PM.UserAdmin.UI.Controllers
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

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _dbReadService.IncludeEntityNavigation<Supplier>();
			var productPackageType = await _dbReadService.GetSingleRecordAsync<ProductPackageType>(p => p.Id.Equals(id));

			if (productPackageType == null)
            {
                return NotFound();
            }

            return View(productPackageType);
        }

        public IActionResult Create(int? id)
        {
	        ViewData["ProductId"] = id;
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("Id,Quantity,Unit,AlternateProductName,AlternateProductUpcCode,SupplierData,SupplierId,AlternateProductPrice,AlternateProductCost,ProductId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] ProductPackageType productPackageType)
        {
	        productPackageType.Id = 0;
            if (ModelState.IsValid)
            {
	            if (User != null)
	            {
		            var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"name").Value;
		            productPackageType.CreatedBy = userFullName;
	            }

	            productPackageType.CreatedOn = DateTime.Now;

	            productPackageType.ProductId = id;
	            _dbWriteService.Add(productPackageType);

                await _dbWriteService.SaveChangesAsync();
            }

            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "ProductName", productPackageType.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName", productPackageType.SupplierId);

            return RedirectToAction("Details", "Requests", new { id = RequestDto.RequestId });
		}

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
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName", productPackageType.SupplierId);

            return View(productPackageType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Quantity,Unit,AlternateProductName,AlternateProductUpcCode,SupplierData,SupplierId,AlternateProductPrice,AlternateProductCost,ProductId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] ProductPackageType productPackageType)
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
		                var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"name").Value;
		                productPackageType.UpdatedBy = userFullName;
	                }

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
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "ProductName", productPackageType.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName", productPackageType.SupplierId);

			return RedirectToAction("Index", "ProductPackageTypes", new { id = productPackageType.ProductId });
		}

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _dbReadService.IncludeEntityNavigation<Supplier>();
			var productPackageType = await _dbReadService.GetSingleRecordAsync<ProductPackageType>(p => p.Id.Equals(id));

			if (productPackageType == null)
            {
                return NotFound();
            }

            return View(productPackageType);
        }

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

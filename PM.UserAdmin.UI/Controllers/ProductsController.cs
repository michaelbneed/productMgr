using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PM.Entity.Models;
using PM.Entity.Services;

namespace PM.UserAdmin.UI.Controllers
{
    public class ProductsController : Controller
    {
        private readonly VandivierProductManagerContext _context;
        private readonly IDbReadService _dbReadService;
        private readonly IDbWriteService _dbWriteService;

        public ProductsController(IDbReadService dbReadService, IDbWriteService dbWriteService, VandivierProductManagerContext context)
        {
            _context = context;
            _dbReadService = dbReadService;
            _dbWriteService = dbWriteService;
		}

        [Authorize]
        public async Task<IActionResult> Index()
        {
			_dbReadService.IncludeEntityNavigation<Category>();
			var products = await _dbReadService.GetAllRecordsAsync<Product>();

			products.Reverse();

			return View(products);
		}

        public IActionResult CreateProduct(int? id)
        {
	        ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName");
	        return View();
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(int id, [Bind("Id,ProductName,ProductDescription,Upccode,ProductLocation,ProductCost,ProductPrice,PackageSize,PackageType,OrderWeek,CategoryId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] Product product)
        {
	        var requestId = id;
	        product.Id = 0;

	        if (ModelState.IsValid)
	        {
		        if (User != null)
		        {
			        var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"name").Value;
			        product.CreatedBy = userFullName;
		        }

		        product.CreatedOn = DateTime.Now;

		        _dbWriteService.Add(product);
		        await _dbWriteService.SaveChangesAsync();

		        _dbReadService.IncludeEntityNavigation<Request>();
		        var request = await _dbReadService.GetSingleRecordAsync<Request>(s => s.Id.Equals(requestId));

		        request.ProductId = product.Id;
		        _dbWriteService.Update(request);
		        await _dbWriteService.SaveChangesAsync();
	        }
	        ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", product.CategoryId);

	        return RedirectToAction("Details", "Requests", new { id = requestId });
        }

		public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			_dbReadService.IncludeEntityNavigation<Category>();
			var product = await _dbReadService.GetSingleRecordAsync<Product>(s => s.Id.Equals(id));

			if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			var product = await _dbReadService.GetSingleRecordAsync<Product>(s => s.Id.Equals(id));

			if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,ProductDescription,Upccode,ProductLocation,ProductCost,ProductPrice,PackageSize,PackageType,OrderWeek,CategoryId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] Product product)
        {
            if (id != product.Id)
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
						product.UpdatedBy = userFullName;
					}

					product.UpdatedOn = DateTime.Now;

					_dbWriteService.Update(product);
                    await _dbWriteService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
					bool result = await ProductExists(product.Id);
					if (!result)
					{
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Products", new { id = product.Id });
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", product.CategoryId);
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _dbReadService.IncludeEntityNavigation<Category>();
            var product = await _dbReadService.GetSingleRecordAsync<Product>(s => s.Id.Equals(id));

			if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _dbReadService.GetSingleRecordAsync<Supplier>(s => s.Id.Equals(id));

            _dbWriteService.Delete(product);

			var response = await _dbWriteService.SaveChangesAsync();

			if (!response)
			{
				TempData["notifyUser"] = "This action could not be performed due to data constraints.";
			}

			return RedirectToAction(nameof(Index));
        }

		private async Task<bool> ProductExists(int id)
		{
			var product = _dbReadService.GetSingleRecordAsync<Product>(s => s.Id.Equals(id));
			return await _dbReadService.DoesRecordExist<Product>(e => product.Id == id);
		}
    }
}
using System;
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
using PM.Entity.Models;
using PM.Entity.Services;

namespace PM.Vendor.UI.Controllers
{
    public class ProductsController : Controller
    {
        private readonly VandivierProductManagerContext _context;
        private readonly IDbReadService _dbReadService;
        private readonly IDbWriteService _dbWriteService;
        private readonly IConfiguration _configuration;


		public ProductsController(IDbReadService dbReadService, IDbWriteService dbWriteService, 
	        VandivierProductManagerContext context, IConfiguration configuration)
        {
            _context = context;
            _dbReadService = dbReadService;
            _dbWriteService = dbWriteService;
            _configuration = configuration;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
			_dbReadService.IncludeEntityNavigation<Category>();
			var products = await _dbReadService.GetAllRecordsAsync<Product>();

			products.Reverse();

			return View(products);
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

			var note = await _dbReadService.GetSingleRecordAsync<Note>(s => s.RequestId.Equals(RequestDto.RequestId));

			if (note != null)
			{
				ViewData["NoteId"] = note.Id;
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

            var note = await _dbReadService.GetSingleRecordAsync<Note>(s => s.RequestId.Equals(RequestDto.RequestId));

            if (note != null)
            {
	            ViewData["NoteId"] = note.Id;
            }

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
						var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"emails").Value;
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
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", product.CategoryId);

            return RedirectToAction("Details", "Products", new { id = product.Id });
		}

        private async Task<bool> ProductExists(int id)
		{
			var product = _dbReadService.GetSingleRecordAsync<Product>(s => s.Id.Equals(id));
			return await _dbReadService.DoesRecordExist<Product>(e => product.Id == id);
		}
    }
}
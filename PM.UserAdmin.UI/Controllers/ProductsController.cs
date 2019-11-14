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
using PM.Business.RequestLogging;
using PM.Business.Security;
using PM.Entity.Models;
using PM.Entity.Services;

namespace PM.UserAdmin.UI.Controllers
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

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		public async Task<IActionResult> Index()
        {
			_dbReadService.IncludeEntityNavigation<Category>();

			_dbReadService.IncludeEntityNavigation<Product, ContainerSizeType>();
			_dbReadService.IncludeEntityNavigation<Product, ContainerType>();
			var products = await _dbReadService.GetAllRecordsAsync<Product>();
			products.Reverse();

			return View(products);
		}

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		public IActionResult CreateProduct(int? id)
        {
	        ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName");
	        ViewData["ContainerTypeId"] = new SelectList(_context.ContainerType, "Id", "ContainerTypeName");
	        ViewData["ContainerSizeTypeId"] = new SelectList(_context.ContainerSizeType, "Id", "ContainerSizeTypeName");
			return View();
        }

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(int id, [Bind("Id,ProductName,ProductDescription,Upccode,ProductLocation,ProductCost,ProductPrice,SuggestedPrice,PackageSize,ContainerSizeTypeId,ContainerTypeId,OrderWeek,CategoryId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy,UnitsPerCase")] Product product)
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

		        RequestEmail requestEmail = new RequestEmail(_configuration, _dbReadService);
				requestEmail.SendRequestToStoreManager(request);

				RequestLogHelper logHelper = new RequestLogHelper();
				logHelper.LogRequestChange(request, _context, RequestLogConstants.RequestAddByStaff);
	        }
	        ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", product.CategoryId);

	        return RedirectToAction("Details", "Requests", new { id = requestId });
        }

        [Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProductAndPackage(int id, [Bind("Id,ProductName,ProductDescription,Upccode,ProductLocation,ProductCost,ProductPrice,SuggestedPrice,PackageSize,PackageType,ContainerSizeTypeId,ContainerTypeId,OrderWeek,CategoryId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy,UnitsPerCase")] Product product)
        {
	        var requestId = RequestDto.RequestId;
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

		        RequestEmail requestEmail = new RequestEmail(_configuration, _dbReadService);
		        requestEmail.SendRequestToStoreManager(request);

				RequestLogHelper logHelper = new RequestLogHelper();
				logHelper.LogRequestChange(request, _context, RequestLogConstants.ProductAndPackageAddByStaff);
			}

	        ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", product.CategoryId);
	        ViewData["ContainerTypeId"] = new SelectList(_context.ContainerType, "Id", "ContainerTypeName");
	        ViewData["ContainerSizeTypeId"] = new SelectList(_context.ContainerSizeType, "Id", "ContainerSizeTypeName");

			return RedirectToAction("Create", "ProductPackageTypes", new { id = product.Id });
		}

        [Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _dbReadService.IncludeEntityNavigation<Product, ContainerSizeType>();
            _dbReadService.IncludeEntityNavigation<Product, ContainerType>();
			_dbReadService.IncludeEntityNavigation<Product, Category>();
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

			var productStoreSpecific = await _dbReadService.GetAllRecordsAsync<ProductStoreSpecific>(p => p.ProductId.Equals(id));
			if (productStoreSpecific != null)
			{
				var productStoreSpecificCount = productStoreSpecific.Count();
				ViewData["ProductStoreSpecificCount"] = productStoreSpecificCount;
			}

			return View(product);
        }

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
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
            ViewData["ContainerTypeId"] = new SelectList(_context.ContainerType, "Id", "ContainerTypeName");
            ViewData["ContainerSizeTypeId"] = new SelectList(_context.ContainerSizeType, "Id", "ContainerSizeTypeName");

			var note = await _dbReadService.GetSingleRecordAsync<Note>(s => s.RequestId.Equals(RequestDto.RequestId));
			if (note != null)
            {
	            ViewData["NoteId"] = note.Id;
            }

			return View(product);
        }

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,ProductDescription,Upccode,ProductLocation,ProductCost,ProductPrice,SuggestedPrice,PackageSize,PackageType,ContainerSizeTypeId,ContainerTypeId,OrderWeek,CategoryId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy,UnitsPerCase")] Product product)
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
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", product.CategoryId);

            return RedirectToAction("Details", "Products", new { id = product.Id });
		}

        [Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			_dbReadService.IncludeEntityNavigation<Product, ContainerSizeType>();
			_dbReadService.IncludeEntityNavigation<Product, ContainerType>();
			_dbReadService.IncludeEntityNavigation<Product, Category>();
			var product = await _dbReadService.GetSingleRecordAsync<Product>(s => s.Id.Equals(id));

			if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

		[Authorize(Policy = GroupAuthorization.EmployeePolicyName)]
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
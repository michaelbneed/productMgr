using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PM.Business.Dto;
using PM.Business.Email;
using PM.Entity.Models;
using PM.Entity.Services;

namespace PM.UserAdmin.UI.Controllers
{
	public class ProductStoreSpecificPackagesController : Controller
	{
		private readonly VandivierProductManagerContext _context;
		private readonly IDbReadService _dbReadService;
		private readonly IDbWriteService _dbWriteService;
		private readonly IConfiguration _configuration;

		public ProductStoreSpecificPackagesController(VandivierProductManagerContext context,
			IDbReadService dbReadService, IDbWriteService dbWriteService, IConfiguration configuration)
		{
			_context = context;
			_dbReadService = dbReadService;
			_dbWriteService = dbWriteService;
			_configuration = configuration;
		}

		public async Task<IActionResult> Index(int? id)
		{
			ViewData["PackageTypeId"] = id;

			var package = await _dbReadService.GetSingleRecordAsync<ProductPackageType>(s => s.Id.Equals(id));
			ViewData["PackageName"] = package.AlternateProductName;

			var productStoreSpecific = await _dbReadService.GetAllRecordsAsync<ProductStoreSpecific>(s => s.PackageTypeId.Equals(id));
			productStoreSpecific.Reverse();

			return View(productStoreSpecific);
		}

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

			_dbReadService.IncludeEntityNavigation<Product>();
			_dbReadService.IncludeEntityNavigation<ProductPackageType>();

			var productStoreSpecific =
				await _dbReadService.GetSingleRecordAsync<ProductStoreSpecific>(p => p.Id.Equals(id));

			if (productStoreSpecific == null)
			{
				return NotFound();
			}

			return View(productStoreSpecific);
		}

		public IActionResult Create(int? id)
		{
			ViewData["PackageTypeId"] = id;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(int? id, [Bind("Id,ProductId,PackageTypeId,StoreName,StorePrice,StoreCost,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")]
			ProductStoreSpecific productStoreSpecific)
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

				productStoreSpecific.PackageTypeId = id;
				_dbWriteService.Add(productStoreSpecific);
				await _dbWriteService.SaveChangesAsync();

				var request = await _dbReadService.GetSingleRecordAsync<Request>(s => s.Id.Equals(RequestDto.RequestId));

				RequestEmail requestEmail = new RequestEmail(_configuration, _dbReadService);
				requestEmail.SendNewRequestToHeadQuarters(request);

				return RedirectToAction("Index", "ProductStoreSpecificPackages", new {id = productStoreSpecific.PackageTypeId});
			}

			ViewData["PackageTypeId"] = productStoreSpecific.PackageTypeId;

			return View(productStoreSpecific);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var productStoreSpecific = await _dbReadService.GetSingleRecordAsync<ProductStoreSpecific>(p => p.Id.Equals(id));
			if (productStoreSpecific == null)
			{
				return NotFound();
			}

			ViewData["PackageTypeId"] = productStoreSpecific.PackageTypeId;

			var note = await _dbReadService.GetSingleRecordAsync<Note>(s => s.RequestId.Equals(RequestDto.RequestId));
			if (note != null)
			{
				ViewData["NoteId"] = note.Id;
			}

			return View(productStoreSpecific);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,PackageTypeId,StoreName,StorePrice,StoreCost,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")]
			ProductStoreSpecific productStoreSpecific)
		{
			var fullProductStoreSpecific = await _dbReadService.GetSingleRecordAsync<ProductStoreSpecific>(p => p.Id.Equals(id));
			
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

					if (fullProductStoreSpecific != null)
					{
						productStoreSpecific.PackageTypeId = fullProductStoreSpecific.PackageTypeId;
					}
					_dbWriteService.Update(productStoreSpecific);
					await _dbWriteService.SaveChangesAsync();

					ViewData["PackageTypeId"] = productStoreSpecific.PackageTypeId;

					return RedirectToAction("Index", "ProductStoreSpecificPackages", new { id = productStoreSpecific.PackageTypeId });
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
			}
			return View(productStoreSpecific);
			
		}

		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _dbReadService.IncludeEntityNavigation<Product>();
            _dbReadService.IncludeEntityNavigation<ProductPackageType>();

			var productStoreSpecific = await _dbReadService.GetSingleRecordAsync<ProductStoreSpecific>(p => p.Id.Equals(id));
			if (productStoreSpecific == null)
            {
                return NotFound();
            }

            return View(productStoreSpecific);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productStoreSpecific = await _dbReadService.GetSingleRecordAsync<ProductStoreSpecific>(s => s.Id.Equals(id));
            _dbWriteService.Delete(productStoreSpecific);
            await _dbWriteService.SaveChangesAsync();

            return RedirectToAction("Index", "ProductStoreSpecificPackages", new { id = productStoreSpecific.PackageTypeId });
		}

		private async Task<bool> ProductStoreSpecificExists(int id)
		{
			var productStoreSpecific = _dbReadService.GetSingleRecordAsync<ProductStoreSpecific>(s => s.Id.Equals(id));
			return await _dbReadService.DoesRecordExist<ProductStoreSpecific>(e => productStoreSpecific.Id == id);
		}
	}
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PM.Business.Dto;
using PM.Business.Security;
using PM.Entity.Models;
using PM.Entity.Services;

namespace PM.UserAdmin.UI.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class SuppliersController : Controller
    {
        private readonly IDbReadService _dbReadService;
        private readonly IDbWriteService _dbWriteService;
        
		public SuppliersController(IDbReadService dbReadService, IDbWriteService dbWriteService)
        {
            _dbReadService = dbReadService;
            _dbWriteService = dbWriteService;
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public async Task<IActionResult> Index()
        {
	        RequestDto.RequestDescription = string.Empty;
			var suppliers = await _dbReadService.GetAllRecordsAsync<Supplier>();
			return View(suppliers.OrderBy(s => s.SupplierName));
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _dbReadService.GetSingleRecordAsync<Supplier>(s => s.Id.Equals(id));

            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public IActionResult Create()
        {
            return View();
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SupplierName,SupplierEmail,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] Supplier supplier)
        {
	        if (ModelState.IsValid)
            {
				if (User != null)
				{
					var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"name").Value;
					supplier.CreatedBy = userFullName;
				}

				supplier.CreatedOn = DateTime.Now;

				_dbWriteService.Add(supplier);

                await _dbWriteService.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        [Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _dbReadService.GetSingleRecordAsync<Supplier>(s => s.Id.Equals(id));
            
			if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SupplierName,SupplierEmail,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] Supplier supplier)
        {
            if (id != supplier.Id)
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
						supplier.UpdatedBy = userFullName;
	                }

	                supplier.UpdatedOn = DateTime.Now;

					_dbWriteService.Update(supplier);

                    await _dbWriteService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
	                bool result = await SupplierExists(supplier.Id);
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
            return View(supplier);
        }

        [Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			var supplier = await _dbReadService.GetSingleRecordAsync<Supplier>(s => s.Id.Equals(id));

			if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
			var supplier = await _dbReadService.GetSingleRecordAsync<Supplier>(s => s.Id.Equals(id));

			_dbWriteService.Delete(supplier);

			var response = await _dbWriteService.SaveChangesAsync();

			if (!response)
			{
				TempData["notifyUser"] = "This action could not be performed due to data constraints.";
	        }

			return RedirectToAction(nameof(Index));
        }

        private async Task<bool> SupplierExists(int id)
        {
	        var supplier = _dbReadService.GetSingleRecordAsync<Supplier>(s => s.Id.Equals(id));
			return await _dbReadService.DoesRecordExist<Supplier>(e => supplier.Id == id);
        }
    }
}

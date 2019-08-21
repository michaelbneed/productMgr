using System;
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

namespace PM.UserAdmin.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StoresController : Controller
    {
	    private readonly IDbReadService _dbReadService;
	    private readonly IDbWriteService _dbWriteService;

		public StoresController(IDbReadService dbReadService, IDbWriteService dbWriteService)
		{
			_dbReadService = dbReadService;
			_dbWriteService = dbWriteService;
		}

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public async Task<IActionResult> Index()
        {
	        RequestDto.RequestDescription = string.Empty;
			var stores = await _dbReadService.GetAllRecordsAsync<Store>();
	        return View(stores.OrderBy(s => s.StoreName));
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			var store = await _dbReadService.GetSingleRecordAsync<Store>(u => u.Id.Equals(id));

			if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public IActionResult Create()
        {
            return View();
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StoreName,StoreSupervisorName,StoreSupervisorEmail,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] Store store)
        {
            if (ModelState.IsValid)
            {
	            if (User != null)
	            {
		            var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"name").Value;
		            store.CreatedBy = userFullName;
	            }

	            store.CreatedOn = DateTime.Now;

	            _dbWriteService.Add(store);

	            await _dbWriteService.SaveChangesAsync();

	            return RedirectToAction(nameof(Index));
			}
            return View(store);
        }

        [Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			var store = await _dbReadService.GetSingleRecordAsync<Store>(s => s.Id.Equals(id));

			if (store == null)
            {
                return NotFound();
            }
            return View(store);
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StoreName,StoreSupervisorName,StoreSupervisorEmail,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] Store store)
        {
            if (id != store.Id)
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
						store.UpdatedBy = userFullName;
					}

					store.UpdatedOn = DateTime.Now;

					_dbWriteService.Update(store);

					await _dbWriteService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
					bool result = await StoreExists(store.Id);
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
            return View(store);
        }

        [Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			var store = await _dbReadService.GetSingleRecordAsync<Store>(s => s.Id.Equals(id));

			if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
			var store = await _dbReadService.GetSingleRecordAsync<Store>(s => s.Id.Equals(id));

			_dbWriteService.Delete(store);

			var response = await _dbWriteService.SaveChangesAsync();

			if (!response)
			{
				TempData["notifyUser"] = "This action could not be performed due to data constraints.";
			}

			return RedirectToAction(nameof(Index));
		}

		private async Task<bool> StoreExists(int id)
		{
			var store = _dbReadService.GetSingleRecordAsync<Store>(s => s.Id.Equals(id));
			return await _dbReadService.DoesRecordExist<Store>(e => store.Id == id);
		}
	}
}

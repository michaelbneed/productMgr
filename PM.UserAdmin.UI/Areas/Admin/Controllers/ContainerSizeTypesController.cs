﻿using System.Linq;
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
	public class ContainerSizeTypesController : Controller
    {
	    private readonly IDbReadService _dbReadService;
	    private readonly IDbWriteService _dbWriteService;
	    
	    public ContainerSizeTypesController(IDbReadService dbReadService, IDbWriteService dbWriteService)
        {
	        _dbReadService = dbReadService;
	        _dbWriteService = dbWriteService;
        }

	    [Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public async Task<IActionResult> Index()
		{
			RequestDto.RequestDescription = string.Empty;
	        var containerSizeTypes = await _dbReadService.GetAllRecordsAsync<ContainerSizeType>();
			return View(containerSizeTypes.OrderBy(s => s.ContainerSizeTypeName));
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var containerSizeType = await _dbReadService.GetSingleRecordAsync<ContainerSizeType>(u => u.Id.Equals(id));
            
            if (containerSizeType == null)
            {
                return NotFound();
            }

            return View(containerSizeType);
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public IActionResult Create()
        {
            return View();
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		[HttpPost]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ContainerSizeTypeName")] ContainerSizeType containerSizeType)
        {
			if (ModelState.IsValid)
			{
				_dbWriteService.Add(containerSizeType);

				await _dbWriteService.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(containerSizeType);
        }

        [Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public async Task<IActionResult> Edit(int? id)
        {
			if (id == null)
			{
				return NotFound();
			}

			var containerSizeType = await _dbReadService.GetSingleRecordAsync<ContainerSizeType>(s => s.Id.Equals(id));

			if (containerSizeType == null)
			{
				return NotFound();
			}
			return View(containerSizeType);
		}

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ContainerSizeTypeName")] ContainerSizeType containerSizeType)
        {
            if (id != containerSizeType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
	                _dbWriteService.Update(containerSizeType);
					await _dbWriteService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
					bool result = await ContainerSizeTypeExists(containerSizeType.Id);
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
            return View(containerSizeType);
        }

        [Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var containerSizeType = await _dbReadService.GetSingleRecordAsync<ContainerSizeType>(s => s.Id.Equals(id));
			
            if (containerSizeType == null)
            {
                return NotFound();
            }

            return View(containerSizeType);
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var containerSizeType = await _dbReadService.GetSingleRecordAsync<ContainerSizeType>(s => s.Id.Equals(id));

            _dbWriteService.Delete(containerSizeType);

            var response = await _dbWriteService.SaveChangesAsync();

            if (!response)
            {
	            TempData["notifyUser"] = "This action could not be performed due to data constraints.";
            }

            return RedirectToAction(nameof(Index));
		}

		private async Task<bool> ContainerSizeTypeExists(int id)
		{
			var containerSizeType = _dbReadService.GetSingleRecordAsync<ContainerSizeType>(s => s.Id.Equals(id));
			return await _dbReadService.DoesRecordExist<ContainerSizeType>(e => containerSizeType.Id == id);
		}
	}
}

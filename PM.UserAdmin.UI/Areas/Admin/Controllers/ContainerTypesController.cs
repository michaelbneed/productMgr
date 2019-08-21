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
	public class ContainerTypesController : Controller
    {
	    private readonly IDbReadService _dbReadService;
	    private readonly IDbWriteService _dbWriteService;
	    
	    public ContainerTypesController(IDbReadService dbReadService, IDbWriteService dbWriteService)
        {
	        _dbReadService = dbReadService;
	        _dbWriteService = dbWriteService;
        }

	    [Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public async Task<IActionResult> Index()
        {
	        RequestDto.RequestDescription = string.Empty;
			var containerTypes = await _dbReadService.GetAllRecordsAsync<ContainerType>();
			return View(containerTypes.OrderBy(s => s.ContainerTypeName));
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ContainerType = await _dbReadService.GetSingleRecordAsync<ContainerType>(u => u.Id.Equals(id));
            
            if (ContainerType == null)
            {
                return NotFound();
            }

            return View(ContainerType);
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public IActionResult Create()
        {
            return View();
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		[HttpPost]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ContainerTypeName")] ContainerType containerType)
        {
			if (ModelState.IsValid)
			{
				_dbWriteService.Add(containerType);

				await _dbWriteService.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(containerType);
        }

        [Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public async Task<IActionResult> Edit(int? id)
        {
			if (id == null)
			{
				return NotFound();
			}

			var ContainerType = await _dbReadService.GetSingleRecordAsync<ContainerType>(s => s.Id.Equals(id));

			if (ContainerType == null)
			{
				return NotFound();
			}
			return View(ContainerType);
		}

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ContainerTypeName")] ContainerType containerType)
        {
            if (id != containerType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
	                _dbWriteService.Update(containerType);
					await _dbWriteService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
					bool result = await ContainerTypeExists(containerType.Id);
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
            return View(containerType);
        }

        [Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var containerType = await _dbReadService.GetSingleRecordAsync<ContainerType>(s => s.Id.Equals(id));
			
            if (containerType == null)
            {
                return NotFound();
            }

            return View(containerType);
        }

		[Authorize(Policy = GroupAuthorization.AdminPolicyName)]
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ContainerType = await _dbReadService.GetSingleRecordAsync<ContainerType>(s => s.Id.Equals(id));

            _dbWriteService.Delete(ContainerType);

            var response = await _dbWriteService.SaveChangesAsync();

            if (!response)
            {
	            TempData["notifyUser"] = "This action could not be performed due to data constraints.";
            }

            return RedirectToAction(nameof(Index));
		}

		private async Task<bool> ContainerTypeExists(int id)
		{
			var ContainerType = _dbReadService.GetSingleRecordAsync<ContainerType>(s => s.Id.Equals(id));
			return await _dbReadService.DoesRecordExist<ContainerType>(e => ContainerType.Id == id);
		}
	}
}

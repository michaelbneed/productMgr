using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PM.DatabaseOperations.Models;
using PM.DatabaseOperations.Services;

namespace PM.UserAdmin.UI.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly IDbReadService _dbReadService;
        private readonly IDbWriteService _dbWriteService;
        
		public SuppliersController(IDbReadService dbReadService, IDbWriteService dbWriteService)
        {
            _dbReadService = dbReadService;
            _dbWriteService = dbWriteService;
        }

		// GET: Suppliers
		[Authorize]
		public async Task<IActionResult> Index()
        {
	        var suppliers = await _dbReadService.GetAllRecordsAsync<Supplier>();
	        return View(suppliers);
        }

        // GET: Suppliers/Details/5
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

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Suppliers/Edit/5
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

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Suppliers/Delete/5
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

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
			var supplier = await _dbReadService.GetSingleRecordAsync<Supplier>(s => s.Id.Equals(id));

			_dbWriteService.Delete(supplier);

            await _dbWriteService.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> SupplierExists(int id)
        {
	        var supplier = _dbReadService.GetSingleRecordAsync<Supplier>(s => s.Id.Equals(id));
			return await _dbReadService.DoesRecordExist<Supplier>(e => supplier.Id == id);
        }
    }
}

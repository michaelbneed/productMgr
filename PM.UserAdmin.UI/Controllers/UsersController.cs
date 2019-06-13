using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PM.Auth.GraphApi;
using PM.Entity.Services;
using PM.Entity.Models;

namespace PM.UserAdmin.UI.Controllers
{
    public class UsersController : Controller
    {
	    private VandivierProductManagerContext _context;
	    private readonly Microsoft.Extensions.Configuration.IConfiguration Configuration;
        private readonly IDbReadService _dbReadService;
        private readonly IDbWriteService _dbWriteService;

        public UsersController(VandivierProductManagerContext context, IConfiguration configuration, 
	        IDbReadService dbReadService, IDbWriteService dbWriteService)

		{
			_context = context;
			Configuration = configuration;
			_dbReadService = dbReadService;
			_dbWriteService = dbWriteService;
		}

		// GET: Users
		[Authorize]
		public async Task<IActionResult> Index()
        {
	        _dbReadService.IncludeEntityNavigation<Supplier>();
			var users = await _dbReadService.GetAllRecordsAsync<User>();
			return View(users);
		}

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			_dbReadService.IncludeEntityNavigation<Supplier>();
            var user = await _dbReadService.GetSingleRecordAsync<User>(u => u.Id.Equals(id));

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SupplierId,FirstName,LastName,EmailAddress,AuthId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] User user)
        {
            if (ModelState.IsValid)
            {
	            if (User != null)
	            {
		            var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"name").Value;
		            user.CreatedBy = userFullName;
	            }

	            user.CreatedOn = DateTime.Now;

				GraphClient graphClient = new GraphClient(Configuration);
	            graphClient.CreateUser(user, out var objectId);

				_dbWriteService.Add(user);
                await _dbWriteService.SaveChangesAsync();
				
				return RedirectToAction(nameof(Index));
            }
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName", user.SupplierId);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _dbReadService.GetSingleRecordAsync<User>(s => s.Id.Equals(id));
			if (user == null)
            {
                return NotFound();
            }
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName", user.SupplierId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SupplierId,FirstName,LastName,EmailAddress,AuthId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] User user)
        {
            if (id != user.Id)
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
		                user.UpdatedBy = userFullName;
	                }

	                user.UpdatedOn = DateTime.Now;

					_dbWriteService.Update(user);

                    await _dbWriteService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
					bool result = await UserExists(user.Id);
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
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName", user.SupplierId);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			_dbReadService.IncludeEntityNavigation<Supplier>();
			var user = await _dbReadService.GetSingleRecordAsync<User>(u => u.Id.Equals(id));

			if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
			var user = await _dbReadService.GetSingleRecordAsync<User>(u => u.Id.Equals(id));
			
			_dbWriteService.Delete(user);
			await _dbWriteService.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> UserExists(int id)
        {
			var user = _dbReadService.GetSingleRecordAsync<User>(u => u.Id.Equals(id));
			return await _dbReadService.DoesRecordExist<User>(e => user.Id == id);
		}
    }
}

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
using PM.DatabaseOperations.Services;
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
                    _dbWriteService.Update(user);
                    await _dbWriteService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
			await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}

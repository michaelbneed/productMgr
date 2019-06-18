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

        [Authorize]
        public async Task<IActionResult> Index()
        {
            _dbReadService.IncludeEntityNavigation<Supplier>();
            var users = await _dbReadService.GetAllRecordsAsync<User>();

            return View(users.OrderBy(s => s.LastName));
        }

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

        public IActionResult Create()
        {
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SupplierId,FirstName,LastName,EmailAddress,AuthId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] User user)
        {
	        var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"name").Value;

			if (ModelState.IsValid)
            {
	            if (User != null)
	            {
		            user.CreatedBy = userFullName;
	            }

	            user.CreatedOn = DateTime.Now;
				var graphClient = new GraphClient(Configuration, true);

                var graphResult = graphClient.CreateUser(user);

                if (graphResult)
                {
                    _dbWriteService.Add(user);

                    var saveResult = await _dbWriteService.SaveChangesAsync();

                    if (saveResult)
                        return RedirectToAction(nameof(Index));

                    // Failed to save to the database, delete the user from the directory.
                    graphClient.DeleteUser(user.AuthId);
                }
            }

            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "SupplierName", user.SupplierId);

            return View(user);
        }

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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _dbReadService.GetSingleRecordAsync<User>(u => u.Id.Equals(id));

            var graphClient = new GraphClient(Configuration, true);
            var graphResult = graphClient.DeleteUser(user.AuthId);

            if (graphResult)
            {
                _dbWriteService.Delete(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}

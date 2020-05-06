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
using PM.Entity.Models;
using PM.Entity.Services;

namespace PM.Vendor.UI.Controllers
{
    public class NotesController : Controller
    {
        private readonly VandivierProductManagerContext _context;
        private readonly IDbReadService _dbReadService;
        private readonly IDbWriteService _dbWriteService;
        private readonly IConfiguration _configuration;

		public NotesController(VandivierProductManagerContext context, IConfiguration configuration, 
						IDbReadService dbReadService, IDbWriteService dbWriteService)
        {
            _context = context;
            _configuration = configuration;
            _dbReadService = dbReadService;
            _dbWriteService = dbWriteService;
		}

		[Authorize]
		public async Task<IActionResult> Index(int? id)
		{
			_dbReadService.IncludeEntityNavigation<Request>();
			List<Note> notes = null;
			if (id > 0 || id != null) 
			{
				notes = await _dbReadService.GetAllRecordsAsync<Note>(s => s.RequestId.Equals(id));
			}
			else
			{
				notes = await _dbReadService.GetAllRecordsAsync<Note>();
			}

			notes.Reverse();

			return View(notes);
		}

		[Authorize]
		public async Task<IActionResult> Details(int? id)
        {
	        _dbReadService.IncludeEntityNavigation<Request>();
			if (id == null)
            {
                return NotFound();
            }

			_dbReadService.IncludeEntityNavigation<Request>();

            var note = await _dbReadService.GetSingleRecordAsync<Note>(s => s.Id.Equals(id));
                
            if (note == null)
            {
                return NotFound();
            }

            var productRelation = await _dbReadService.GetSingleRecordAsync<Request>(s => s.Id.Equals(RequestDto.RequestId));

            if (productRelation != null)
            {
				ViewData["ProductId"] = productRelation.ProductId;
			}

            return View(note);
        }

		[Authorize]
		public IActionResult CreateNote(int? id)
		{
			return View();
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateNote(int? id, [Bind("Id,NoteText,SendEmailRequestor,SendEmailSupplier,RequestId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] Note note)
		{
			note.Id = 0;
			if (ModelState.IsValid)
			{
				if (User != null)
				{
					var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"emails").Value;
					note.CreatedBy = userFullName;
				}

				note.CreatedOn = DateTime.Now;

				note.RequestId = id;
				_dbWriteService.Add(note);

				await _dbWriteService.SaveChangesAsync();

				var request = await _dbReadService.GetSingleRecordAsync<Request>(r => r.Id.Equals(id));

				RequestEmail email = new RequestEmail(_configuration, _dbReadService);
				var user = await _dbReadService.GetSingleRecordAsync<Entity.Models.User>(u => u.Id.Equals(request.UserId));
				if (user.SupplierId != null && user.SupplierId > 0)
				{
					email.SendNewNoteEmailToOriginatingUser(request, note, user.SupplierId);
				}
				else
				{
					email.SendNewNoteEmailToOriginatingUser(request, note);
				}
			}
			return RedirectToAction("Index", "Notes", new { id = note.RequestId });
		}

		[Authorize]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _dbReadService.GetSingleRecordAsync<Note>(s => s.Id.Equals(id));

            if (note == null)
            {
                return NotFound();
            }

            ViewData["RequestId"] = new SelectList(_context.Request, "Id", "RequestDescription", note.RequestId);

            var productRelation = await _dbReadService.GetSingleRecordAsync<Request>(s => s.Id.Equals(RequestDto.RequestId));

            if (productRelation != null)
            {
	            ViewData["ProductId"] = productRelation.ProductId;
            }

			return View(note);
        }

		[Authorize]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NoteText,SendEmailRequestor,SendEmailSupplier,RequestId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] Note note)
        {
            if (id != note.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
					if (User != null)
					{
						var userFullName = User.Claims.FirstOrDefault(x => x.Type == $"emails").Value;
						note.UpdatedBy = userFullName;
					}

					note.UpdatedOn = DateTime.Now;

					_dbWriteService.Update(note);
                    await _dbWriteService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
					bool result = await NoteExists(note.Id);
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
            ViewData["RequestId"] = new SelectList(_context.Request, "Id", "RequestDescription", note.RequestId);
			return RedirectToAction("Index", "Notes", new { id = note.RequestId });
		}

        [Authorize]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			_dbReadService.IncludeEntityNavigation<Request>();

            var note = await _dbReadService.GetSingleRecordAsync<Note>(s => s.Id.Equals(id));
                
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

		[Authorize]
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var note = await _dbReadService.GetSingleRecordAsync<Note>(s => s.Id.Equals(id));

            _dbWriteService.Delete(note);

			var response = await _dbWriteService.SaveChangesAsync();

			if (!response)
			{
				TempData["notifyUser"] = "This action could not be performed due to data constraints.";
			}

			return RedirectToAction("Index", "Notes", new { id = note.RequestId });
		}

		private async Task<bool> NoteExists(int id)
		{
			var note = _dbReadService.GetSingleRecordAsync<Note>(s => s.Id.Equals(id));
			return await _dbReadService.DoesRecordExist<Note>(e => note.Id == id);
		}
    }
}

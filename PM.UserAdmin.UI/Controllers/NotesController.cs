using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PM.DatabaseOperations.Services;
using PM.Entity.Models;

namespace PM.UserAdmin.UI.Controllers
{
    public class NotesController : Controller
    {
	    private VandivierProductManagerContext _context;
		private readonly IDbReadService _dbReadService;
	    private readonly IDbWriteService _dbWriteService;

        public NotesController(VandivierProductManagerContext context, IDbReadService dbReadService, IDbWriteService dbWriteService)
        {
	        _context = context;
	        _dbReadService = dbReadService;
	        _dbWriteService = dbWriteService;
        }

        // GET: Notes
        public async Task<IActionResult> Index()
        {
            _dbReadService.IncludeEntityNavigation<Request>();
            var notes = await _dbReadService.GetAllRecordsAsync<Note>();
            return View(notes);
		}

        // GET: Notes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _dbReadService.IncludeEntityNavigation<Request>();
            var note = await _dbReadService.GetSingleRecordAsync<Note>(n => n.Id.Equals(id));

            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // GET: Notes/Create
        public IActionResult Create()
        {
            ViewData["RequestId"] = new SelectList(_context.Request, "Id", "RequestDescription");
            return View();
        }

        // POST: Notes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NoteText,RequestId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] Note note)
        {
            if (ModelState.IsValid)
            {
                _dbWriteService.Add(note);
                await _dbWriteService.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["RequestId"] = new SelectList(_context.Request, "Id", "RequestDescription", note.RequestId);
            return View(note);
        }

        // GET: Notes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _dbReadService.GetSingleRecordAsync<Note>(n => n.Id.Equals(id));

			if (note == null)
            {
                return NotFound();
            }
            ViewData["RequestId"] = new SelectList(_context.Request, "Id", "RequestDescription", note.RequestId);
            return View(note);
        }

        // POST: Notes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NoteText,RequestId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] Note note)
        {
            if (id != note.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbWriteService.Update(note);
                    await _dbWriteService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoteExists(note.Id))
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
            ViewData["RequestId"] = new SelectList(_context.Request, "Id", "RequestDescription", note.RequestId);
            return View(note);
        }

        // GET: Notes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			_dbReadService.IncludeEntityNavigation<Request>();
			var note = await _dbReadService.GetSingleRecordAsync<Note>(u => u.Id.Equals(id));

			if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var note = await _dbReadService.GetSingleRecordAsync<Note>(n => n.Id.Equals(id));

			_dbWriteService.Delete(note);
            await _dbWriteService.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool NoteExists(int id)
        {
            return _context.Note.Any(e => e.Id == id);
        }
    }
}

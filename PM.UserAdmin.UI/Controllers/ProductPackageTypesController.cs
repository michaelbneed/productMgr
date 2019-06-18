using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PM.Entity.Models;

namespace PM.UserAdmin.UI.Controllers
{
    public class ProductPackageTypesController : Controller
    {
        private readonly VandivierProductManagerContext _context;

        public ProductPackageTypesController(VandivierProductManagerContext context)
        {
            _context = context;
        }

        // GET: ProductPackageTypes
        public async Task<IActionResult> Index()
        {
            var vandivierProductManagerContext = _context.ProductPackageType.Include(p => p.Product);
            return View(await vandivierProductManagerContext.ToListAsync());
        }

        // GET: ProductPackageTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productPackageType = await _context.ProductPackageType
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productPackageType == null)
            {
                return NotFound();
            }

            return View(productPackageType);
        }

        // GET: ProductPackageTypes/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "ProductName");
            return View();
        }

        // POST: ProductPackageTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Quantity,Unit,AlternateProductName,AlternateProductUpcCode,Supplier,SupplierId,AlternateProductPrice,AlternateProductCost,ProductId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] ProductPackageType productPackageType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productPackageType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "ProductName", productPackageType.ProductId);
            return View(productPackageType);
        }

        // GET: ProductPackageTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productPackageType = await _context.ProductPackageType.FindAsync(id);
            if (productPackageType == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "ProductName", productPackageType.ProductId);
            return View(productPackageType);
        }

        // POST: ProductPackageTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Quantity,Unit,AlternateProductName,AlternateProductUpcCode,Supplier,SupplierId,AlternateProductPrice,AlternateProductCost,ProductId,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy")] ProductPackageType productPackageType)
        {
            if (id != productPackageType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productPackageType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductPackageTypeExists(productPackageType.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "ProductName", productPackageType.ProductId);
            return View(productPackageType);
        }

        // GET: ProductPackageTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productPackageType = await _context.ProductPackageType
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productPackageType == null)
            {
                return NotFound();
            }

            return View(productPackageType);
        }

        // POST: ProductPackageTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productPackageType = await _context.ProductPackageType.FindAsync(id);
            _context.ProductPackageType.Remove(productPackageType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductPackageTypeExists(int id)
        {
            return _context.ProductPackageType.Any(e => e.Id == id);
        }
    }
}

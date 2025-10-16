using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Main.Domain.entities.item;
using Main.Infrastructure.DataAccess;

namespace Main.Presentation.MVC.Views
{
    public class ItemFieldValuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItemFieldValuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ItemFieldValues
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ItemFieldValues.Include(i => i.InventoryField).Include(i => i.Item);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ItemFieldValues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemFieldValue = await _context.ItemFieldValues
                .Include(i => i.InventoryField)
                .Include(i => i.Item)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (itemFieldValue == null)
            {
                return NotFound();
            }

            return View(itemFieldValue);
        }

        // GET: ItemFieldValues/Create
        public IActionResult Create()
        {
            ViewData["InventoryFieldId"] = new SelectList(_context.InventoryFields, "Id", "Description");
            ViewData["ItemId"] = new SelectList(_context.Items, "Id", "CreatedById");
            return View();
        }

        // POST: ItemFieldValues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,InventoryFieldId,TextValue,MultilineTextValue,NumberValue,FileUrl,BooleanValue,CreatedAt,UpdatedAt,Id")] ItemFieldValue itemFieldValue)
        {
            if (ModelState.IsValid)
            {
                _context.Add(itemFieldValue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InventoryFieldId"] = new SelectList(_context.InventoryFields, "Id", "Description", itemFieldValue.InventoryFieldId);
            ViewData["ItemId"] = new SelectList(_context.Items, "Id", "CreatedById", itemFieldValue.ItemId);
            return View(itemFieldValue);
        }

        // GET: ItemFieldValues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemFieldValue = await _context.ItemFieldValues.FindAsync(id);
            if (itemFieldValue == null)
            {
                return NotFound();
            }
            ViewData["InventoryFieldId"] = new SelectList(_context.InventoryFields, "Id", "Description", itemFieldValue.InventoryFieldId);
            ViewData["ItemId"] = new SelectList(_context.Items, "Id", "CreatedById", itemFieldValue.ItemId);
            return View(itemFieldValue);
        }

        // POST: ItemFieldValues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,InventoryFieldId,TextValue,MultilineTextValue,NumberValue,FileUrl,BooleanValue,CreatedAt,UpdatedAt,Id")] ItemFieldValue itemFieldValue)
        {
            if (id != itemFieldValue.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemFieldValue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemFieldValueExists(itemFieldValue.Id))
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
            ViewData["InventoryFieldId"] = new SelectList(_context.InventoryFields, "Id", "Description", itemFieldValue.InventoryFieldId);
            ViewData["ItemId"] = new SelectList(_context.Items, "Id", "CreatedById", itemFieldValue.ItemId);
            return View(itemFieldValue);
        }

        // GET: ItemFieldValues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemFieldValue = await _context.ItemFieldValues
                .Include(i => i.InventoryField)
                .Include(i => i.Item)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (itemFieldValue == null)
            {
                return NotFound();
            }

            return View(itemFieldValue);
        }

        // POST: ItemFieldValues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var itemFieldValue = await _context.ItemFieldValues.FindAsync(id);
            if (itemFieldValue != null)
            {
                _context.ItemFieldValues.Remove(itemFieldValue);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemFieldValueExists(int id)
        {
            return _context.ItemFieldValues.Any(e => e.Id == id);
        }
    }
}

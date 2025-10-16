using Main.Application.Dtos;
using Main.Application.Interfaces;
using Main.Domain.entities.inventory;
using Main.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Main.Presentation.MVC.Controllers
{
    public class InventoriesController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoriesController> _logger;

        public InventoriesController(IInventoryService inventoryService,
            ILogger<InventoriesController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        //// GET: Inventories
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
           var t = await _inventoryService.GetAll(cancellationToken); 
            return View(t);
        }

       
        // GET: Inventories/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]
            CreateInventoryDto createDto,
            CancellationToken cancellationToken = default)
        {
            createDto = createDto with { CustomIdFormat = "eghnteh" };
            Console.WriteLine($"Fields count: {createDto.Fields?.Count}");
            if (createDto.Fields != null)
            {
                foreach (var field in createDto.Fields)
                {
                    Console.WriteLine($"Field: {field.Name}, Type: {field.FieldType}");
                }
            }
            var ownerId = "kjilsfhrfbeibkv ";
            if (string.IsNullOrEmpty(ownerId))
                return Unauthorized("User not authenticated");

            var inventory = await _inventoryService.CreateInventoryAsync(createDto, ownerId, cancellationToken);

            _logger.LogInformation("User {UserId} created inventory {InventoryId}", ownerId, inventory.Id);

            //return CreatedAtAction(nameof(GetInventory), new { id = inventory.Id }, inventory);
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int[] selectedIds) // Используйте тип вашего ID (int, Guid)
        {
            await _inventoryService.DeleteInventoryAsync(selectedIds);
            return RedirectToAction(nameof(Index));
        }
        //// GET: Inventories/Edit/5
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _inventoryService.GetById(id, cancellationToken);
            if (inventory == null)
            {
                return NotFound();
            }
            //ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", inventory.CategoryId);
            return View(inventory);
        }

        //public async Task<IActionResult> Items(int id, CancellationToken cancellationToken)
        //{
        //    return RedirectToAction("Index","items",id);
        //}
        // POST: Inventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(InventoryDto inventory, CancellationToken cancellationToken)
        {
                try
                {
                   await _inventoryService.UpdateInventoryAsync(inventory);
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                }
            return RedirectToAction("Index", "Items", new { inventoryId  = inventory.Id});
        }
        //// GET: Inventories/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var inventory = await _context.Inventories
        //        .Include(i => i.Category)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (inventory == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(inventory);
        //}

        //// GET: Inventories/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var inventory = await _context.Inventories
        //        .Include(i => i.Category)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (inventory == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(inventory);
        //}

        //// POST: Inventories/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var inventory = await _context.Inventories.FindAsync(id);
        //    if (inventory != null)
        //    {
        //        _context.Inventories.Remove(inventory);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

    }
}

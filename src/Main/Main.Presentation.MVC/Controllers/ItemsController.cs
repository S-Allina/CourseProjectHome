using Main.Application.Dtos;
using Main.Application.Interfaces;
using Main.Application.Services;
using Main.Domain.entities.item;
using Main.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Main.Presentation.MVC.Controllers
{
    public class ItemsController : Controller
    {
        private readonly IItemService _itemService;
        private readonly IInventoryService _inventoryService;

        public ItemsController(IItemService itemService, IInventoryService inventoryService)
        {
            _itemService = itemService;
            _inventoryService = inventoryService;
        }

        // GET: Items
        [HttpGet("{id}")]
        public async Task<IActionResult> Index(int? inventoryId, CancellationToken cancellationToken)
        {
            // Если передан inventoryId, показываем товары этого инвентаря
            if (inventoryId.HasValue)
            {
                var items = await _itemService.GetByInventoryAsync(inventoryId.Value, cancellationToken);
                var inventory = await _inventoryService.GetById(inventoryId.Value, cancellationToken);

                ViewBag.SelectedInventory = inventory;
                return View(items);
            }

            // Иначе показываем список инвентарей
            var inventories = await _inventoryService.GetAll(cancellationToken);
            return View(inventories);
        }

        // Метод для получения товаров в формате JSON (для AJAX)
        [HttpGet]
        public async Task<IActionResult> GetInventoryItems(int inventoryId, CancellationToken cancellationToken)
        {
            var items = await _itemService.GetByInventoryAsync(inventoryId, cancellationToken);
            var inventory = await _inventoryService.GetById(inventoryId, cancellationToken);

            return Json(new { Items = items, Inventory = inventory });
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            ////var item = await _context.Items
            ////    .Include(i => i.Inventory)
            ////    .FirstOrDefaultAsync(m => m.Id == id);
            //if (item == null)
            //{
            //    return NotFound();
            //}

            return View();
        }
        public async Task<IActionResult> Create(int inventoryId, CancellationToken cancellationToken)
        {
            var inventory = await _inventoryService.GetById(inventoryId, cancellationToken);
            if (inventory == null)
            {
                return NotFound();
            }

            var createDto = new CreateItemDto
            {
                InventoryId = inventoryId,
                FieldValues = inventory.Fields.Select(f => new CreateItemFieldValueDto
                {
                    InventoryFieldId = f.Id,
                    FieldName = f.Name,
                    FieldType = f.FieldType,
                    IsRequired = f.IsRequired
                }).ToList()
            };

            ViewBag.Inventory = inventory;
            return View(createDto);
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateItemDto createDto, CancellationToken cancellationToken)
        {
                try
                {
                    var item = await _itemService.CreateAsync(createDto, "hhhhh", cancellationToken);
                    return RedirectToAction("Index", "Items", new { inventoryId = createDto.InventoryId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating item: " + ex.Message);
                }

            return View(createDto);
        }
        //// POST: Items/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("InventoryId,CustomId,CreatedById,Version,CreatedAt,UpdatedAt,Id")] Item item)
        //{
        //    if (id != item.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(item);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ItemExists(item.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["InventoryId"] = new SelectList(_context.Inventories, "Id", "CustomIdFormat", item.InventoryId);
        //    return View(item);
        //}

        //// GET: Items/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var item = await _context.Items
        //        .Include(i => i.Inventory)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (item == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(item);
        //}

        //// POST: Items/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var item = await _context.Items.FindAsync(id);
        //    if (item != null)
        //    {
        //        _context.Items.Remove(item);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool ItemExists(int id)
        //{
        //    return _context.Items.Any(e => e.Id == id);
        //}
    }
}

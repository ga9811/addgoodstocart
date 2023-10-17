using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewShoppingCart.Data;
using NewShoppingCart.Models;
using Newtonsoft.Json;

namespace NewShoppingCart.Controllers
{
    public class ItemModelsController : Controller
    {
        private readonly NewShoppingCartContext _context;
        private readonly ILogger<ItemModelsController> _logger;

        //public ItemModelsController(NewShoppingCartContext context)
        //{
        //    _context = context;
        //}
        public ItemModelsController(NewShoppingCartContext context, ILogger<ItemModelsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: ItemModels
        public async Task<IActionResult> Index(string userId)
        {
            ViewBag.UserId = userId;  // Pass UserId to the view
            _logger.LogInformation($"Received UserId: {userId}");
            return _context.ItemModel != null ?
                      View(await _context.ItemModel.ToListAsync()) :
                      Problem("Entity set 'NewShoppingCartContext.ItemModel' is null.");
        }

        [HttpPost]
        public IActionResult Order(string UserId, IDictionary<string, int> orderAmounts)
        {
            List<UserCartItem> cartItems = new List<UserCartItem>();

            foreach (var order in orderAmounts)
            {
                if (order.Value > 0) // Ensure that the item quantity is greater than zero
                {
                    var item = _context.ItemModel.Find(Convert.ToInt32(order.Key));
                    if (item != null)
                    {
                        cartItems.Add(new UserCartItem
                        {
                            UserId = UserId,
                            ItemName = item.name,
                            Price = item.price,
                            Quantity = order.Value
                        });
                    }
                }
            }

            //TempData["CartItems"] = cartItems; // Store temporarily in TempData
            TempData["CartItems"] = JsonConvert.SerializeObject(cartItems);
            _logger.LogInformation($"TempData[\"CartItems\"] content: {TempData["CartItems"]}");
            // Redirect the user to the Cart index page without saving anything to the database yet.
            return RedirectToAction("Index", "UserCartItems");
        }








        // GET: ItemModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ItemModel == null)
            {
                return NotFound();
            }

            var itemModel = await _context.ItemModel
                .FirstOrDefaultAsync(m => m.id == id);
            if (itemModel == null)
            {
                return NotFound();
            }

            return View(itemModel);
        }

        // GET: ItemModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ItemModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,price,qty")] ItemModel itemModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(itemModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(itemModel);
        }

        // GET: ItemModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ItemModel == null)
            {
                return NotFound();
            }

            var itemModel = await _context.ItemModel.FindAsync(id);
            if (itemModel == null)
            {
                return NotFound();
            }
            return View(itemModel);
        }

        // POST: ItemModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,price,qty")] ItemModel itemModel)
        {
            if (id != itemModel.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemModelExists(itemModel.id))
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
            return View(itemModel);
        }

        // GET: ItemModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ItemModel == null)
            {
                return NotFound();
            }

            var itemModel = await _context.ItemModel
                .FirstOrDefaultAsync(m => m.id == id);
            if (itemModel == null)
            {
                return NotFound();
            }

            return View(itemModel);
        }

        // POST: ItemModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ItemModel == null)
            {
                return Problem("Entity set 'NewShoppingCartContext.ItemModel'  is null.");
            }
            var itemModel = await _context.ItemModel.FindAsync(id);
            if (itemModel != null)
            {
                _context.ItemModel.Remove(itemModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemModelExists(int id)
        {
          return (_context.ItemModel?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}

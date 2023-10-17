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
    public class UserCartItemsController : Controller
    {
        private readonly NewShoppingCartContext _context;
        private readonly ILogger<UserCartItemsController> _logger;

        public UserCartItemsController(NewShoppingCartContext context, ILogger<UserCartItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: UserCartItems
        public IActionResult Index()
        {
            if (TempData["CartItems"] is string serializedUserCartItems)
            {
                var userCartItems = JsonConvert.DeserializeObject<List<UserCartItem>>(serializedUserCartItems);


                _logger.LogInformation($"Deserialized UserCartItems count: {userCartItems.Count}");

                return View(userCartItems);
            }


            return View(new List<UserCartItem>());
        }

        public async Task<IActionResult> Confirm()
        {
            try
            {
                var cartItemsSerialized = TempData["CartItems"] as string;
                if (string.IsNullOrEmpty(cartItemsSerialized))
                {
                    TempData["ErrorMessage"] = "Cart items not found.";
                    return RedirectToAction(nameof(Index));
                }

                var cartItems = JsonConvert.DeserializeObject<List<UserCartItem>>(cartItemsSerialized);

                foreach (var cartItem in cartItems)
                {
                    var existingItem = await _context.ItemModel.FirstOrDefaultAsync(item => item.name == cartItem.ItemName);
                    if (existingItem == null)
                    {
                        TempData["ErrorMessage"] = $"Item {cartItem.ItemName} not found.";
                        return RedirectToAction(nameof(Index));
                    }

                    existingItem.qty -= cartItem.Quantity;
                    if (existingItem.qty < 0)
                    {
                        TempData["ErrorMessage"] = $"Not enough stock for {cartItem.ItemName}.";
                        return RedirectToAction(nameof(Index));
                    }

                    _logger.LogInformation($"Updating {existingItem.name} from {existingItem.qty + cartItem.Quantity} to {existingItem.qty}");

                    _context.Update(existingItem);
                    await _context.SaveChangesAsync();  // Save changes immediately after updating each item.
                }


                
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
            }

            return RedirectToAction(nameof(Index));
        }







        public IActionResult Cancel()
        {

            return RedirectToAction("Index", "ItemModels");
        }
        // GET: UserCartItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UserCartItem == null)
            {
                return NotFound();
            }

            var userCartItem = await _context.UserCartItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userCartItem == null)
            {
                return NotFound();
            }

            return View(userCartItem);
        }

        // GET: UserCartItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserCartItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ItemName,Price,Quantity")] UserCartItem userCartItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userCartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userCartItem);
        }

        // GET: UserCartItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UserCartItem == null)
            {
                return NotFound();
            }

            var userCartItem = await _context.UserCartItem.FindAsync(id);
            if (userCartItem == null)
            {
                return NotFound();
            }
            return View(userCartItem);
        }

        // POST: UserCartItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ItemName,Price,Quantity")] UserCartItem userCartItem)
        {
            if (id != userCartItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userCartItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserCartItemExists(userCartItem.Id))
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
            return View(userCartItem);
        }

        // GET: UserCartItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UserCartItem == null)
            {
                return NotFound();
            }

            var userCartItem = await _context.UserCartItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userCartItem == null)
            {
                return NotFound();
            }

            return View(userCartItem);
        }

        // POST: UserCartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UserCartItem == null)
            {
                return Problem("Entity set 'NewShoppingCartContext.UserCartItem'  is null.");
            }
            var userCartItem = await _context.UserCartItem.FindAsync(id);
            if (userCartItem != null)
            {
                _context.UserCartItem.Remove(userCartItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserCartItemExists(int id)
        {
          return (_context.UserCartItem?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bangazon.Data;
using Bangazon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Bangazon.Models.CartViewModels;

namespace Bangazon.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Cart
        public async Task<IActionResult> Index(int? id)
        {
            var user = await GetCurrentUserAsync();
            var order = await _context.Order
                                        .Include(o => o.OrderProducts)
                                        .ThenInclude(op => op.Product)
                                        .Where(o => o.PaymentTypeId == null)
                                        .Where(o => o.User == user)
                                        .FirstOrDefaultAsync()
                                        ;

            return View(order);
        }
        
        // GET: Cart/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context
                .OrderProduct
                .Include(op => op.Product)
                .ThenInclude(p => p.ProductType)
                .FirstOrDefaultAsync(op => op.OrderProductId == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // POST: Cart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cartItem = await _context.OrderProduct.FindAsync(id);
            _context.OrderProduct.Remove(cartItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Cart/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.OrderProduct
                .Include(op => op.Product)
                .ThenInclude(p => p.ProductType)
                .FirstOrDefaultAsync(op => op.OrderProductId == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // GET: Checkout - get all payment options for current user
        public async Task<IActionResult> Checkout(int id)
        {
            var user = await GetCurrentUserAsync();
            var model = new CheckoutViewModel
            {
                Order = await _context.Order
                    .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                    .Where(o => o.User == user && o.PaymentTypeId == null)
                    .FirstOrDefaultAsync(),
                PaymentTypes = await _context.PaymentType
                .Where(pt => pt.User == user && pt.Active == true)
                .ToListAsync()
            };

            return View(model);
        }

        // POST: Place order - check all the stuff

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }
    }
}

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
            var applicationDbContext = _context.OrderProduct
                                        .Include(op => op.Product)
                                        .Include(op => op.Order)
                                        .Where(op => op.Order.PaymentTypeId == null)
                                        .Where(op => op.Order.User == user)
                                        ;
                                        
            return View(await applicationDbContext.ToListAsync());
        }
        



        // GET: Cart/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context
                .Product
                //.Include(p => p.ProductType)
                //.Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Cart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.OrderProduct.FindAsync(id);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        /*
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderproduct = await _context.OrderProduct.FindAsync();
            _context.OrderProduct.Remove();
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        // GET: Cart/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.ProductType)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }
    }
}

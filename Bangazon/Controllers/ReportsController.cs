using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bangazon.Data;
using Bangazon.Models;
using Bangazon.Models.ProductViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Bangazon.Models.ReportViewModels;

namespace Bangazon.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public ReportsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public async Task<IActionResult> IncompleteOrders()
        {
            var user = await GetCurrentUserAsync();
            var incompleteOrders = await _context.Order
                .Include(o => o.User)
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .Where(o => o.PaymentType == null && o.OrderProducts.Any(op => op.Product.User == user))
                .ToListAsync();

            return View(incompleteOrders);
        }
        public async Task<IActionResult> MultipleOrders()
        {
            var user = await GetCurrentUserAsync();

            var model = new MultipleOrdersViewModel();


            model.MultipleOrdersList = await _context.ApplicationUsers
                .Include(u => u.Orders)
                .Where(u => u.Orders.Any(o => o.OrderProducts.Any(op => op.Product.User == user)))
                .Where(u => u.Orders.Where(o => o.PaymentTypeId == null).Count() > 1)
                .Select(u => new UserOrderCount
                {
                    User = u,
                    OpenOrderCount = u.Orders.Where(o => o.PaymentTypeId == null).Count()
                })
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> AbandonedProductTypes()
        {
            var user = await GetCurrentUserAsync();

            var model = new ProductTypesViewModel();

            model.GroupedProducts = await _context
                .ProductType
                .OrderBy(pt => pt.Label)
                .Select(pt => new GroupedProducts
                {
                    TypeId = pt.ProductTypeId,
                    TypeName = pt.Label,
                    ProductCount = pt.Products.Where(p => p.Active == true).Count(),
                    Products = pt.Products.OrderByDescending(p => p.DateCreated).Where(p => p.Active == true).Take(3)
                }).ToListAsync();

            return View(model);
        }
    }
}
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
    // TODO: Confirm if client wants unauthorized users to be able to view products
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public async Task<IActionResult> Types()
        {
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

        public async Task<IActionResult> AddToOrder(int productId)
        {
            var user = await GetCurrentUserAsync();
            // Check to see if active user (customer) has an open order
            var openOrder = await _context.Order.FirstOrDefaultAsync(o => o.User == user && o.PaymentTypeId == null);
            var orderId = 0;
            if (openOrder == null)
            {
            // -- if no, create order in the order table and return new order ID
                var newOrder = new Order()
                {
                    UserId = user.Id.ToString()
                };
                _context.Add(newOrder);
                await _context.SaveChangesAsync();
                var addedOrder = await _context.Order.FirstOrDefaultAsync(o => o.User == user && o.PaymentTypeId == null);
                orderId = addedOrder.OrderId;
            }
            else
            {
                orderId = openOrder.OrderId;
            }
            // then create entry in order product table using the order ID and product ID
            var newOrderProduct = new OrderProduct()
            {
                OrderId = orderId,
                ProductId = productId
            };
            _context.Add(newOrderProduct);
            await _context.SaveChangesAsync();

            var errMsg = TempData["SuccessMessage"] as string;
            TempData["SuccessMessage"] = "This product has been added to your shopping cart";

            return RedirectToAction(nameof(Details), new { id = productId });
        }

        // NOTE: Have to name the parameter in this function the same as the key in the anonymous object on the Razor page
        public async Task<IActionResult> GetProductListForCategory( int productTypeId)
        {
            //var productList = await _context.Product
            //    .Include(p => p.ProductType)
            //    .Where(p => p.ProductTypeId == productTypeId && p.Active == true)
            //    .ToListAsync();

            var productGroup = await _context
                .ProductType
                    .OrderBy(pt => pt.Label)
                    .Where(pt => pt.ProductTypeId == productTypeId)
                    .Select(pt => new GroupedProducts
                    {
                        TypeId = pt.ProductTypeId,
                        TypeName = pt.Label,
                        ProductCount = pt.Products.Where(p => p.Active == true).Count(),
                        ProductsList = pt.Products.OrderByDescending(p => p.DateCreated).Where(p => p.Active == true).ToList()
                    }).SingleOrDefaultAsync();

            return View(productGroup);
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Product.Include(p => p.ProductType).Include(p => p.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Products/Details/5
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

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new ProductCreateViewModel()
            {
                ProductTypes = await _context.ProductType.OrderBy(pt => pt.Label).ToListAsync()
            };
            return View(viewModel);
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel viewModel)
        {
            ModelState.Remove("Product.UserId");
            ModelState.Remove("Product.User");
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                viewModel.Product.UserId = user.Id;
                viewModel.Product.Active = true;

                _context.Add(viewModel.Product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Types));
            }
            return View(viewModel);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", product.ProductTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", product.UserId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,DateCreated,Description,Title,Price,Quantity,UserId,City,ImagePath,Active,ProductTypeId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", product.ProductTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", product.UserId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }

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
    }
}

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
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Bangazon.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;


        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
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

            var successMsg = TempData["SuccessMessage"] as string;
            TempData["SuccessMessage"] = "This product has been added to your shopping cart";

            return RedirectToAction(nameof(Details), new { id = productId });
        }

        // NOTE: Have to name the parameter in this function the same as the key in the anonymous object on the Razor page
        public async Task<IActionResult> GetProductListForCategory(int productTypeId)
        {
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
            // When the process of creating a product fails due to ModelState validation or regex matching, the app needs to get the product categories again, so a newViewModel variable was needed inside this method.
            var newViewModel = new ProductCreateViewModel()
            {
                ProductTypes = GetProductCategories()
            };

            ModelState.Remove("Product.UserId");
            ModelState.Remove("Product.User");
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                viewModel.Product.UserId = user.Id;
                viewModel.Product.Active = true;
                _context.Add(viewModel.Product);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyProducts));
            }
            return View(newViewModel);
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

            ModelState.Remove("UserId");
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await GetCurrentUserAsync();
                    product.UserId = user.Id;
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
                return RedirectToAction(nameof(MyProducts));
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
            product.Active = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyProducts));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }

        // Seller Methods
        // GET: Products
        public async Task<IActionResult> MyProducts()
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    var user = await GetCurrentUserAsync();

                    cmd.CommandText = @"
                                                    SELECT p.ProductId, p.Title, p.Quantity, COUNT(op.OrderProductId) AS CountOrders
                                                    FROM Product p
                                                    INNER JOIN OrderProduct op ON p.ProductId = op.ProductId
                                                    INNER JOIN[Order] o ON op.OrderId = o.OrderId
                                                    WHERE(p.Active = 1 AND o.PaymentTypeId Is Not Null AND p.UserId = @userId)
                                                    GROUP BY p.ProductId, p.Title, p.Quantity";
                    cmd.Parameters.Add(new SqlParameter("@userId", user.Id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Product> products = new List<Product>();

                    while (reader.Read())
                    {
                        var newProduct = new Product
                        {
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            ProductsSold = reader.GetInt32(reader.GetOrdinal("CountOrders"))
                            // Add in other display properties
                        };
                        products.Add(newProduct);
                    }
                    return View(products);

                }
            }



                //return View(await applicationDbContext.ToListAsync());
        }

        private List<ProductType> GetProductCategories()
        {
            var productTypes = _context.ProductType.OrderBy(pt => pt.Label).ToList();
            return productTypes;
        }
    }
}

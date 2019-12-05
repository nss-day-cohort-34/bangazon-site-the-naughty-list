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
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Bangazon.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;


        public ReportsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration config)
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
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    var user = await GetCurrentUserAsync();

                    cmd.CommandText = @"
                                        select ProductTypeId, Label, count(OrderId) as [IncompleteOrderCount]
                                        from(select p.ProductTypeId, pt.Label, o.OrderId
                                                from[Order] o
                                                inner join OrderProduct op on op.OrderId = o.OrderId
                                                inner join Product p on p.ProductId = op.ProductId
                                                inner join ProductType pt on pt.ProductTypeId = p.ProductTypeId
                                                where o.PaymentTypeId IS NULL and p.UserId = @userId
                                                group by o.OrderId, p.ProductTypeId, pt.Label
                                                     ) oo
                                        group by ProductTypeId, Label
                                        order by ProductTypeId
                                      ";
                    cmd.Parameters.Add(new SqlParameter("@userId", user.Id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    
                    var model = new AbandonedProductTypesReportViewModel();
                    
                    while (reader.Read())
                    {
                        var newProductType = new ProductType
                        {
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            Label = reader.GetString(reader.GetOrdinal("Label"))
                        };

                        var newProductTypeOrderCount = new ProductTypeOrderCount
                        {
                            ProductType = newProductType,
                            IncompleteOrderCount = reader.GetInt32(reader.GetOrdinal("IncompleteOrderCount"))
                        };
                        model.IncompleteOrderCounts.Add(newProductTypeOrderCount);
                    }
                    return View(model);
                } 
            }
        }
    }
}
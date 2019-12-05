using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.ProductViewModels
{
    public class MyProductsViewModel
    {
        public Product Product { get; set; }

        public List<Product> Products { get; set; }
        public Order Order { get; set; }

        public List<Order> Orders { get; set; }
        public OrderProduct OrderProduct { get; set; }
        //public int ProductsSold
        //{

            // get total number of available products - Product.Quantity
            // get number of products on completed orders


        //    get
        //    {
        //        return Product.Quantity - 
        //    }
        //}


    }
}

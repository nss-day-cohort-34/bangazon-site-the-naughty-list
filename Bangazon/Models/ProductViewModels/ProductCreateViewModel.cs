using Bangazon.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.ProductViewModels
{
    public class ProductCreateViewModel
    {
        public Product Product { get; set; }
        public List<ProductType> ProductTypes { get; set; } = new List<ProductType>();
        public List<SelectListItem> ProductTypeOptions
        {
            get
            {
                if (ProductTypes == null) return null;

                List<SelectListItem> selectItems = ProductTypes
                    .Select(pt => new SelectListItem(pt.Label, pt.ProductTypeId.ToString()))
                    .ToList();
                selectItems.Insert(0, new SelectListItem
                {
                    Text = "Choose category...",
                    Value = ""
                });

                return selectItems;
            }
        }
    }
}

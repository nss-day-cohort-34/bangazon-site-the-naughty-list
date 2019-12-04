using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.ReportViewModels
{
    public class ProductTypeOrderCount
    {
        public ProductType ProductType { get; set; }
        public int IncompleteOrderCount { get; set; }
    }
}

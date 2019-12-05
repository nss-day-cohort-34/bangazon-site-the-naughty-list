using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.ReportViewModels
{
    public class ProductTypeOrderCount
    {
        public ProductType ProductType { get; set; }
        [Display(Name ="Incomplete Orders")]
        public int IncompleteOrderCount { get; set; }
    }
}

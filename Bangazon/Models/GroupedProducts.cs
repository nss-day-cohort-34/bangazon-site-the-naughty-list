using Bangazon.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bangazon.Models
{
    public class GroupedProducts
    {
        public int TypeId { get; set; }
        [Display(Name = "Category")]
        public string TypeName { get; set; }
        public int ProductCount { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public List<Product> ProductsList { get; set; }
    }
}
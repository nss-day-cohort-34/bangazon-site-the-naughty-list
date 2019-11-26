using System.Collections.Generic;
using Bangazon.Models;
using Bangazon.Data;

namespace Bangazon.Models.ProductViewModels
{
  public class ProductListViewModel
  {
    public IEnumerable<Product> Products { get; set; }
  }
}
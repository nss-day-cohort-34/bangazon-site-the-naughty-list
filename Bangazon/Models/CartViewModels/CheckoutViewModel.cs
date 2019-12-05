using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.CartViewModels
{
    public class CheckoutViewModel
    {
        public Order Order { get; set; }
        public List<PaymentType> PaymentTypes { get; set; } = new List<PaymentType>();
        public List<SelectListItem> PaymentTypeOptions
        {
            get
            {
                if (PaymentTypes == null) return null;

                List<SelectListItem> selectItems = PaymentTypes
                    .Select(pt => new SelectListItem($"{pt.Description} ({pt.AccountNumber})", pt.PaymentTypeId.ToString()))
                    .ToList();
                selectItems.Insert(0, new SelectListItem
                {
                    Text = "Choose payment type...",
                    Value = ""
                });

                return selectItems;
            }
        }
    }
}

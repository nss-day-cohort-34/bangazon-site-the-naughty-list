﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.ReportViewModels
{
    public class AbandonedProductTypesReportViewModel
    {
        public List<ProductTypeOrderCount> IncompleteOrderCounts { get; set; } = new List<ProductTypeOrderCount>();
    }
}

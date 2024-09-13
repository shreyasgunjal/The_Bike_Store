using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMVC.Models
{
    public class ProductsWithMinStock
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int MinimumStock { get; set; }
    }
}
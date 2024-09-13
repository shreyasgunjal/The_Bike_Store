using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMVC.Models
{
    public class ProductsByStore
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public int NumberOfProductsSold { get; set; }
    }
}
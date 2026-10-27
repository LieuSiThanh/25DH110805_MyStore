using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _25DH110805_MyStore.Models.ViewModel
{
    public class ProductSearchVM
    {
        public string searchTerm { get; set; }
        public List<Product> Products { get; set; }
    }
}
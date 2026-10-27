using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using _25DH110805_MyStore.Models;

namespace _25DH110805_MyStore.Models.ViewModel
{
    public class ProductSearchVM
    {
        public string SearchTerm { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public string SortOrder { get; set; }

        public IPagedList<Product> Products { get; set; }
    }
}
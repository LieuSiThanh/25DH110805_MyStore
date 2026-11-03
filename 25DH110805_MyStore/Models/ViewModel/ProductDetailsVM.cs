using _25DH110805_MyStore.Models;
using _25DH110805_MyStore.Models;
using PagedList;
using System.Collections.Generic;

namespace _25DH110805_MyStore.Models.ViewModel
{
    public class ProductDetailsVM
    {
        public Product product { get; set; }
        public int quantity { get; set; } = 1;

        public decimal estimatedValue { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 3;

        public List<Product> RelatedProducts { get; set; }

        public IPagedList<Product> TopProducts { get; set; }
    }
}
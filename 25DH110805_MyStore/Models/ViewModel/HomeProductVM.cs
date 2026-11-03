using _25DH110805_MyStore.Models;
using _25DH110805_MyStore.Models;
using PagedList;
using System.Collections.Generic;

namespace _25DH110805_MyStore.Models.ViewModel
{
    public class HomeProductVM
    {
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 6;

        public List<Product> FeaturedProducts { get; set; }
        public IPagedList<Product> NewProducts { get; set; }
    }
}
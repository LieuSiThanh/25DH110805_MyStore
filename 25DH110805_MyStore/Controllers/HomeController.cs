using _25DH110805_MyStore.Models;
using _25DH110805_MyStore.Models;
using _25DH110805_MyStore.Models.ViewModel;
using PagedList;
using System.Linq;
using System.Web.Mvc;

namespace _25DH110805_MyStore.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index(string searchTerm, int? page)
        {
            var model = new HomeProductVM();
            var products = db.Product.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p => p.ProductName.Contains(searchTerm) ||
                                               p.ProductDescription.Contains(searchTerm) ||
                                               p.Category.CategoryName.Contains(searchTerm));
            }

            int pageNumber = page ?? 1;
            int pageSize = 6;

            model.FeaturedProducts = products.OrderByDescending(p => p.OrderDetail.Count()).Take(10).ToList();

            model.NewProducts = products.OrderBy(p => p.OrderDetail.Count()).ToPagedList(pageNumber, pageSize);

            return View(model);
        }
        public ActionResult ProductDetail(int? id, int? quantity, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Product pro = db.Product.Find(id);
            if (pro == null)
            {
                return HttpNotFound();
            }

            var products = db.Product.Where(p => p.CategoryID == pro.CategoryID && p.ProductID != pro.ProductID).AsQueryable();

            ProductDetailsVM model = new ProductDetailsVM();
            model.product = pro;

            int pageNumber = page ?? 1;
            int pageSize = model.PageSize;

            model.RelatedProducts = products.OrderBy(p => p.ProductID).Take(8).ToList();

            model.TopProducts = products.OrderByDescending(p => p.OrderDetail.Count())
                                        .Take(8)
                                        .ToPagedList(pageNumber, pageSize);

            if (quantity.HasValue)
            {
                model.quantity = quantity.Value;
            }

            model.estimatedValue = model.quantity * pro.ProductPrice;

            return View(model);
        }
    }
}
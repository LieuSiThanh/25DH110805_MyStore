using _25DH110805_MyStore.Models;
using _25DH110805_MyStore.Models.ViewModel;
using _25DH110805_MyStore.Models;
using _25DH110805_MyStore.Models.ViewModel;
using System.Linq;
using System.Web.Mvc;

namespace _25DH110805_MyStore.Controllers
{
    public class CartController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        public ActionResult Index()
        {
            var cart = GetCartService().GetCart();
            return View(cart);
        }

        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var product = db.Product.Find(id);
            if (product != null)
            {
                var cart = GetCartService().GetCart();
                cart.AddItem(product.ProductID, product.ProductImage, product.ProductName, product.ProductPrice, quantity);
            }
            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromCart(int id)
        {
            var cart = GetCartService().GetCart();
            cart.RemoveItem(id);
            return RedirectToAction("Index");
        }

        public ActionResult ClearCart()
        {
            GetCartService().ClearCart();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCartService().GetCart();
            cart.UpdateQuantity(id, quantity);
            return RedirectToAction("Index");
        }
    }
}
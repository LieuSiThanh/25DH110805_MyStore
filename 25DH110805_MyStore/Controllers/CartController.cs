using _25DH110805_MyStore.Models;
using _25DH110805_MyStore.Models.ViewModel;
using System.Linq;
using System.Web.Mvc;

namespace _25DH110805_MyStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        private int GetCurrentCustomerID()
        {
            if (Session["CustomerID"] != null)
            {
                return int.Parse(Session["CustomerID"].ToString());
            }

            var currentUsername = User.Identity.Name;
            if (!string.IsNullOrEmpty(currentUsername))
            {
                var customer = db.Customer.AsEnumerable().FirstOrDefault(c =>
                    c.Username != null &&
                    c.Username.Trim().Equals(currentUsername.Trim(), System.StringComparison.OrdinalIgnoreCase));
                if (customer != null)
                {
                    Session["CustomerID"] = customer.CustomerID;
                    return customer.CustomerID;
                }
            }

            return 0;
        }

        public ActionResult Index()
        {
            int customerId = GetCurrentCustomerID();
            if (customerId == 0) return RedirectToAction("Login", "Account");

            var cartItems = db.Cart
                .Where(c => c.CustomerID == customerId)
                .Select(c => new CartItem
                {
                    ProductID = (int)c.ProductID,
                    ProductName = c.Product.ProductName,
                    ProductImage = c.Product.ProductImage,
                    UnitPrice = c.UnitPrice,
                    Quantity = (int)c.Quantity
                }).ToList();

            return View(cartItems);
        }

        public ActionResult AddToCart(int id, int quantity = 1)
        {
            int customerId = GetCurrentCustomerID();
            if (customerId == 0) return RedirectToAction("Login", "Account");

            var product = db.Product.Find(id);
            if (product != null)
            {
                var cartItem = db.Cart.SingleOrDefault(c => c.CustomerID == customerId && c.ProductID == id);

                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                }
                else
                {
                    var newCartItem = new _25DH110805_MyStore.Models.Cart
                    {
                        CustomerID = customerId,
                        ProductID = id,
                        Quantity = quantity,
                        UnitPrice = product.ProductPrice
                    };
                    db.Cart.Add(newCartItem);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromCart(int id)
        {
            int customerId = GetCurrentCustomerID();
            var cartItem = db.Cart.SingleOrDefault(c => c.CustomerID == customerId && c.ProductID == id);
            if (cartItem != null)
            {
                db.Cart.Remove(cartItem);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult ClearCart()
        {
            int customerId = GetCurrentCustomerID();
            var cartItems = db.Cart.Where(c => c.CustomerID == customerId).ToList();
            db.Cart.RemoveRange(cartItems);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            int customerId = GetCurrentCustomerID();
            var cartItem = db.Cart.SingleOrDefault(c => c.CustomerID == customerId && c.ProductID == id);
            if (cartItem != null && quantity > 0)
            {
                cartItem.Quantity = quantity;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
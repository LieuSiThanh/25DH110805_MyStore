using _25DH110805_MyStore.Models;
using _25DH110805_MyStore.Models.ViewModel;
using System.Linq;
using System.Web.Mvc;

namespace _25DH110805_MyStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
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

        public ActionResult MyOrder()
        {
            int customerId = GetCurrentCustomerID();
            if (customerId == 0) return RedirectToAction("Login", "Customer");

            var orders = db.Order
                           .Where(o => o.CustomerID == customerId)
                           .OrderByDescending(o => o.OrderDate)
                           .ToList();

            return View(orders);
        }

        public ActionResult Checkout()
        {
            int customerId = GetCurrentCustomerID();
            if (customerId == 0) return RedirectToAction("Login", "Customer");

            var cartItems = db.Cart.Where(c => c.CustomerID == customerId)
                .Select(c => new CartItem
                {
                    ProductID = (int)c.ProductID,
                    ProductName = c.Product.ProductName,
                    ProductImage = c.Product.ProductImage,
                    UnitPrice = c.UnitPrice,
                    Quantity = (int)c.Quantity
                }).ToList();

            if (!cartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var customer = db.Customer.Find(customerId);

            var viewModel = new CheckoutVM
            {
                CustomerID = customerId,
                Username = customer != null ? customer.Username : User.Identity.Name,
                CartItems = cartItems,
                ShippingAddress = customer != null ? customer.CustomerAddress : string.Empty,
                TotalAmount = cartItems.Sum(x => x.TotalPrice),
                OrderDate = System.DateTime.Now
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVM model)
        {
            int customerId = GetCurrentCustomerID();
            if (customerId == 0) return RedirectToAction("Login", "Customer");

            var cartItems = db.Cart.Where(c => c.CustomerID == customerId).ToList();
            if (!cartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var order = new Order
            {
                CustomerID = customerId,
                OrderDate = System.DateTime.Now,
                ShippingAddress = model.ShippingAddress,
                DeliveryMethod = model.ShippingMethod,
                PaymentMethod = model.PaymentMethod,
                TotalAmount = cartItems.Sum(c => c.Quantity * c.UnitPrice),
                PaymentStatus = "Chưa thanh toán"
            };

            db.Order.Add(order);
            db.SaveChanges();

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    OrderID = order.OrderID,
                    ProductID = (int)item.ProductID,
                    Quantity = (int)item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.Quantity * item.UnitPrice
                };
                db.OrderDetail.Add(orderDetail);
            }

            db.Cart.RemoveRange(cartItems);
            db.SaveChanges();

            return RedirectToAction("MyOrder", "Order");
        }
        public ActionResult Details(int id)
        {
            int customerId = GetCurrentCustomerID();
            if (customerId == 0) return RedirectToAction("Login", "Customer");

            var order = db.Order.FirstOrDefault(o => o.OrderID == id && o.CustomerID == customerId);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }
    }
}
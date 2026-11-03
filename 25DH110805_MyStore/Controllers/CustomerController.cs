using _25DH110805_MyStore.Models;
using _25DH110805_MyStore.Models; 
using System.Linq;
using System.Web.Mvc;

namespace _25DH110805_MyStore.Controllers
{
    public class CustomerController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "CustomerName,CustomerPhone,CustomerEmail,CustomerAddress,Username")] Customer customer, string password)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.User.FirstOrDefault(u => u.Username == customer.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại.");
                    return View(customer);
                }

                User newUser = new User
                {
                    Username = customer.Username,
                    Password = password,
                    UserRole = "Customer"
                };
                db.User.Add(newUser);

                db.Customer.Add(customer);

                db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(customer);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            var user = db.User.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                Session["Username"] = user.Username;
                Session["UserRole"] = user.UserRole;

 
                var customer = db.Customer.FirstOrDefault(c => c.Username == username);
                if (customer != null)
                {
                    Session["CustomerName"] = customer.CustomerName;
                    Session["CustomerID"] = customer.CustomerID;
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không chính xác.");
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Customer customer = db.Customer.Find(id);
            if (customer == null) return HttpNotFound();
            return View(customer);
        }
    }
}
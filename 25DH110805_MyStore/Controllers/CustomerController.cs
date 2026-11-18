using _25DH110805_MyStore.Models;
using _25DH110805_MyStore.Models.ViewModel;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

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
        public ActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.User.FirstOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại!");
                    return View(model);
                }

                var newUser = new User
                {
                    Username = model.Username,
                    Password = model.Password,
                    UserRole = "C"
                };
                db.User.Add(newUser);

                var customer = new Customer
                {
                    Username = model.Username,
                    CustomerName = model.CustomerName,
                    CustomerPhone = model.CustomerPhone,
                    CustomerEmail = model.CustomerEmail,
                    CustomerAddress = model.CustomerAddress
                };
                db.Customer.Add(customer);

                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine($"- Property: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                        }
                    }
                    throw; 
                }
                return RedirectToAction("Login");
            }

            return View(model);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.");
                return View();
            }

            // Dùng AsEnumerable kết hợp Trim() để loại bỏ khoảng trắng thừa gây lệch dữ liệu
            var user = db.User.AsEnumerable().FirstOrDefault(u =>
                u.Username != null &&
                u.Username.Trim().Equals(username.Trim(), System.StringComparison.OrdinalIgnoreCase) &&
                u.Password != null &&
                u.Password.Trim().Equals(password.Trim())
            );

            if (user != null)
            {
                Session["Username"] = user.Username;
                Session["UserRole"] = user.UserRole;

                var customer = db.Customer.AsEnumerable().FirstOrDefault(c =>
                    c.Username != null &&
                    c.Username.Trim().Equals(user.Username.Trim(), System.StringComparison.OrdinalIgnoreCase)
                );

                if (customer != null)
                {
                    Session["CustomerName"] = customer.CustomerName;
                    Session["CustomerID"] = customer.CustomerID;
                }

                FormsAuthentication.SetAuthCookie(user.Username, false);

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
            FormsAuthentication.SignOut();
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
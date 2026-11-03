using _25DH110805_MyStore.Models;
using _25DH110805_MyStore.Models; 
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web.Helpers;
using System.Web.Mvc;

namespace _25DH110805_MyStore.Areas.Admin.Controllers
{
    public class CustomersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index(string searchString)
        {
            var customers = db.Customer.Include(c => c.User).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(c => c.CustomerName.Contains(searchString) || c.CustomerEmail.Contains(searchString));
            }

            ViewBag.CurrentFilter = searchString;
            return View(customers.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Customer customer = db.Customer.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using _25DH110805_MyStore.Models;
using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
namespace _25DH110805_MyStore.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index(string searchCustomer, string searchStatus, DateTime? searchDate, int? page)
        {
            var orders = db.Order.Include("Customer").AsQueryable();

            if (!String.IsNullOrEmpty(searchCustomer))
            {
                orders = orders.Where(o => o.Customer.CustomerName.Contains(searchCustomer));
            }

            if (!String.IsNullOrEmpty(searchStatus))
            {
                orders = orders.Where(o => o.PaymentStatus == searchStatus);
            }

            if (searchDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate.Year == searchDate.Value.Year &&
                                           o.OrderDate.Month == searchDate.Value.Month &&
                                           o.OrderDate.Day == searchDate.Value.Day);
            }

            orders = orders.OrderByDescending(o => o.OrderDate);

            int pageSize = 15;
            int pageNumber = (page ?? 1);

            ViewBag.CurrentCustomer = searchCustomer;
            ViewBag.CurrentStatus = searchStatus;
            ViewBag.CurrentDate = searchDate.HasValue ? searchDate.Value.ToString("yyyy-MM-dd") : "";

            return View(orders.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult DetailedOrder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = db.Order.Include("Customer").FirstOrDefault(o => o.OrderID == id);
            if (order == null)
            {
                return HttpNotFound();
            }

            var orderDetails = db.OrderDetail.Include("Product").Where(od => od.OrderID == id).ToList();
            ViewBag.OrderDetails = orderDetails;

            return View(order);
        }
    }
}
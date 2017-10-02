using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyAspNetMvcApp.Models.OrderApp;
using MyAspNetMvcApp.Models;

namespace MyAspNetMvcApp.Controllers
{
    public class Cart1Controller : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Cart1
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddToCart(int quantity, int productId)
        {
            Order myOrder;
            var myItem = new OrderItem();

            Order currentOrder = db.Orders.Where(x => x.UserName == User.Identity.Name && x.Status == -1).FirstOrDefault();

            if(currentOrder == null)
            {
                myOrder = new Order();
                myOrder.OrderDate = DateTime.Now;
                myOrder.Status = -1;
                myOrder.UserName = User.Identity.Name;
                myItem.OrderId = myOrder.Id;
                db.Orders.Add(myOrder);
            }
            else
            {
                myItem.OrderId = currentOrder.Id;
            }

            myItem.ProductId = productId;
            myItem.Quantity = quantity;

            db.OrderItems.Add(myItem);
            db.SaveChanges();

            return RedirectToAction("Index", "Products1");
        }
    }
}
using MyAspNetMvcApp.Models;
using MyAspNetMvcApp.Models.OrderApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyAspNetMvcApp.ViewModels;
using System.Data.Entity;

namespace MyAspNetMvcApp.Controllers
{
    public class CartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Cart
        public ActionResult Index()
        {
            OrderViewModel OrderVM = db.Orders
                .Where(x => x.UserName == User.Identity.Name && x.Status == -1)
                .Include(c => c.Customer).Include(u => u.Customer)
                .Select(s => new OrderViewModel
                {
                    OrderId = s.Id,
                    CustomerName = s.Customer.FirstName + " " + s.Customer.LastName
                }).FirstOrDefault();

            OrderVM.OrderItems = db.OrderItems.Where(x => x.OrderId == OrderVM.OrderId)
                .Include(p => p.Product)
                .Select(s => new OrderItemViewModel
                {
                    ProductName = s.Product.Name,
                    Quantity = s.Quantity,
                    Price = s.Product.Price,
                    Amount = s.Quantity * s.Product.Price
                }).ToList();

            return View();
        }

        public ActionResult Add()
        {
            return RedirectToAction("Index", "Products");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Add(int productId, int quantity)
        {
            Order myOrder = new Order();
            var myOrderItems = new OrderItem();

            var currentOrder = db.Orders.Where(x => x.UserName == User.Identity.Name && x.Status == -1).FirstOrDefault();
            if(currentOrder == null)
            {
                myOrder.UserName = User.Identity.Name;
                myOrder.OrderDate = DateTime.Now;
                myOrder.Status = -1;
                db.Orders.Add(myOrder);
                myOrderItems.OrderId = myOrder.Id;
            }
            else
            {
                myOrderItems.OrderId = currentOrder.Id;
            }

            myOrderItems.ProductId = productId;
            myOrderItems.Quantity = quantity;

            db.OrderItems.Add(myOrderItems);
            db.SaveChanges();

            TempData["MessageBox"] = "Added to cart.";
            return RedirectToAction("Index", "Products");
        }
    }
}
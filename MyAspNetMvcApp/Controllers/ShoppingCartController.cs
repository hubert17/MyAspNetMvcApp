using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyAspNetMvcApp.Models.OrderApp;
using MyAspNetMvcApp.Models;
using MyAspNetMvcApp.ViewModels;
using System.Data.Entity;

namespace MyAspNetMvcApp.Controllers
{
    public class ShoppingCartController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: ShoppingCart
        public ActionResult Index()
        {
            OrderViewModel OrderVM = db.Orders.Where(x => x.UserName == User.Identity.Name && x.Status == -1)
                .Include(c => c.Customer)
                .Include(i => i.Customer)
                .Select(s => new OrderViewModel
                {
                    OrderId = s.Id,
                    CustomerName = s.Customer.FirstName + " " + s.Customer.LastName,
                    OrderDate = s.OrderDate
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

            OrderVM.OrderTotal = OrderVM.OrderItems.Sum(t => t.Amount);

            return View(OrderVM);
        }

        [HttpPost]
        public ActionResult Add(int productId, int quantity)
        {
            Order currentOrder = db.Orders.Where(x => x.UserName == User.Identity.Name && x.Status == -1).FirstOrDefault();

            Order myOrders;
            var myItem = new OrderItem();

            if (currentOrder == null)
            {
                myOrders = new Order();
                myOrders.OrderDate = DateTime.Now;
                myOrders.UserName = User.Identity.Name;
                myOrders.Status = -1;
                myItem.OrderId = myOrders.Id;
                db.Orders.Add(myOrders);
            }
            else
            {
                myItem.OrderId = currentOrder.Id;
            }


            myItem.ProductId = productId;
            myItem.Quantity = quantity;

            db.OrderItems.Add(myItem);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
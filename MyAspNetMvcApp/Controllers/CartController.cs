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
        [Authorize]
        public ActionResult Index()
        {
            OrderViewModel OrderVM = db.Orders
                .Where(x => x.UserName == User.Identity.Name && x.Status == -1)
                .Select(s => new OrderViewModel
                {
                    OrderId = s.Id,
                    CustomerName = s.Customer.Profile.FirstName + " " + s.Customer.Profile.LastName
                }).FirstOrDefault();

            OrderVM.OrderItems = db.OrderItems.Where(x => x.OrderId == OrderVM.OrderId)
                .Select(s => new OrderItemViewModel
                {
                    OrderItemId = s.Id,
                    ProductName = s.Product.Name,
                    Quantity = s.Quantity,
                    Price = s.Product.Price,
                    Amount = s.Quantity * s.Product.Price
                }).ToList();

            OrderVM.OrderTotal = OrderVM.OrderItems.Sum(t => t.Amount);

            return View(OrderVM);
        }

        public ActionResult Add()
        {
            return RedirectToAction("Index", "Products");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Add(int productId, int quantity)
        {
            var myOrder = new Order();
            var myOrderItems = new OrderItem();

            var currentOrder = db.Orders.Where(x => x.UserName == User.Identity.Name && x.Status == -1).FirstOrDefault();
            if(currentOrder == null)
            {
                // Create new Order
                myOrder.UserName = User.Identity.Name; // Get current logged in User
                myOrder.OrderDate = DateTime.Now;
                myOrder.Status = -1; // Shopping status
                db.Orders.Add(myOrder);
                myOrderItems.OrderId = myOrder.Id;
            }
            else
            {
                // Get Id from existing "Shopping" Order
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
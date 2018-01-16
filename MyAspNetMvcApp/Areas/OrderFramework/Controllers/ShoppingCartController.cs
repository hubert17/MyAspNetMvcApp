using MyAspNetMvcApp.Areas.OrderFramework.ViewModels;
using MyAspNetMvcApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Areas.OrderFramework.Controllers
{
    public class ShoppingCartController : Controller
    {

        ApplicationDbContext storeDB = new ApplicationDbContext();
        //
        // GET: /ShoppingCart/
        public ActionResult Index()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            // Return the view
            return View(viewModel);
        }
        //
        // GET: /Store/AddToCart/5
        [HttpPost]
        public ActionResult AddToCart(int id)
        {
            // Retrieve the item from the database
            var addedItem = storeDB.OF_Products
                .Single(item => item.Id == id);

            // Add it to the shopping cart
            var cart = ShoppingCart.GetCart(this.HttpContext);
            cart.AddToCart(addedItem);

            // Send response for updated cart data
            var results = new ShoppingCartResponseViewModel
            {
                CartCount = cart.GetCount()
            };
            return Json(results);
        }
        //
        // AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Get the name of the item to display confirmation

            // Get the name of the album to display confirmation
            string itemName = storeDB.OF_Products
                .Single(item => item.Id == id).Name;

            // Remove from cart
            int itemQty = cart.RemoveFromCart(id);

            // Send response for updated cart data
            var results = new ShoppingCartResponseViewModel
            {
                ItemName = itemName,
                CartTotal = cart.GetTotal(),
                CartTotalFormatted = string.Format("{0:C}", cart.GetTotal()),
                CartCount = cart.GetCount(),
                ItemQty = itemQty,
                DeleteId = id
            };
            return Json(results);
        }
        //
        // AJAX: /ShoppingCart/UpdateQtyFromCart/5
        [HttpPost]
        public ActionResult UpdateQtyFromCart(int id, int newQty)
        {
            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Get the name of the item to display confirmation

            // Remove from cart
            int itemQty = cart.UpdateQtyFromCart(id, newQty);

            // Send response for updated cart data
            var results = new ShoppingCartResponseViewModel
            {
                CartTotal = cart.GetTotal(),
                CartTotalFormatted = string.Format("{0:C}", cart.GetTotal()),
                ItemQty = itemQty
            };

            return Json(results);
        }
        //
        // GET: /ShoppingCart/CartSummary
        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            ViewData["CartCount"] = cart.GetCount();
            return PartialView("CartSummary");
        }
    }
}
using MyAspNetMvcApp.Areas.OrderFramework.Models;
using MyAspNetMvcApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAspNetMvcApp.Areas.OrderFramework.ViewModels
{
    public partial class ShoppingCart
    {
        ApplicationDbContext storeDb = new ApplicationDbContext();
        string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";

        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        // Helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }

        public int AddToCart(Product item)
        {
            // Get the matching cart and item instances
            var cartItem = storeDb.OF_Carts.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.ItemId == item.Id);

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new Cart
                {
                    ItemId = item.Id,
                    CartId = ShoppingCartId,
                    Quantity = 1,
                    DateCreated = DateTime.Now
                };
                storeDb.OF_Carts.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, 
                // then add one to the quantity
                cartItem.Quantity++;
            }
            // Save changes
            storeDb.SaveChanges();

            return cartItem.Quantity;
        }

        public int RemoveFromCart(int id)
        {
            // Get the cart
            var cartItem = storeDb.OF_Carts.Single(
                cart => cart.CartId == ShoppingCartId
                && cart.ItemId == id);

            int itemQty = 0;

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                    itemQty = cartItem.Quantity;
                }
                else
                {
                    storeDb.OF_Carts.Remove(cartItem);
                }
                // Save changes
                storeDb.SaveChanges();
            }
            return itemQty;
        }

        public int UpdateQtyFromCart(int id, int newQty)
        {
            // Get the cart
            var cartItem = storeDb.OF_Carts.Single(
                cart => cart.CartId == ShoppingCartId
                && cart.ItemId == id);

            if (cartItem != null)
            {
                cartItem.Quantity = newQty;
                storeDb.Entry(cartItem).State = System.Data.Entity.EntityState.Modified;
                // Save changes
                storeDb.SaveChanges();
            }

            return cartItem.Quantity;
        }

        public void EmptyCart()
        {
            var cartItems = storeDb.OF_Carts.Where(
                cart => cart.CartId == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                storeDb.OF_Carts.Remove(cartItem);
            }
            // Save changes
            storeDb.SaveChanges();
        }

        public List<Cart> GetCartItems()
        {
            return storeDb.OF_Carts.Where(
                cart => cart.CartId == ShoppingCartId).ToList();
        }

        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            //int? count = (from cartItems in storeDb.OF_Carts
            //              where cartItems.CartId == ShoppingCartId
            //              select (int?)cartItems.Count).Sum();

            int? count = storeDb.OF_Carts.Where(x => x.CartId == ShoppingCartId).Count();

            // Return 0 if all entries are null
            return count ?? 0;
        }

        public decimal GetTotal()
        {
            // Multiply item price by count of that item to get 
            // the current price for each of those items in the cart
            // sum all item price totals to get the cart total

            decimal Total = decimal.Zero; 

            var cartItems = storeDb.OF_Carts.Where(x => x.CartId == ShoppingCartId);
            if (cartItems.Count() > 0)
                Total = storeDb.OF_Carts.Where(x => x.CartId == ShoppingCartId).ToList().Sum(t => t.Quantity * t.Item.UnitPrice);

            return Total;
        }

        public Order CreateOrder(Order order)
        {
            decimal orderTotal = 0;
            order.OrderDetails = new List<OrderDetail>();

            var cartItems = GetCartItems();
            // Iterate over the items in the cart, 
            // adding the order details for each
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ItemId = item.ItemId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Item.UnitPrice,
                    Quantity = item.Quantity
                };
                // Set the order total of the shopping cart
                orderTotal += (item.Quantity * item.Item.UnitPrice);
                order.OrderDetails.Add(orderDetail);
                storeDb.OF_OrderDetails.Add(orderDetail);

            }
            // Set the order's total to the orderTotal count
            order.OrderTotal = orderTotal;

            // Save the order
            storeDb.SaveChanges();
            // Empty the shopping cart
            EmptyCart();
            // Return the OrderId as the confirmation number
            return order;
        }

        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContextBase context)
        {
            string tempCartId;
            if (context.Request.IsAuthenticated) //if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
            {
                tempCartId = context.User.Identity.Name;
            }
            else
            {
                if (context.Session[CartSessionKey] == null)
                {
                    // Generate a new random GUID using System.Guid class
                    tempCartId = Guid.NewGuid().ToString();
                    // Send tempCartId back to client as a cookie
                    context.Session[CartSessionKey] = tempCartId;
                }
                else
                    tempCartId = context.Session[CartSessionKey].ToString();
            }

            return tempCartId;
        }

        // When a user has logged in, migrate their shopping cart to
        // be associated with their username
        public void MigrateCart(string userName)
        {
            var shoppingCart = storeDb.OF_Carts.Where(
                c => c.CartId == ShoppingCartId);

            foreach (Cart item in shoppingCart)
            {
                item.CartId = userName;
            }
            storeDb.SaveChanges();
        }
    }
}
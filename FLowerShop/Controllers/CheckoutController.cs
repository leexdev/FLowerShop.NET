using FLowerShop.Context;
using FLowerShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FLowerShop.Controllers
{
    public class CheckoutController : BaseController
    {
        private readonly FlowerShopEntities db;

        public CheckoutController()
        {
            db = new FlowerShopEntities();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            LoadCommonData();
            base.OnActionExecuting(filterContext);
        }

        [HttpPost]
        public ActionResult ExitCheckout()
        {
            Session.Remove("BuyFlower");
            return Json(new { success = true });
        }

        public ActionResult ExitCheckoutTest()
        {
            Session.Remove("BuyFlower");
            return Json(new { success = true });
        }

        private List<SHOPPINGCART> GetShoppingCartItems()
        {
            var shoppingCartItems = new List<SHOPPINGCART>();

            if (Session["BuyFlower"] != null && Session["BuyFlower"] is SHOPPINGCART)
            {
                shoppingCartItems.Add(Session["BuyFlower"] as SHOPPINGCART);
            }
            else if (Session["ShoppingCart"] != null && Session["ShoppingCart"] is List<SHOPPINGCART>)
            {
                shoppingCartItems = Session["ShoppingCart"] as List<SHOPPINGCART>;
            }

            return shoppingCartItems;
        }

        public ActionResult Index()
        {
            if (Session["BuyFlower"] == null && Session["ShoppingCart"] == null)
            {
                return RedirectToAction("Index", "ShoppingCart");
            }

            var flower = GetShoppingCartItems();

            var orderModel = new OrderModel
            {
                ShoppingCarts = flower,
            };

            return View(orderModel);
        }

        [HttpPost]
        public ActionResult Index(ORDER order, ORDERDETAIL orderDetail, USER user, string coupon)
        {
            if (ModelState.IsValid)
            {
                return View();
            }

            var flower = GetShoppingCartItems();

            var orderModel = new OrderModel
            {
                ShoppingCarts = flower,
            };

            return View(orderModel);
        }
    }
}
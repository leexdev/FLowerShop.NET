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

        public ActionResult Index()
        {
            if (Session["BuyFlower"] == null && Session["ShoppingCart"] == null)
            {
                return RedirectToAction("Index", "ShoppingCart");
            }

            var flower = new List<SHOPPINGCART>();

            if (Session["BuyFlower"] != null && Session["BuyFlower"] is SHOPPINGCART)
            {
                flower.Add(Session["BuyFlower"] as SHOPPINGCART);
                Session["FlowerCheckout"] = Session["BuyFlower"];
                Session.Remove("BuyFlower");
            }
            else if (Session["ShoppingCart"] != null && Session["ShoppingCart"] is List<SHOPPINGCART>)
            {
                flower = Session["ShoppingCart"] as List<SHOPPINGCART>;
            }

            var orderModel = new OrderModel
            {
                ShoppingCarts = flower,
            };

            return View(orderModel);
        }

        [HttpPost]
        public ActionResult AddOrder(ORDER order)
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
using FLowerShop.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FLowerShop.Controllers
{
    public class CheckoutController : BaseController
    {
        FlowerShopEntities db = new FlowerShopEntities();
        public ActionResult Index()
        {
            LoadCommonData();
            var ShoppingCarts = Session["ShoppingCart"] as List<SHOPPINGCART>;
            return View(ShoppingCarts);
        }
    }
}
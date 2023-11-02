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
            var ShoppingCarts = Session["ShoppingCart"] as List<SHOPPINGCART>;
            return View(ShoppingCarts);
        }
    }
}
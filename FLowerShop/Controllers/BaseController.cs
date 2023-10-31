using FLowerShop.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FLowerShop.Controllers
{
    public class BaseController : Controller
    {
        protected void LoadCommonData()
        {
            using (var db = new FlowerShopEntities())
            {
                ViewBag.lstFlowerTypes = db.FLOWERTYPES.ToList();
                ViewBag.lstFlowerCarts = Session["ShoppingCart"] as List<SHOPPINGCART>;
            }
        }
    }
}
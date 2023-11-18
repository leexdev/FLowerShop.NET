using FlowerShop.Context;
using FLowerShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlowerShop.Controllers
{
    public class HomeController : BaseController
    {
        private readonly FlowerShopEntities db;

        public HomeController()
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
            var flowerTypes = db.FLOWERTYPES.AsNoTracking().ToList();
            var flowers = db.FLOWERS.AsNoTracking().ToList();

            var objHomeModel = new HomeModel
            {
                FlowerTypes = flowerTypes,
                Flowers = flowers
            };

            return View(objHomeModel);
        }

    }

}
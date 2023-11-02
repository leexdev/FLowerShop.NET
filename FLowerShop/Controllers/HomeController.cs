using FLowerShop.Context;
using FLowerShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FLowerShop.Controllers
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
            HomeModel objHomeModel = new HomeModel();
            objHomeModel.FlowerTypes = db.FLOWERTYPES.ToList();
            objHomeModel.Flowers = db.FLOWERS.ToList();
            return View(objHomeModel);
        }
    }
}
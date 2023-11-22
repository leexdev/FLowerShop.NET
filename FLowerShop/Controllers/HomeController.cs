using FlowerShop.Context;
using FlowerShop.Models;
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
            var flowers = db.FLOWERS.AsNoTracking().Where(f => f.DELETED == false).ToList();

            return View(flowers);
        }

        public ActionResult Info()
        {
            return PartialView("_NotFound");
        }

        public ActionResult News()
        {
            return PartialView("_NotFound");
        }

        public ActionResult Contact()
        {
            return PartialView("_NotFound");
        }
    }

}
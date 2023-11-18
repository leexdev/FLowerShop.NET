using FlowerShop.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlowerShop.Controllers
{
    public class FlowerTypesController : BaseController
    {
        private readonly FlowerShopEntities db;

        public FlowerTypesController()
        {
            db = new FlowerShopEntities();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            LoadCommonData();
            base.OnActionExecuting(filterContext);
        }

        public ActionResult Index(Guid? flowerTypeId, string flowertypeName)
        {
            var flower = db.FLOWERS.AsNoTracking().Where(f => f.FLOWERTYPE_ID == flowerTypeId).ToList();
            ViewBag.FlowerTypeName = flowertypeName;
            return View(flower);
        }
    }
}
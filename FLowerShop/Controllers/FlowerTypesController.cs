using FLowerShop.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FLowerShop.Controllers
{
    public class FlowerTypesController : BaseController
    {
        private readonly FLowerShopEntities db;

        public FlowerTypesController()
        {
            db = new FLowerShopEntities();
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
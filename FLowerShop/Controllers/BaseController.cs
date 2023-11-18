using FlowerShop.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlowerShop.Controllers
{
    public class BaseController : Controller
    {
        protected void LoadCommonData()
        {
            using (var db = new FlowerShopEntities())
            {
                ViewBag.lstFlowerTypes = db.FLOWERTYPES.AsNoTracking().ToList();

                Guid? userId = Session["UserId"] as Guid?;
                if (userId != null)
                {
                    var shoppingCarts = db.SHOPPINGCARTs
                    .Include("FLOWER")
                    .AsNoTracking()
                    .Where(s => s.USER_ID == userId)
                    .ToList();

                    ViewBag.ShoppingCarts = shoppingCarts.ToList();
                }
            }
        }

        public string RenderToString(string partialViewName, object model)
        {
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, partialViewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, new ViewDataDictionary { Model = model }, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.ToString();
            }
        }
    }
}
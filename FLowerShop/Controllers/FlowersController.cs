using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FLowerShop.Context;
using FLowerShop.Models;

namespace FLowerShop.Controllers
{
    public class FlowersController : BaseController
    {
        FlowerShopEntities db = new FlowerShopEntities();

        public ActionResult Detail(Guid? flowerId)
        {
            LoadCommonData();
            if (flowerId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetailModel detailModel = new DetailModel();
            var flower = db.FLOWERS.Find(flowerId);
            if (flower == null)
            {
                return HttpNotFound();
            }
            detailModel.Flower = flower;
            detailModel.Flowers = db.FLOWERS.Where(f => f.FLOWERTYPE_ID == flower.FLOWERTYPE_ID && f.FLOWER_ID != flowerId).ToList();
            detailModel.DiscountCodes = db.DISCOUNTCODES.ToList();
            return View(detailModel);
        }
    }
}

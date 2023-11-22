using FlowerShop.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlowerShop.Areas.Admin.Controllers
{
    public class FlowersController : Controller
    {
        private readonly FlowerShopEntities db;

        public FlowersController()
        {
            db = new FlowerShopEntities();
        }

        public ActionResult Index()
        {
            var flower = db.FLOWERS.AsNoTracking().Where(f => f.DELETED == false).ToList();
            return View(flower);
        }

        public ActionResult Detail(Guid? flowerId)
        {
            var flower = db.FLOWERS.Where(f => f.FLOWER_ID == flowerId).FirstOrDefault();
            return View(flower);
        }

        public ActionResult Edit(Guid? flowerId)
        {
            var flower = db.FLOWERS.Where(f => f.FLOWER_ID == flowerId).FirstOrDefault();

            var flowerTypes = db.FLOWERTYPES.Where(f => f.DELETED == false).ToList();

            SelectList flowerTypeList = new SelectList(flowerTypes, "FLOWERTYPE_ID", "FLOWERTYPE_NAME");
            ViewBag.FlowerTypes = flowerTypeList;
            return View(flower);
        }

        [HttpPost]
        public ActionResult Edit(FLOWER flower)
        {
            if (!ModelState.IsValid)
            {
                return View(flower);
            }

            var existingFlower = db.FLOWERS.Where(f => f.FLOWER_ID == flower.FLOWER_ID).FirstOrDefault();

            if (existingFlower != null)
            {
                existingFlower.FLOWER_NAME = flower.FLOWER_NAME;
                existingFlower.FLOWER_IMAGE = flower.FLOWER_IMAGE;
                existingFlower.DESCRIPTION = flower.DESCRIPTION;
                existingFlower.OLD_PRICE = flower.OLD_PRICE;
                existingFlower.NEW_PRICE = flower.NEW_PRICE;
                existingFlower.FLOWERTYPE_ID = flower.FLOWERTYPE_ID;
                db.SaveChanges();
                return View("Index");
            }

            return PartialView("_NotFound");
        }
    }
}
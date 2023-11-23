﻿using FlowerShop.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

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
            var flower = db.FLOWERS.AsNoTracking().Where(f => f.FLOWER_ID == flowerId).FirstOrDefault();

            if (flower != null)
            {
                return View(flower);
            }
            return PartialView("_NotFound");
        }

        public ActionResult Add()
        {
            var flowerTypes = db.FLOWERTYPES.AsNoTracking().Where(f => f.DELETED == false).ToList();

            SelectList flowerTypeList = new SelectList(flowerTypes, "FLOWERTYPE_ID", "FLOWERTYPE_NAME");
            ViewBag.FlowerTypes = flowerTypeList;
            return View();
        }

        [HttpPost]
        public ActionResult Add(FLOWER flower, HttpPostedFileBase ImageUpLoad)
        {
            ModelState.Remove("FLOWER_IMAGE");
            if (!ModelState.IsValid)
            {
                var flowerTypes = db.FLOWERTYPES.AsNoTracking().Where(f => f.DELETED == false).ToList();

                SelectList flowerTypeList = new SelectList(flowerTypes, "FLOWERTYPE_ID", "FLOWERTYPE_NAME");
                ViewBag.FlowerTypes = flowerTypeList;
                return View(flower);
            }

            if (ImageUpLoad != null)
            {
                var folderPath = Server.MapPath("~/Content/assets/img/product/");

                var currentTime = DateTime.Now;

                var originalFileName = Path.GetFileName(ImageUpLoad.FileName);

                var fileName = $"{currentTime:yyyyMMddHHmmss}_{originalFileName}";

                var path = Path.Combine(folderPath, fileName);

                ImageUpLoad.SaveAs(path);

                flower.FLOWER_IMAGE = fileName;
            }
            flower.FLOWER_ID = Guid.NewGuid();

            db.FLOWERS.Add(flower);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(Guid? flowerId)
        {
            var flower = db.FLOWERS.Where(f => f.FLOWER_ID == flowerId).FirstOrDefault();

            if (flower != null)
            {
                var flowerTypes = db.FLOWERTYPES.AsNoTracking().Where(f => f.DELETED == false).ToList();

                SelectList flowerTypeList = new SelectList(flowerTypes, "FLOWERTYPE_ID", "FLOWERTYPE_NAME");
                ViewBag.FlowerTypes = flowerTypeList;
                return View(flower);
            }
            return PartialView("_NotFound");
        }

        [HttpPost]
        public ActionResult Edit(FLOWER flower, HttpPostedFileBase ImageUpLoad)
        {
            if (!ModelState.IsValid)
            {
                var flowerTypes = db.FLOWERTYPES.AsNoTracking().Where(f => f.DELETED == false).ToList();

                SelectList flowerTypeList = new SelectList(flowerTypes, "FLOWERTYPE_ID", "FLOWERTYPE_NAME");
                ViewBag.FlowerTypes = flowerTypeList;
                return View(flower);
            }

            if (ImageUpLoad != null)
            {
                var folderPath = Server.MapPath("~/Content/assets/img/product/");

                var currentTime = DateTime.Now;

                var originalFileName = Path.GetFileName(ImageUpLoad.FileName);

                var fileName = $"{currentTime:yyyyMMddHHmmss}_{originalFileName}";

                var path = Path.Combine(folderPath, fileName);

                ImageUpLoad.SaveAs(path);

                flower.FLOWER_IMAGE = fileName;
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
                return RedirectToAction("Index");
            }

            return PartialView("_NotFound");
        }

        [HttpPost]
        public ActionResult Delete()
        {
            return RedirectToAction("Index");
        }
    }
}
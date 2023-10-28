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

        public ActionResult Detail(Guid? flower_id)
        {
            LoadCommonData();
            if (flower_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetailModel detailModel = new DetailModel();
            var flower = db.FLOWERS.Find(flower_id);
            if (flower == null)
            {
                return HttpNotFound();
            }
            detailModel.Flower = flower;
            detailModel.Flowers = db.FLOWERS.Where(f => f.FLOWERTYPE_ID == flower.FLOWERTYPE_ID && f.FLOWER_ID != flower_id).ToList();
            detailModel.DiscountCodes = db.DISCOUNTCODES.ToList();
            return View(detailModel);
        }

        public ActionResult Create()
        {
            ViewBag.FLOWERTYPE_ID = new SelectList(db.FLOWERTYPES, "FLOWERTYPE_ID", "FLOWERTYPE_NAME");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FLOWER_ID,FLOWER_NAME,FLOWER_IMAGE,DESCRIPTION,OLD_PRICE,NEW_PRICE,DELETED,FLOWERTYPE_ID")] FLOWER fLOWER)
        {
            if (ModelState.IsValid)
            {
                fLOWER.FLOWER_ID = Guid.NewGuid();
                db.FLOWERS.Add(fLOWER);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FLOWERTYPE_ID = new SelectList(db.FLOWERTYPES, "FLOWERTYPE_ID", "FLOWERTYPE_NAME", fLOWER.FLOWERTYPE_ID);
            return View(fLOWER);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FLOWER fLOWER = db.FLOWERS.Find(id);
            if (fLOWER == null)
            {
                return HttpNotFound();
            }
            ViewBag.FLOWERTYPE_ID = new SelectList(db.FLOWERTYPES, "FLOWERTYPE_ID", "FLOWERTYPE_NAME", fLOWER.FLOWERTYPE_ID);
            return View(fLOWER);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FLOWER_ID,FLOWER_NAME,FLOWER_IMAGE,DESCRIPTION,OLD_PRICE,NEW_PRICE,DELETED,FLOWERTYPE_ID")] FLOWER fLOWER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fLOWER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FLOWERTYPE_ID = new SelectList(db.FLOWERTYPES, "FLOWERTYPE_ID", "FLOWERTYPE_NAME", fLOWER.FLOWERTYPE_ID);
            return View(fLOWER);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FLOWER fLOWER = db.FLOWERS.Find(id);
            if (fLOWER == null)
            {
                return HttpNotFound();
            }
            return View(fLOWER);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            FLOWER fLOWER = db.FLOWERS.Find(id);
            db.FLOWERS.Remove(fLOWER);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

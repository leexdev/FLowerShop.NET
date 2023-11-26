using FlowerShop.Context;
using FlowerShop.Controllers;
using FlowerShop.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FlowerShop.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "1")]
    public class FlowerTypesController : Controller
    {
        private readonly FlowerShopEntities db;

        public FlowerTypesController()
        {
            db = new FlowerShopEntities();
        }

        public ActionResult Index()
        {
            var flowerTypes = db.FLOWERTYPES.AsNoTracking().Where(f => f.DELETED == false).ToList();
            ViewBag.SuccessFully = TempData["SuccessFully"];
            return View(flowerTypes);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(FLOWERTYPE flowerType)
        {
            if (!ModelState.IsValid)
            {
                return View(flowerType);
            }

            var formattedFlowerType = char.ToUpper(flowerType.FLOWERTYPE_NAME[0]) + flowerType.FLOWERTYPE_NAME.Substring(1).ToLower();

            var existingFlowerTypes = db.FLOWERTYPES.AsNoTracking().Where(f => f.FLOWERTYPE_NAME == formattedFlowerType && f.DELETED == false).FirstOrDefault();

            if (existingFlowerTypes != null)
            {
                ModelState.AddModelError("FLOWERTYPE_NAME", "Tên danh mục đã tồn tại");
                return View(flowerType);
            }

            flowerType.FLOWERTYPE_ID = Guid.NewGuid();
            flowerType.FLOWERTYPE_NAME = formattedFlowerType;
            db.FLOWERTYPES.Add(flowerType);
            db.SaveChanges();

            TempData["SuccessFully"] = "Thêm thành công!";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(Guid? flowerTypeId)
        {
            var flowerType = db.FLOWERTYPES.AsNoTracking().Where(f => f.FLOWERTYPE_ID == flowerTypeId && f.DELETED == false).FirstOrDefault();

            if (flowerType != null)
            {
                return View(flowerType);
            }
            return PartialView("_NotFound");
        }

        [HttpPost]
        public ActionResult Edit(FLOWERTYPE flowerType)
        {
            if (!ModelState.IsValid)
            {
                return View(flowerType);
            }

            var formattedFlowerType = char.ToUpper(flowerType.FLOWERTYPE_NAME[0]) + flowerType.FLOWERTYPE_NAME.Substring(1).ToLower();

            var existingFlowerType = db.FLOWERTYPES.AsNoTracking()
                .Where(f => f.FLOWERTYPE_NAME == formattedFlowerType && f.FLOWERTYPE_ID != flowerType.FLOWERTYPE_ID && f.DELETED == false)
                .FirstOrDefault();


            if (existingFlowerType != null)
            {
                ModelState.AddModelError("FLOWERTYPE_NAME", "Tên danh mục đã tồn tại");
                return View(flowerType);
            }

            var flowerTypeToUpdate = db.FLOWERTYPES.Where(f => f.FLOWERTYPE_ID == flowerType.FLOWERTYPE_ID && f.DELETED == false).FirstOrDefault();
            if (flowerTypeToUpdate != null)
            {
                flowerTypeToUpdate.FLOWERTYPE_NAME = formattedFlowerType;
                db.SaveChanges();
                TempData["SuccessFully"] = "Sửa thành công!";

                return RedirectToAction("Index");
            }

            return PartialView("_NotFound");
        }

        [HttpPost]
        public JsonResult DeleteMultiple(List<Guid> flowerTypeIds)
        {
            try
            {
                var flowerTypesToDelete = db.FLOWERTYPES.Where(f => flowerTypeIds.Contains(f.FLOWERTYPE_ID) && f.DELETED == false).ToList();

                foreach (var item in flowerTypesToDelete)
                {
                    item.DELETED = true;
                }

                db.SaveChanges();
                TempData["SuccessFully"] = "Xóa thành công!";

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}
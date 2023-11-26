using FlowerShop.Context;
using FlowerShop.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FlowerShop.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "1")]
    public class DiscountController : Controller
    {
        private readonly FlowerShopEntities db;
        public DiscountController()
        {
            db = new FlowerShopEntities();
        }
        public ActionResult Index()
        {
            var discount = db.DISCOUNTCODES.AsNoTracking().Where(d => d.DELETED == false).ToList();
            ViewBag.SuccessFully = TempData["SuccessFully"];
            return View(discount);
        }

        public ActionResult Detail(Guid? discountId)
        {
            var discount = db.DISCOUNTCODES.AsNoTracking().Where(d => d.DISCOUNT_ID == discountId && d.DELETED == false).FirstOrDefault();
            return View(discount);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(DISCOUNTCODE discount)
        {
            if (!ModelState.IsValid)
            {
                return View(discount);
            }

            if(discount.DISCOUNT_TYPE == true && discount.DISCOUNT_VALUE > 100)
            {
                ModelState.AddModelError("DISCOUNT_VALUE", "Giá trị giảm giá < 100%");
                return View(discount);
            }

            if (discount.DISCOUNT_TYPE == false && discount.DISCOUNT_VALUE > discount.MINIMUM_ORDER_AMOUNT)
            {
                ModelState.AddModelError("DISCOUNT_VALUE", "Giá trị giảm giá không được lớn hơn đơn hàng tối thiểu");
                return View(discount);
            }

            discount.CODE = discount.CODE.ToUpper();
            var existingDiscount = db.DISCOUNTCODES.AsNoTracking().Where(d => d.CODE == discount.CODE && d.DELETED == false).FirstOrDefault();

            if (existingDiscount != null)
            {
                ModelState.AddModelError("CODE", "Mã giảm giá đã tồn tại");
                return View(discount);
            }

            var discountValue = "";
            if (discount.DISCOUNT_TYPE == true)
            {
                discountValue = discount.DISCOUNT_VALUE + "%";
            }
            else
            {
                discountValue = discount.DISCOUNT_VALUE + "VNĐ";
            }

            discount.DESCRIPTION = "Giảm " + discountValue + " cho đơn hàng từ " + discount.MINIMUM_ORDER_AMOUNT / 1000 + "K(Mỗi khách hàng chỉ sử dụng được 1 lần)";

            discount.DISCOUNT_ID = Guid.NewGuid();
            db.DISCOUNTCODES.Add(discount);
            db.SaveChanges();

            TempData["SuccessFully"] = "Thêm thành công!";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(Guid? discountId)
        {
            var discount = db.DISCOUNTCODES.AsNoTracking().Where(f => f.DISCOUNT_ID == discountId && f.DELETED == false).FirstOrDefault();

            if (discount != null)
            {
                return View(discount);
            }
            return PartialView("_NotFound");
        }

        [HttpPost]
        public ActionResult Edit(DISCOUNTCODE discount)
        {
            if (!ModelState.IsValid)
            {
                return View(discount);
            }

            if (discount.DISCOUNT_TYPE == true && discount.DISCOUNT_VALUE > 100)
            {
                ModelState.AddModelError("DISCOUNT_VALUE", "Giá trị giảm giá < 100%");
                return View(discount);
            }

            if (discount.DISCOUNT_TYPE == false && discount.DISCOUNT_VALUE > discount.MINIMUM_ORDER_AMOUNT)
            {
                ModelState.AddModelError("DISCOUNT_VALUE", "Giá trị giảm giá không được lớn hơn đơn hàng tối thiểu");
                return View(discount);
            }

            discount.CODE = discount.CODE.ToUpper();
            var existingDiscount = db.DISCOUNTCODES.AsNoTracking().Where(d => d.CODE == discount.CODE && d.DISCOUNT_ID != discount.DISCOUNT_ID && d.DELETED == false).FirstOrDefault();

            if (existingDiscount != null)
            {
                ModelState.AddModelError("CODE", "Mã giảm giá đã tồn tại");
                return View(discount);
            }

            var discountToUpdate = db.DISCOUNTCODES.Where(d => d.DISCOUNT_ID == discount.DISCOUNT_ID && d.DELETED == false).FirstOrDefault();
            if (discountToUpdate != null)
            {
                var discountValue = "";
                if (discount.DISCOUNT_TYPE == true)
                {
                    discountValue = discount.DISCOUNT_VALUE + "%";
                }
                else
                {
                    discountValue = discount.DISCOUNT_VALUE.Value.ToString("N0") + "VNĐ";
                }

                discountToUpdate.DESCRIPTION = "Giảm " + discountValue + " cho đơn hàng từ " + discount.MINIMUM_ORDER_AMOUNT.Value.ToString("N0") + "VNĐ(Mỗi khách hàng chỉ sử dụng được 1 lần)";
                discountToUpdate.CODE = discount.CODE;
                discountToUpdate.DISCOUNT_TYPE = discount.DISCOUNT_TYPE;
                discountToUpdate.DISCOUNT_VALUE = discount.DISCOUNT_VALUE;
                discountToUpdate.MINIMUM_ORDER_AMOUNT = discount.MINIMUM_ORDER_AMOUNT;
                discountToUpdate.START_DATE = discount.START_DATE;
                discountToUpdate.END_DATE = discount.END_DATE;
                db.SaveChanges();
                TempData["SuccessFully"] = "Sửa thành công!";

                return RedirectToAction("Index");
            }

            return PartialView("_NotFound");
        }

        [HttpPost]
        public JsonResult DeleteMultiple(List<Guid> discountIds)
        {
            try
            {
                var discountsToDelete = db.DISCOUNTCODES.Where(d => discountIds.Contains(d.DISCOUNT_ID) && d.DELETED == false).ToList();

                foreach (var item in discountsToDelete)
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
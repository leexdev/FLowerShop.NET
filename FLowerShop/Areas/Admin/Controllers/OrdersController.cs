using FlowerShop.Context;
using FlowerShop.Models;
using FlowerShop.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FlowerShop.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        private readonly FlowerShopEntities db;
        private readonly ApiProvince apiProvince;

        public OrdersController()
        {
            db = new FlowerShopEntities();
            apiProvince = new ApiProvince();
        }

        public ActionResult Index()
        {
            var orders = db.ORDERS.AsNoTracking().Where(o => o.DELETED == false).ToList();
            ViewBag.SuccessFully = TempData["SuccessFully"];

            return View(orders);
        }

        public ActionResult Detail(Guid? orderId)
        {
            var order = db.ORDERS.AsNoTracking().Where(f => f.ORDER_ID == orderId && f.DELETED == false).FirstOrDefault();

            if (order != null)
            {
                return View(order);
            }
            return PartialView("_NotFound");
        }

        public async Task<ActionResult> Edit(Guid? orderId)
        {
            var order = db.ORDERS.AsNoTracking().Where(f => f.ORDER_ID == orderId && f.DELETED == false).FirstOrDefault();


            if (order != null)
            {
                OrderModel orderModel = new OrderModel()
                {
                    Order = order
                };
                ViewBag.ListProvinces = await apiProvince.GetProvincesAsync();
                return View(orderModel);
            }
            return PartialView("_NotFound");
        }

        [HttpPost]
        public ActionResult Edit(ORDER order)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }

            var orderToUpdate = db.ORDERS.AsNoTracking().Where(f => f.ORDER_ID == order.ORDER_ID && f.DELETED == false).FirstOrDefault();
            if (orderToUpdate != null)
            {
                orderToUpdate = order;
                db.SaveChanges();
                TempData["SuccessFully"] = "Sửa thành công!";

                return RedirectToAction("Index");
            }

            return PartialView("_NotFound");
        }

        [HttpPost]
        public JsonResult DeleteMultiple(List<Guid> orderIds)
        {
            try
            {
                var ordersToDelete = db.ORDERS.Where(f => orderIds.Contains(f.ORDER_ID) && f.DELETED == false).ToList();

                foreach (var item in ordersToDelete)
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
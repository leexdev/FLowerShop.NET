using FlowerShop.Context;
using FlowerShop.Controllers;
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
    [CustomAuthorize(Roles = "1")]
    public class OrdersController : BaseController
    {
        private readonly FlowerShopEntities db;
        private readonly EmailService emailService;
        private readonly ApiProvince apiProvince;

        public OrdersController()
        {
            db = new FlowerShopEntities();
            apiProvince = new ApiProvince();
            emailService = new EmailService();
        }

        public ActionResult Index()
        {
            var orders = db.ORDERS.AsNoTracking().Where(o => o.DELETED == false).ToList();
            ViewBag.SuccessFully = TempData["SuccessFully"];

            return View(orders);
        }

        [HttpPost]  
        public async Task<ActionResult> UpdateStatus(Guid? orderId, string status, string content)
        {
            var order = db.ORDERS.Where(o => o.ORDER_ID == orderId).FirstOrDefault();
            if (order != null)
            {
                var orderHistory = new ORDERHISTORY()
                {
                    HISTORY_ID = Guid.NewGuid(),
                    ORDER_ID = orderId,
                    CHANGE_DATE = DateTime.Now,
                    CONTENT = content,
                    STATUS = status
                };
                db.ORDERHISTORies.Add(orderHistory);
                db.SaveChanges();

                string toEmailCustomer = order.SENDER_EMAIL;
                string subjectCustomer = status;
                string bodyCustomer = "Thông tin đơn hàng";

                string htmlBodyCustomer = RenderToString("_MailTextToCustomer", order);

                await Task.WhenAll(
                    emailService.SendEmailAsync(toEmailCustomer, subjectCustomer, bodyCustomer, htmlBodyCustomer)
                );

                TempData["SuccessFully"] = "Cập nhật thành công!";
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
        }

        public ActionResult Detail(Guid? orderId)
        {
            var order = db.ORDERS.AsNoTracking().Where(f => f.ORDER_ID == orderId && f.DELETED == false).FirstOrDefault();

            if (order != null)
            {
                if (order.MESSAGE_TO_RECIPIENT != null)
                {
                    order.MESSAGE_TO_RECIPIENT = order.MESSAGE_TO_RECIPIENT.Replace("\n", "<br>");
                }
                if (order.MESSAGE_TO_SHOP != null)
                {
                    order.MESSAGE_TO_SHOP = order.MESSAGE_TO_SHOP.Replace("\n", "<br>");    
                }
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
                ViewBag.ListDistricts = await apiProvince.GetDistrictsByProvinceNameAsync(order.RECIPIENT_PROVINCE);

                return View(orderModel);
            }
            return PartialView("_NotFound");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ORDER order, string districtName, int ProvinceCode)
        {
            if (!ModelState.IsValid)
            {
                var existingOrder = db.ORDERS.AsNoTracking().Where(o => o.ORDER_ID == order.ORDER_ID).FirstOrDefault();
                ViewBag.ListProvinces = await apiProvince.GetProvincesAsync();
                ViewBag.ListDistricts = await apiProvince.GetDistrictsByProvinceAsync(ProvinceCode);
                OrderModel orderModel = new OrderModel()
                {
                    Order = existingOrder
                };

                return View(orderModel);
            }

            var orderToUpdate = db.ORDERS.Where(f => f.ORDER_ID == order.ORDER_ID && f.DELETED == false).FirstOrDefault();
            if (orderToUpdate != null)
            {
                var provinceName = await apiProvince.GetProvinceNameByCodeAsync(ProvinceCode);
                orderToUpdate.RECIPIENT_PROVINCE = provinceName;
                orderToUpdate.RECIPIENT_DISTRICT = districtName;
                orderToUpdate.SENDER_NAME = order.SENDER_NAME;
                orderToUpdate.SENDER_EMAIL = order.SENDER_EMAIL;
                orderToUpdate.SENDER_PHONE = order.SENDER_PHONE;
                orderToUpdate.RECIPIENT_NAME = order.RECIPIENT_NAME;
                orderToUpdate.RECIPIENT_PHONE = order.RECIPIENT_PHONE;
                orderToUpdate.RECIPIENT_ADDRESS = order.RECIPIENT_ADDRESS;
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

        public async Task<JsonResult> GetDistricts(int provinceCode)
        {
            var districts = await apiProvince.GetDistrictsByProvinceAsync(provinceCode);

            var districtsData = districts.Select(d => new
            {
                name = (string)d.name
            });

            return Json(districtsData, JsonRequestBehavior.AllowGet);
        }
    }
}
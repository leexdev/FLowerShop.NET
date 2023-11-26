using Antlr.Runtime.Misc;
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
    public class HomeController : Controller
    {
        private readonly FlowerShopEntities db;

        public HomeController()
        {
            db = new FlowerShopEntities();
        }

        public ActionResult Index()
        {
            var totalAmount = db.ORDERS
                 .AsNoTracking()
                 .Where(o => o.DELETED == false)
                 .Join(
                     db.ORDERHISTORies
                         .GroupBy(oh => oh.ORDER_ID)
                         .Select(ohGroup => new
                         {
                             ORDER_ID = ohGroup.Key,
                             LatestStatus = ohGroup.OrderByDescending(oh => oh.CHANGE_DATE).FirstOrDefault().STATUS
                         }),
                     order => order.ORDER_ID,
                     orderHistory => orderHistory.ORDER_ID,
                     (order, orderHistory) => new { Order = order, OrderHistory = orderHistory }
                 )
                 .Where(joined => joined.OrderHistory.LatestStatus == "Đã giao hàng")
                 .Sum(joined => joined.Order.TOTAL_AMOUNT);


            ViewBag.TotalAmount = totalAmount.HasValue ? totalAmount.Value.ToString("N0") : "0";

            ViewBag.CountCustomer = db.USERS.AsNoTracking().Where(u => u.ROLE == false && u.DELETED == false).Count();
            ViewBag.CountFlower = db.FLOWERS.AsNoTracking().Where(f => f.DELETED == false).Count();
            ViewBag.CountOrder = db.ORDERS.AsNoTracking().Where(o => o.DELETED == false).Count();

            var flowerSales = db.FLOWERS
                .AsNoTracking()
                .Where(flower => flower.ORDERDETAILS.Any() &&
                    flower.ORDERDETAILS.Any(od =>
                        od.ORDER.ORDERHISTORies
                            .OrderByDescending(oh => oh.CHANGE_DATE)
                            .FirstOrDefault() != null &&
                        od.ORDER.ORDERHISTORies
                            .OrderByDescending(oh => oh.CHANGE_DATE)
                            .FirstOrDefault().STATUS == "Đã giao hàng"
                    ) && flower.DELETED == false
                )
                .OrderByDescending(flower => flower.ORDERDETAILS.Sum(od => od.QUANTITY))
                .Take(5)
                .ToList();



            return View(flowerSales);
        }

        public ActionResult GetSalesData(int year)
        {
            var allMonthsData = Enumerable.Range(1, 12)
                .Select(month => new
                {
                    Month = month,
                    Quantity = 0,
                    TotalAmount = 0
                })
                .ToList();

            var dbData = db.ORDERDETAILS
                .Where(s => s.ORDER.ORDER_DATE.Value.Year == year &&
                            s.ORDER.ORDERHISTORies.OrderByDescending(oh => oh.CHANGE_DATE)
                                .FirstOrDefault() != null &&
                            s.ORDER.ORDERHISTORies.OrderByDescending(oh => oh.CHANGE_DATE)
                                .FirstOrDefault().STATUS == "Đã giao hàng"
                )
                .Select(s => new
                {
                    Month = s.ORDER.ORDER_DATE.Value.Month,
                    s.QUANTITY,
                    s.ORDER.TOTAL_AMOUNT
                })
                .ToList();

            var combinedData = allMonthsData
                .GroupJoin(dbData,
                           allMonth => allMonth.Month,
                           dbMonth => dbMonth.Month,
                           (allMonth, dbMonthGroup) => new
                           {
                               allMonth.Month,
                               Quantity = dbMonthGroup.Sum(dbMonth => dbMonth?.QUANTITY ?? 0),
                               TotalAmount = dbMonthGroup.Select(dbMonth => dbMonth.TOTAL_AMOUNT).Distinct().Sum()
                           })
                .ToList();

            return Json(combinedData, JsonRequestBehavior.AllowGet);
        }


    }
}
using Antlr.Runtime.Misc;
using FlowerShop.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlowerShop.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly FlowerShopEntities db;

        public HomeController()
        {
            db = new FlowerShopEntities();
        }

        public ActionResult Index()
        {
            ViewBag.TotalAmount = db.ORDERS.AsNoTracking().Where(o => o.DELETED == false).Sum(o => o.TOTAL_AMOUNT).Value.ToString("N0");
            ViewBag.CountCustomer = db.USERS.AsNoTracking().Where(u => u.ROLE == false && u.DELETED == false).Count();
            ViewBag.CountFlower = db.FLOWERS.AsNoTracking().Where(f => f.DELETED == false).Count();
            ViewBag.CountOrder = db.ORDERS.AsNoTracking().Where(o => o.DELETED == false).Count();

            var flowerSales = db.FLOWERS.AsNoTracking()
                 .Where(flower => flower.ORDERDETAILS.Any() && flower.DELETED == false)
                 .OrderByDescending(flower => flower.ORDERDETAILS.Sum(orderDetail => orderDetail.QUANTITY))
                 .Take(4)
                 .ToList();

            return View(flowerSales);
        }
    }
}
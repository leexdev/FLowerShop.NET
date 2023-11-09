using FLowerShop.Context;
using FLowerShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;

namespace FLowerShop.Controllers
{
    public class CheckoutController : BaseController
    {
        private readonly FLowerShopEntities db;

        public CheckoutController()
        {
            db = new FLowerShopEntities();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            LoadCommonData();
            base.OnActionExecuting(filterContext);
        }

        [HttpPost]
        public ActionResult ExitCheckout()
        {
            Session.Remove("BuyFlower");
            return Json(new { success = true });
        }

        private List<SHOPPINGCART> GetShoppingCartItems()
        {
            var shoppingCartItems = new List<SHOPPINGCART>();

            if (Session["BuyFlower"] != null && Session["BuyFlower"] is SHOPPINGCART)
            {
                shoppingCartItems.Add(Session["BuyFlower"] as SHOPPINGCART);
            }
            else if (Session["ShoppingCart"] != null && Session["ShoppingCart"] is List<SHOPPINGCART>)
            {
                shoppingCartItems = Session["ShoppingCart"] as List<SHOPPINGCART>;
            }

            return shoppingCartItems;
        }

        public ActionResult Index()
        {
            if (Session["BuyFlower"] == null && Session["ShoppingCart"] == null)
            {
                return RedirectToAction("Index", "ShoppingCart");
            }

            var flower = GetShoppingCartItems();

            var orderModel = new OrderModel
            {
                ShoppingCarts = flower,
            };

            return View(orderModel);
        }

        [HttpPost]
        public ActionResult Index(ORDER order, string couponName)
        {
            if (ModelState.IsValid)
            {
                order.ORDER_ID = Guid.NewGuid();
                order.ORDER_DATE = DateTime.Now;
                order.USER_ID = Session["UserId"] as Guid?;

                var flower = GetShoppingCartItems();
                var totalAmount = 0;
                foreach (var item in flower)
                {
                    var orderDetail = new ORDERDETAIL();
                    orderDetail.ORDERDETAIL_ID = Guid.NewGuid();
                    orderDetail.ORDER_ID = order.ORDER_ID;
                    orderDetail.FLOWER_ID = (Guid)item.FLOWER_ID;
                    orderDetail.QUANTITY = item.QUANTITY;
                    orderDetail.SUBTOTAL = item.SUBTOTAL;
                    totalAmount += (int)item.SUBTOTAL;
                    db.ORDERDETAILS.Add(orderDetail);
                }
                var coupon = db.DISCOUNTCODES.FirstOrDefault(c => c.CODE == couponName);
                if (coupon != null)
                {
                    if (coupon.DISCOUNT_VALUE == 1)
                    {
                        totalAmount = totalAmount - (totalAmount * (int)coupon.DISCOUNT_VALUE / 100);
                    }
                    else
                    {
                        totalAmount = totalAmount - (int)coupon.DISCOUNT_VALUE;
                    }
                }
                order.DISCOUNT_ID = coupon.DISCOUNT_ID;
                order.TOTAL_AMOUNT = totalAmount;
                db.ORDERS.Add(order);
                db.SaveChanges();
                return View("OrderSuccess");
            }
            else
            {
                var flower = GetShoppingCartItems();
                var orderModel = new OrderModel
                {
                    ShoppingCarts = flower,
                };

                return View(orderModel);
            }
        }

        [HttpPost]
        public ActionResult CheckCoupon(string couponName)
        {
            var coupon = db.DISCOUNTCODES.FirstOrDefault(c => c.CODE == couponName);
            var flower = GetShoppingCartItems();
            var totalPriceGrand = flower.Sum(f => f.SUBTOTAL);
            if (coupon != null)
            {
                if (coupon.DISCOUNT_VALUE == 1)
                {
                    totalPriceGrand = totalPriceGrand - (totalPriceGrand * (int)coupon.DISCOUNT_VALUE / 100);
                }
                else
                {
                    totalPriceGrand = totalPriceGrand - (int)coupon.DISCOUNT_VALUE;
                }
            }

            return Json(new { TotalPriceGrand = totalPriceGrand, couponSuccess = "Áp dụng mã giảm giá thành công" });
        }
    }
}
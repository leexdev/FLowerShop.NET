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
        private readonly FlowerShopEntities db;
        private readonly EmailService _emailService;

        public CheckoutController()
        {
            db = new FlowerShopEntities();
            _emailService = new EmailService();
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

        private int CalculateTotalPrice(List<SHOPPINGCART> shoppingCarts)
        {
            return shoppingCarts.Sum(cart => (int)cart.SUBTOTAL);
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
            if (!ModelState.IsValid)
            {
                var flower = GetShoppingCartItems();
                var orderModel = new OrderModel
                {
                    ShoppingCarts = flower,
                };
                return View(orderModel);
            }

            else
            {
                var flower = GetShoppingCartItems();
                order.ORDER_ID = Guid.NewGuid();
                order.ORDER_DATE = DateTime.Now;
                order.USER_ID = Session["UserId"] as Guid?;

                var totalPriceGrand = CalculateTotalPrice(flower);
                var coupon = db.DISCOUNTCODES.FirstOrDefault(c => c.CODE == couponName.ToUpper());

                if (coupon != null)
                {
                    var discountValue = coupon.DISCOUNT_TYPE == true ? (totalPriceGrand * (int)coupon.DISCOUNT_VALUE / 100) : (int)coupon.DISCOUNT_VALUE;
                    coupon.CODE_COUNT -= 1;
                    totalPriceGrand -= discountValue;
                    order.DISCOUNT_ID = coupon.DISCOUNT_ID;
                }

                order.TOTAL_AMOUNT = totalPriceGrand;

                foreach (var item in flower)
                {
                    var orderDetail = new ORDERDETAIL
                    {
                        ORDERDETAIL_ID = Guid.NewGuid(),
                        ORDER_ID = order.ORDER_ID,
                        FLOWER_ID = (Guid)item.FLOWER_ID,
                        QUANTITY = item.QUANTITY,
                        SUBTOTAL = item.SUBTOTAL
                    };
                    db.ORDERDETAILS.Add(orderDetail);
                }

                db.ORDERS.Add(order);
                db.SaveChanges();
                string toEmail = "leex.dev@gmail.com";
                string subject = "Subject of the email";
                string body = "Body of the email";

                var checkoutFlower = new OrderModel
                {
                    ShoppingCarts = flower
                };
                
                string htmlBody = RenderToString("_CheckoutFlower", checkoutFlower);

                _emailService.SendEmail(toEmail, subject, body, htmlBody);
                return View("OrderSuccess");
            }
        }


        //[HttpPost]
        //public ActionResult DeleteFLower(Guid? shoppingCartId)
        //{
        //    var shoppingCarts = GetShoppingCartItems();
        //    var shoppingCart = shoppingCarts.FirstOrDefault(s => s.CART_ID == shoppingCartId);

        //    if (shoppingCart != null)
        //    {
        //        shoppingCarts.Remove(shoppingCart);
        //        UpdateShoppingCartData(shoppingCarts);

        //        if (shoppingCarts.Count == 0)
        //        {
        //            Session["ShoppingCart"] = null;
        //        }


        //        var checkoutFlowerModel = new OrderModel
        //        {
        //            ShoppingCarts = shoppingCarts
        //        };

        //        return Json(new
        //        {
        //            CheckoutFlower = RenderToString("_CheckoutFlower", checkoutFlowerModel),
        //            CartCount = shoppingCarts.Count
        //        });
        //    }

        //    return PartialView("_NotFound");
        //}

        [HttpPost]  
        public ActionResult CheckCoupon(string couponName)
        {
            var coupon = db.DISCOUNTCODES.FirstOrDefault(c => c.CODE == couponName.ToUpper());
            var flower = GetShoppingCartItems();

            decimal? totalPriceGrand = CalculateTotalPrice(flower);
            decimal? totalPriceGrandNew = 0;
            decimal? couponValue = 0;

            //if (Session["UserId"] == null)
            //    return Json(new { CouponError = "Vui lòng đăng nhập để sử dụng mã giảm giá" });
            if (coupon != null)
            {
                var userId = Session["UserId"] as Guid?;
                var discountId = db.USERDISCOUNTs.FirstOrDefault(u => u.USER_ID == userId)?.DISCOUNT_ID;
                if (discountId == coupon.DISCOUNT_ID)
                    return Json(new { CouponError = "Mỗi mã giảm giá chỉ sử dụng được 1 lần" });

                if (coupon.MINIMUM_ORDER_AMOUNT > totalPriceGrand)
                    return Json(new { TotalPriceGrand = totalPriceGrand + 30000, CouponError = $"Giá trị đơn hàng tối thiểu là {coupon.MINIMUM_ORDER_AMOUNT:N0} VNĐ" });

                couponValue = coupon.DISCOUNT_TYPE == true ? (totalPriceGrand * (int)coupon.DISCOUNT_VALUE / 100) : (int)coupon.DISCOUNT_VALUE;
                totalPriceGrandNew = totalPriceGrand - couponValue;

                return Json(new { TotalPriceGrand = totalPriceGrandNew + 30000, CouponValue = couponValue, CouponSuccess = "Áp dụng mã giảm giá thành công!" });
            }

            return Json(new { TotalPriceGrand = totalPriceGrand + 30000, CouponError = "Mã giảm giá không tồn tại!" });
        }

    }
}
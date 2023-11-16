using FLowerShop.Context;
using FLowerShop.Models;
using FLowerShop.Service.Momo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        private readonly EmailService emailService;

        public CheckoutController()
        {
            db = new FlowerShopEntities();
            emailService = new EmailService();
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
            if (Session["BuyFlower"] is SHOPPINGCART singleCart)
            {
                return new List<SHOPPINGCART> { singleCart };
            }

            return Session["ShoppingCart"] as List<SHOPPINGCART> ?? new List<SHOPPINGCART>();
        }

        private int CalculateTotalPrice(List<SHOPPINGCART> shoppingCarts)
        {
            return shoppingCarts.Sum(cart => (int)cart.SUBTOTAL);
        }

        public ActionResult OrderSuccess()
        {
            return View();
        }

        public ActionResult OrderPaymentSuccess()
        {
            return View();
        }

        public ActionResult OrderPaymentError()
        {
            return View();
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
                return View(new OrderModel { ShoppingCarts = GetShoppingCartItems() });
            }

            var userId = Session["UserId"] as Guid?;
            var flower = GetShoppingCartItems();
            order.ORDER_ID = Guid.NewGuid();
            order.ORDER_DATE = DateTime.Now;
            order.USER_ID = userId;

            var totalPriceGrand = CalculateTotalPrice(flower);
            var coupon = db.DISCOUNTCODES.FirstOrDefault(c => c.CODE == couponName.ToUpper());

            if (coupon != null)
            {
                var discountUser = db.USERDISCOUNTs.FirstOrDefault(u => u.USER_ID == userId && u.DISCOUNT_ID == coupon.DISCOUNT_ID);

                if (userId != null && coupon.MINIMUM_ORDER_AMOUNT < totalPriceGrand && discountUser == null)
                {
                    var discountValue = coupon.DISCOUNT_TYPE == true ? (totalPriceGrand * (int)coupon.DISCOUNT_VALUE / 100) : (int)coupon.DISCOUNT_VALUE;
                    totalPriceGrand -= discountValue;
                }

                coupon.CODE_COUNT -= 1;
                order.DISCOUNT_ID = coupon.DISCOUNT_ID;
            }

            order.TOTAL_AMOUNT = totalPriceGrand + 30000;

            foreach (var item in flower)
            {
                var orderDetail = new ORDERDETAIL
                {
                    ORDERDETAIL_ID = Guid.NewGuid(),
                    ORDER_ID = order.ORDER_ID,
                    FLOWER_ID = (Guid)item.FLOWER_ID,
                    QUANTITY = item.QUANTITY,
                    SUBTOTAL = item.SUBTOTAL,
                    FLOWER = db.FLOWERS.FirstOrDefault()
                };
                db.ORDERDETAILS.Add(orderDetail);
            }

            db.ORDERS.Add(order);

            var orderHistory = new ORDERHISTORY()
            {
                HISTORY_ID = Guid.NewGuid(),
                CHANGE_DATE = DateTime.Now,
                ORDER_ID = order.ORDER_ID,
                STATUS = "Chờ xác nhận",
                CONTENT = "Đơn hàng đã được tạo thành công"
            };
            db.ORDERHISTORies.Add(orderHistory);
            db.SaveChanges();

            string toEmailAdmin = "leex.dev@gmail.com";
            string subjectAdmin = "Bạn có đơn hàng mới!";
            string bodyAdmin = "Thông tin đơn hàng";

            string toEmailCustomer = order.SENDER_EMAIL;
            string subjectCustomer = "Đơn hàng đã được tạo";
            string bodyCustomer = "Thông tin đơn hàng";

            var orderToAdmin = new OrderModel
            {
                Order = order
            };

            var orderToCustomer = new OrderModel
            {
                Order = order,
                OrderHistories = db.ORDERHISTORies.Where(o => o.ORDER_ID == order.ORDER_ID).ToList()
            };

            string htmlBodyAdmin = RenderToString("_MailTextToAdmin", orderToAdmin);
            string htmlBodyCustomer = RenderToString("_MailTextToCustomer", orderToCustomer);

            emailService.SendEmail(toEmailAdmin, subjectAdmin, bodyAdmin, htmlBodyAdmin);
            if (order.PAYMENT_METHOD == false)
            {
                emailService.SendEmail(toEmailCustomer, subjectCustomer, bodyCustomer, htmlBodyCustomer);
            }

            if (Session["BuyFlower"] == null)
            {
                Session.Remove("ShoppingCart");
            }

            ViewBag.EmailCustomer = order.SENDER_EMAIL;

            if (order.PAYMENT_METHOD == true)
            {
                return RedirectToAction("Payment", order);
            }

            return View("OrderSuccess");
        }

        public ActionResult Payment(ORDER order)
        {
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "Thanh toán đơn hàng cho Bloom Shop";
            string returnUrl = "https://localhost:44388/Checkout/ConfirmPaymentClient";
            string notifyurl = "https://localhost:44388/Checkout/SavePayment";

            string amount = order.TOTAL_AMOUNT.ToString();
            string orderId = order.ORDER_ID.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            string rawHash = $"partnerCode={partnerCode}&accessKey={accessKey}&requestId={requestId}&amount={amount}&orderId={orderId}&orderInfo={orderInfo}&returnUrl={returnUrl}&notifyUrl={notifyurl}&extraData={extraData}";

            MoMoSecurity crypto = new MoMoSecurity();
            string signature = crypto.signSHA256(rawHash, serectkey);

            JObject message = new JObject
        {
            { "partnerCode", partnerCode },
            { "accessKey", accessKey },
            { "requestId", requestId },
            { "amount", amount },
            { "orderId", orderId },
            { "orderInfo", orderInfo },
            { "returnUrl", returnUrl },
            { "notifyUrl", notifyurl },
            { "extraData", extraData },
            { "requestType", "captureMoMoWallet" },
            { "signature", signature }
        };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        public ActionResult ConfirmPaymentClient(Result result)
        {
            string rMessage = result.message;
            string rOrderId = result.orderId;
            int rErrorCode = int.Parse(result.errorCode);
            var orderId = Guid.Parse(rOrderId);
            var order = db.ORDERS.FirstOrDefault(o => o.ORDER_ID == orderId);
            ViewBag.EmailCustomer = order.SENDER_EMAIL;

            if (rErrorCode == 0)
            {
                string toEmailCustomer = order.SENDER_EMAIL;
                string subjectCustomer = "Đơn hàng đã được tạo";
                string bodyCustomer = "Thông tin đơn hàng";
                var orderToCustomer = new OrderModel
                {
                    Order = order,
                    OrderHistories = db.ORDERHISTORies.Where(o => o.ORDER_ID == order.ORDER_ID).ToList()
                };

                string htmlBodyCustomer = RenderToString("_MailTextToCustomer", orderToCustomer);
                emailService.SendEmail(toEmailCustomer, subjectCustomer, bodyCustomer, htmlBodyCustomer);

                return View("OrderPaymentSuccess");
            }
            else
            {
                if (order != null)
                {
                    order.PAYMENT_METHOD = false;
                    db.SaveChanges();
                }
                return View("OrderPaymentError");
            }
        }

        [HttpPost]
        public void SavePayment()
        {
            // Handle saving payment data to the database
        }

        [HttpPost]
        public JsonResult CheckCoupon(string couponName, ORDER order)
        {
            var userId = Session["UserId"] as Guid?;
            if (userId == null)
                return Json(new { CouponError = "Vui lòng đăng nhập để sử dụng mã giảm giá" });

            var coupon = db.DISCOUNTCODES.FirstOrDefault(c => c.CODE == couponName.ToUpper());
            var flower = GetShoppingCartItems();

            decimal? totalPriceGrand = CalculateTotalPrice(flower);

            if (coupon != null)
            {
                var discountUser = db.USERDISCOUNTs.FirstOrDefault(u => u.USER_ID == userId && u.DISCOUNT_ID == coupon.DISCOUNT_ID);
                if (discountUser != null)
                    return Json(new { CouponError = "Mỗi mã giảm giá chỉ sử dụng được 1 lần" });

                decimal? totalPriceGrandNew = 0;
                decimal? couponValue = 0;

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
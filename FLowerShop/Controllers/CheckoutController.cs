using FlowerShop.Context;
using FlowerShop.Models;
using FlowerShop.Service;
using FlowerShop.Service.Momo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FlowerShop.Controllers
{
    public class CheckoutController : BaseController
    {
        private readonly FlowerShopEntities db;
        private readonly EmailService emailService;
        private readonly ApiProvince apiProvince;

        public CheckoutController()
        {
            db = new FlowerShopEntities();
            emailService = new EmailService();
            apiProvince = new ApiProvince();
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

        private List<SHOPPINGCART> GetShoppingCarts()
        {
            if (Session["BuyFlower"] is SHOPPINGCART singleCart)
            {
                return new List<SHOPPINGCART> { singleCart };
            }

            if (Session["UserId"] != null)
            {
                Guid userId = (Guid)Session["UserId"];
                return db.SHOPPINGCARTs.Where(s => s.USER_ID == userId && s.DELETED == false).ToList();
            }
            else if (Session["ShoppingCart"] != null)
            {
                return Session["ShoppingCart"] as List<SHOPPINGCART>;
            }
            return new List<SHOPPINGCART>();
        }

        private int CalculateTotalPrice(List<SHOPPINGCART> shoppingCarts)
        {
            return shoppingCarts.Sum(cart => (int)cart.SUBTOTAL);
        }

        public ActionResult OrderSuccess()
        {
            ViewBag.EmailCustomer = TempData["EmailCustomer"];
            return View();
        }

        public async Task<ActionResult> Index()
        {
            var flower = GetShoppingCarts();

            if (flower.Count == 0)
            {
                return RedirectToAction("Index", "ShoppingCart");
            }

            var order = new ORDER();

            if (Session["UserId"] != null)
            {
                var userId = (Guid)Session["UserId"];
                var user = db.USERS.Where(u => u.USER_ID == userId && u.DELETED == false).FirstOrDefault();
                order.SENDER_NAME = user.USER_NAME;
                order.SENDER_PHONE = user.USER_PHONE;
                order.SENDER_EMAIL = user.USER_EMAIL;
            }

            var orderModel = new OrderModel
            {
                ShoppingCarts = flower,
                Order = order
            };

            ViewBag.ListProvinces = await apiProvince.GetProvincesAsync();

            return View(orderModel);
        }

        [HttpPost]
        public async Task<ActionResult> Index(ORDER order, string couponName, string districtName, int ProvinceCode)
        {
            var provinceName = await apiProvince.GetProvinceNameByCodeAsync(ProvinceCode);
            if (!ModelState.IsValid)
            {
                ViewBag.ListProvinces = await apiProvince.GetProvincesAsync();
                ViewBag.ListDistricts = await apiProvince.GetDistrictsByProvinceAsync(ProvinceCode);
                OrderModel model = new OrderModel
                {
                    ShoppingCarts = GetShoppingCarts(),
                    ProvinceName = provinceName,
                    DistrictName = districtName
                };

                return View(model);
            }

            var userId = Session["UserId"] as Guid?;
            var flower = GetShoppingCarts();
            order.ORDER_ID = Guid.NewGuid();
            order.ORDER_DATE = DateTime.Now;
            order.USER_ID = userId;
            order.RECIPIENT_DISTRICT = districtName;
            order.RECIPIENT_PROVINCE = provinceName;

            var totalPriceGrand = CalculateTotalPrice(flower);

            DateTime currentDate = DateTime.Now;

            var coupon = db.DISCOUNTCODES
                .Where(c => c.CODE == couponName.ToUpper() &&
                            c.START_DATE <= currentDate &&
                            c.END_DATE >= currentDate &&
                            c.CODE_COUNT > 0)
                .FirstOrDefault();

            if (coupon != null)
            {
                var discountUser = db.USERDISCOUNTs.FirstOrDefault(u => u.USER_ID == userId && u.DISCOUNT_ID == coupon.DISCOUNT_ID && u.DELETED == false);

                if (userId != null && coupon.MINIMUM_ORDER_AMOUNT < totalPriceGrand && discountUser == null)
                {
                    var discountValue = coupon.DISCOUNT_TYPE == true ? (totalPriceGrand * (int)coupon.DISCOUNT_VALUE / 100) : (int)coupon.DISCOUNT_VALUE;
                    totalPriceGrand -= discountValue;
                    coupon.CODE_COUNT -= 1;
                    var userDiscount = new USERDISCOUNT()
                    {
                        USERDISCOUNT_ID = Guid.NewGuid(),
                        DISCOUNT_ID = coupon.DISCOUNT_ID,
                        USER_ID = userId
                    };
                    db.USERDISCOUNTs.Add(userDiscount);
                }

                order.DISCOUNT_ID = coupon.DISCOUNT_ID;
            }

            order.TOTAL_AMOUNT = totalPriceGrand + 30000;

            await SaveOrderToDatabaseAsync(order, flower);

            db.SaveChanges();

            if (order.PAYMENT_METHOD == true)
            {
                return RedirectToAction("Payment", "Checkout", new { orderId = order.ORDER_ID });
            }

            TempData["EmailCustomer"] = order.SENDER_EMAIL;
            return RedirectToAction("OrderSuccess");
        }

        public ActionResult OrderPaymentSuccess()
        {
            ViewBag.EmailCustomer = TempData["EmailCustomer"];
            return View();
        }

        public ActionResult OrderPaymentError()
        {
            return View();
        }

        public ActionResult Payment(Guid orderid)
        {
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "Thanh toán đơn hàng cho Bloom Shop";
            string returnUrl = "https://localhost:44343/Checkout/ConfirmPaymentClient";
            string notifyurl = "https://localhost:44343/Checkout/SavePayment";

            var order = db.ORDERS.FirstOrDefault(o => o.ORDER_ID == orderid && o.DELETED == false);
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

        public async Task<ActionResult> ConfirmPaymentClient(ResultMomo result)
        {
            string rMessage = result.message;
            string rOrderId = result.orderId;
            int rErrorCode = int.Parse(result.errorCode);
            var orderId = Guid.Parse(rOrderId);
            var order = db.ORDERS.FirstOrDefault(o => o.ORDER_ID == orderId && o.DELETED == false);
            if (rErrorCode == 0)
            {
                await SendEmailsAsync(order.ORDER_ID);

                if (Session["BuyFlower"] == null)
                {
                    if (Session["UserId"] != null)
                    {
                        var userId = (Guid)Session["UserId"];

                        var shoppingCarts = db.SHOPPINGCARTs.Where(s => s.USER_ID == userId && s.DELETED == false);
                        foreach (var shoppingCart in shoppingCarts)
                        {
                            db.SHOPPINGCARTs.Remove(shoppingCart);
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        Session["ShoppingCart"] = null;
                    }
                }
                TempData["EmailCustomer"] = order.SENDER_EMAIL;
                return RedirectToAction("OrderPaymentSuccess");
            }
            else
            {
                var orderDetail = db.ORDERDETAILS.Where(o => o.ORDER_ID == orderId && o.DELETED == false).ToList();

                foreach (var item in orderDetail)
                {
                    db.ORDERDETAILS.Remove(item);
                }

                var orderHistories = db.ORDERHISTORies.Where(o => o.ORDER_ID == orderId).ToList();

                foreach (var item in orderHistories)
                {
                    db.ORDERHISTORies.Remove(item);
                }

                db.ORDERS.Remove(order);
                db.SaveChanges();
                return RedirectToAction("OrderPaymentError");
            }
        }


        [HttpPost]
        public void SavePayment()
        {
            // Handle saving payment data to the database
        }

        private async Task SaveOrderToDatabaseAsync(ORDER order, List<SHOPPINGCART> shoppingCart)
        {
            foreach (var item in shoppingCart)
            {
                var orderDetail = new ORDERDETAIL
                {
                    ORDERDETAIL_ID = Guid.NewGuid(),
                    ORDER_ID = order.ORDER_ID,
                    FLOWER_ID = (Guid)item.FLOWER_ID,
                    QUANTITY = item.QUANTITY,
                    SUBTOTAL = item.SUBTOTAL,
                };

                db.ORDERDETAILS.Add(orderDetail);
            }

            var orderHistory = new ORDERHISTORY()
            {
                HISTORY_ID = Guid.NewGuid(),
                CHANGE_DATE = DateTime.Now,
                ORDER_ID = order.ORDER_ID,
                STATUS = "Chờ xác nhận",
                CONTENT = "Đơn hàng đã được tạo thành công"
            };

            db.ORDERHISTORies.Add(orderHistory);

            db.ORDERS.Add(order);

            db.SaveChanges();

            if (order.PAYMENT_METHOD == false)
            {
                await SendEmailsAsync(order.ORDER_ID);
            }

            if (Session["BuyFlower"] == null && order.PAYMENT_METHOD == false)
            {
                if (Session["UserId"] != null)
                {
                    foreach (var item in shoppingCart)
                    {
                        db.SHOPPINGCARTs.Remove(item);
                    }
                }
                else
                {
                    Session["ShoppingCart"] = null;
                }
            }
        }

        private async Task SendEmailsAsync(Guid? orderId)
        {
            var order = db.ORDERS.AsNoTracking().Where(o => o.ORDER_ID == orderId).FirstOrDefault();
            string toEmailAdmin = "leex.dev@gmail.com";
            string subjectAdmin = "Bạn có đơn hàng mới!";
            string bodyAdmin = "Thông tin đơn hàng";

            string toEmailCustomer = order.SENDER_EMAIL;
            string subjectCustomer = "Đơn hàng đã được tạo";
            string bodyCustomer = "Thông tin đơn hàng";

            string htmlBodyAdmin = RenderToString("_MailTextToAdmin", order);
            string htmlBodyCustomer = RenderToString("_MailTextToCustomer", order);

            await Task.WhenAll(
                emailService.SendEmailAsync(toEmailAdmin, subjectAdmin, bodyAdmin, htmlBodyAdmin),
                emailService.SendEmailAsync(toEmailCustomer, subjectCustomer, bodyCustomer, htmlBodyCustomer)
            );
        }

        [HttpPost]
        public JsonResult CheckCoupon(string couponName)
        {
            var userId = Session["UserId"] as Guid?;
            if (userId == null)
                return Json(new { CouponError = "Vui lòng đăng nhập để sử dụng mã giảm giá" });

            DateTime currentDate = DateTime.Now;

            var coupon = db.DISCOUNTCODES
                .Where(c => c.CODE == couponName.ToUpper() &&
                            c.START_DATE <= currentDate &&
                            c.END_DATE >= currentDate &&
                            c.CODE_COUNT > 0)
                .FirstOrDefault();

            var flower = GetShoppingCarts();

            decimal? totalPriceGrand = CalculateTotalPrice(flower);

            if (coupon != null)
            {
                var discountUser = db.USERDISCOUNTs.FirstOrDefault(u => u.USER_ID == userId && u.DISCOUNT_ID == coupon.DISCOUNT_ID && u.DELETED == false);
                if (discountUser != null)
                    return Json(new { CouponError = "Mỗi mã giảm giá chỉ sử dụng được 1 lần" });

                decimal? totalPriceGrandNew = 0;
                decimal? couponValue = 0;

                if (coupon.MINIMUM_ORDER_AMOUNT > totalPriceGrand)
                    return Json(new { TotalPriceGrand = totalPriceGrand + 30000, CouponError = $"Giá trị đơn hàng tối thiểu là {coupon.MINIMUM_ORDER_AMOUNT:N0} VNĐ" });

                couponValue = coupon.DISCOUNT_TYPE == true ? (totalPriceGrand * (int)coupon.DISCOUNT_VALUE / 100) : (int)coupon.DISCOUNT_VALUE;
                totalPriceGrandNew = totalPriceGrand - couponValue;

                return Json(new { TotalPriceGrand = totalPriceGrandNew + 30000, CouponValue = couponValue, CouponName = couponName ,CouponSuccess = "Áp dụng mã giảm giá thành công!" });
            }

            return Json(new { TotalPriceGrand = totalPriceGrand + 30000, CouponError = "Mã giảm giá không tồn tại!" });
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
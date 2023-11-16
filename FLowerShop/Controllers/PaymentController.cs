using FLowerShop.Context;
using FLowerShop.Models;
using FLowerShop.Service.Momo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FLowerShop.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly FlowerShopEntities db;
        private readonly EmailService emailService;

        public PaymentController()
        {
            db = new FlowerShopEntities();
            emailService = new EmailService();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            LoadCommonData();
            base.OnActionExecuting(filterContext);
        }

        public ActionResult OrderPaymentSuccess()
        {
            return View();
        }

        public ActionResult OrderPaymentError()
        {
            return View();
        }

        public ActionResult Payment(ORDER order)
        {
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "Thanh toán đơn hàng cho Bloom Shop";
            string returnUrl = "https://localhost:44388/Payment/ConfirmPaymentClient";
            string notifyurl = "https://localhost:44388/Payment/SavePayment";

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
            if (rErrorCode == 0)
            {
                string toEmailAdmin = "leex.dev@gmail.com";
                string subjectAdmin = "Bạn có đơn hàng mới!";
                string bodyAdmin = "Thông tin đơn hàng";

                string toEmailCustomer = order.SENDER_EMAIL;
                string subjectCustomer = "Đơn hàng đã được tạo";
                string bodyCustomer = "Thông tin đơn hàng";

                var orderToCustomer = new OrderModel
                {
                    Order = order,
                    OrderHistories = db.ORDERHISTORies.Where(o => o.ORDER_ID == order.ORDER_ID).ToList()
                };

                var orderToAdmin = new OrderModel
                {
                    Order = order
                };

                string htmlBodyCustomer = RenderToString("_MailTextToCustomer", orderToCustomer);
                string htmlBodyAdmin = RenderToString("_MailTextToAdmin", orderToAdmin);

                emailService.SendEmail(toEmailCustomer, subjectCustomer, bodyCustomer, htmlBodyCustomer);
                emailService.SendEmail(toEmailAdmin, subjectAdmin, bodyAdmin, htmlBodyAdmin);

                ViewBag.EmailCustomer = order.SENDER_EMAIL;
                return View("OrderPaymentSuccess");
            }
            else
            {
                var orderDetail = db.ORDERDETAILS.Where(o => o.ORDER_ID == order.ORDER_ID).ToList();

                foreach(var item in orderDetail)
                {
                    db.ORDERDETAILS.Remove(item);
                }
                var orderHistories = db.ORDERHISTORies.Where(o => o.ORDER_ID == order.ORDER_ID).ToList();

                foreach (var item in orderHistories)
                {
                    db.ORDERHISTORies.Remove(item);
                }

                db.ORDERS.Remove(order);
                db.SaveChanges();
                return View("OrderPaymentError");
            }
        }


        [HttpPost]
        public void SavePayment()
        {
            // Handle saving payment data to the database
        }
    }
}
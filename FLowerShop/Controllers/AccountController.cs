using Facebook;
using FlowerShop.Context;
using FlowerShop.Models;
using FlowerShop.Service;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FlowerShop.Controllers
{
    public class AccountController : BaseController
    {
        private readonly FlowerShopEntities db;
        private readonly EmailService emailService;

        public AccountController()
        {
            db = new FlowerShopEntities();
            emailService = new EmailService();
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            LoadCommonData();
            base.OnActionExecuting(filterContext);
        }

        public ActionResult RegisterSucess()
        {
            return View();
        }

        [CustomAuthorize]
        public ActionResult Logout()
        {
            Session.Clear();
            TempData["ShowAlert"] = 2;
            return RedirectToAction("Login");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(USER user)
        {
            if (!ModelState.IsValidField("USER_NAME") || !ModelState.IsValidField("USER_EMAIL") || !ModelState.IsValidField("USER_PHONE") || !ModelState.IsValidField("USER_PASSWORD"))
            {
                return View(user);
            }

            var existingUser = db.USERS.Where(u => u.DELETED == false).FirstOrDefault(u => u.USER_EMAIL == user.USER_EMAIL);

            if (existingUser != null)
            {
                ModelState.AddModelError("USER_EMAIL", "Địa chỉ Email đã tồn tại");
                return View(user);
            }

            if (user.USER_PASSWORD != user.CONFIRM_PASSWORD)
            {
                ModelState.AddModelError("CONFIRM_PASSWORD", "Mật khẩu và mật khẩu xác nhận không khớp!");
                return View(user);
            }

            user.USER_ID = Guid.NewGuid();
            user.USER_PASSWORD = GetMd5Hash(user.USER_PASSWORD);

            db.USERS.Add(user);
            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();

            return RedirectToAction("RegisterSucess");
        }

        //Login Facebook
        public ActionResult Login()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = "236953919208447",
                redirect_uri = "https://localhost:44343/Account/FacebookRedirect",
                scope = "public_profile,email"
            });
            ViewBag.UrlFacebook = loginUrl;
            ViewBag.ShowAlert = TempData["ShowAlert"];

            return View();
        }

        [HttpPost]
        public ActionResult Login(USER user)
        {
            if (!ModelState.IsValidField("USER_EMAIL") || !ModelState.IsValidField("USER_PASSWORD"))
            {
                return View(user);
            }

            var existingUser = db.USERS.Where(u => u.USER_EMAIL == user.USER_EMAIL && u.DELETED == false).FirstOrDefault();
            if (existingUser == null)
            {
                ModelState.AddModelError("USER_EMAIL", "Địa chỉ Email chưa được đăng ký");
                return View(user);
            }

            if (existingUser.USER_PASSWORD != GetMd5Hash(user.USER_PASSWORD))
            {
                ModelState.AddModelError("USER_PASSWORD", "Mật khẩu không chính xác");
                return View(user);
            }

            Session["UserId"] = existingUser.USER_ID;
            Session["Role"] = existingUser.ROLE;
            Session.Remove("ShoppingCart");

            ModelState.Clear();

            if((Boolean)Session["Role"] == true)
            {
                return RedirectToAction("Index", "Home", new {area = "Admin"});
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult FacebookRedirect(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Get("/oauth/access_token", new
            {
                client_id = "236953919208447",
                client_secret = "679b8583142923a12be66f7fcd24a9e9",
                redirect_uri = "https://localhost:44343/Account/FacebookRedirect",
                code = code
            });

            fb.AccessToken = result.access_token;

            dynamic me = fb.Get("/me?fields=name,email");
            string name = me.name;
            string email = me.email;
            string facebookid = me.id;

            var user = db.USERS.FirstOrDefault(u => u.FACEBOOKID == facebookid && u.DELETED == false);

            if (user == null)
            {
                user = new USER
                {
                    USER_ID = Guid.NewGuid(),
                    USER_NAME = name,
                    USER_EMAIL = email,
                    FACEBOOKID = facebookid
                };

                db.USERS.Add(user);
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
            }

            Session["UserId"] = user.USER_ID;
            Session["Role"] = user.ROLE;
            Session.Remove("ShoppingCart");

            if ((Boolean)Session["Role"] == true)
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(USER user)
        {
            if (!ModelState.IsValidField("USER_EMAIL"))
            {
                return View(user);
            }

            var existingUser = db.USERS.FirstOrDefault(u => u.USER_EMAIL == user.USER_EMAIL && u.DELETED == false);
            if (existingUser == null)
            {
                ModelState.AddModelError("USER_EMAIL", "Địa chỉ Email chưa được đăng ký");
                return View(user);
            }

            var resetToken = Guid.NewGuid();

            existingUser.RESETTOKEN = resetToken;
            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();

            string resetLink = Url.Action("ResetPassword", "Account", new { token = resetToken }, Request.Url.Scheme);
            string emailBody = $"Mật khẩu mới đã được yêu cầu cho tài khoản khách hàng.<br><br> Để đặt lại mật khẩu của bạn, hãy nhấp vào liên kết bên dưới: <br><br> {resetLink}";

            await emailService.SendEmailAsync(user.USER_EMAIL, "Thay đổi mật khẩu", "Quên mật khẩu", emailBody);

            TempData["ShowAlert"] = 0;

            return RedirectToAction("Login");
        }

        public ActionResult ResetPassword(Guid? token)
        {
            if(token == null)
            {
                return PartialView("_NotFound");
            }
            var user = db.USERS.FirstOrDefault(u => u.RESETTOKEN == token && u.DELETED == false);

            if (user == null)
            {
                return PartialView("_NotFound");
            }

            ViewBag.ResetToken = token;

            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(Guid? token, USER user)
        {
            var existingUser = db.USERS.FirstOrDefault(u => u.RESETTOKEN == token && u.DELETED == false);
            if (existingUser == null)
            {
                return PartialView("_NotFound");
            }

            if (!ModelState.IsValidField("USER_PASSWORD"))
            {
                ViewBag.ResetToken = token;
                return View(user);
            }

            if(user.USER_PASSWORD != user.CONFIRM_PASSWORD)
            {
                ModelState.AddModelError("CONFIRM_PASSWORD", "Mật khẩu và mật khẩu xác nhận không khớp");
                ViewBag.ResetToken = token;
                return View(user);
            }

            existingUser.USER_PASSWORD = GetMd5Hash(user.USER_PASSWORD);
            existingUser.RESETTOKEN = null;
            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();

            TempData["ShowAlert"] = 1;
            return RedirectToAction("Login");
        }

        public static string GetMd5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        [CustomAuthorize]
        public ActionResult Account()
        {
            ViewBag.ShowAlert = TempData["ShowAlert"];
            return View();
        }

        [CustomAuthorize]
        public ActionResult EditAccount()
        {
            var userId = (Guid)Session["UserId"];

            if (userId != null)
            {
                var user = db.USERS.Where(u => u.USER_ID == userId && u.DELETED == false).FirstOrDefault();

                return View(user);
            }

            return PartialView("_NotFound");
        }

        [CustomAuthorize]
        [HttpPost]
        public ActionResult EditAccount(USER user)
        {
            if (!ModelState.IsValidField("USER_NAME") || !ModelState.IsValidField("USER_EMAIL") || !ModelState.IsValidField("USER_PHONE"))
            {
                return View(user);
            }

            var existingUser = db.USERS.Where(u => u.USER_ID == user.USER_ID && u.DELETED == false).FirstOrDefault();

            if (existingUser != null)
            {
                existingUser.USER_NAME = user.USER_NAME;
                existingUser.USER_PHONE = user.USER_PHONE;
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
                TempData["ShowAlert"] = 0;
                return RedirectToAction("Account", "Account");
            }

            return PartialView("_NotFound");
        }

        [CustomAuthorize]
        public ActionResult ChangePassword()
        {
            var userId = (Guid)Session["UserId"];

            if (userId != null)
            {
                var user = db.USERS.Where(u => u.USER_ID == userId && u.DELETED == false).FirstOrDefault();

                return View(user);
            }

            return PartialView("_NotFound");
        }

        [CustomAuthorize]
        [HttpPost]
        public ActionResult ChangePassword(USER user)
        {
            if (!ModelState.IsValidField("USER_PASSWORD"))
            {
                return View(user);
            }

            if (user.USER_PASSWORD != user.CONFIRM_PASSWORD)
            {
                ModelState.AddModelError("CONFIRM_PASSWORD", "Mật khẩu và mật khẩu xác nhận không khớp");
                return View(user);
            }

            var existingUser = db.USERS.Where(u => u.USER_ID == user.USER_ID && u.DELETED == false).FirstOrDefault();

            if (existingUser != null)
            {
                existingUser.USER_PASSWORD = GetMd5Hash(user.USER_PASSWORD);
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
                TempData["ShowAlert"] = 1;
                return RedirectToAction("Account");
            }
            return PartialView("_NotFound");
        }

        [CustomAuthorize]
        public ActionResult OrderHistory(int? page)
        {
            var userId = (Guid)Session["UserId"];

            if (userId != null)
            {
                var order = db.ORDERS.Where(o => o.USER_ID == userId && o.DELETED == false).ToList();

                int pageSize = 5;
                int pageNumber = (page ?? 1);
                return View(order.ToPagedList(pageNumber, pageSize));
            }

            return PartialView("_NotFound");
        }

        [CustomAuthorize]
        public ActionResult OrderDetail(Guid? orderId)
        {
            var order = db.ORDERS.Where(o => o.ORDER_ID == orderId && o.DELETED == false).FirstOrDefault();
            if (order != null)
            {
                return View(order);
            }

            return PartialView("_NotFound");
        }
    }
}
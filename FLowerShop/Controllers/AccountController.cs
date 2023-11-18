using Facebook;
using FLowerShop.Context;
using FLowerShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FLowerShop.Controllers
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

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(USER user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var exitingEmail = db.USERS.FirstOrDefault(u => u.USER_EMAIL == user.USER_EMAIL);

            if (exitingEmail != null)
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
            db.SaveChanges();

            ModelState.Clear();

            return View("RegisterSucess");
        }

        //Login Facebook
        public ActionResult Login()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = "236953919208447",
                redirect_uri = "https://localhost:44388/Account/FacebookRedirect",
                scope = "public_profile,email"
            });
            ViewBag.UrlFacebook = loginUrl;

            return View();
        }

        [HttpPost]
        public ActionResult Login(USER user)
        {
            Session.Remove("ShoppingCart");
            return View();
        }

        public ActionResult FacebookRedirect(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Get("/oauth/access_token", new
            {
                client_id = "236953919208447",
                client_secret = "679b8583142923a12be66f7fcd24a9e9",
                redirect_uri = "https://localhost:44388/Account/FacebookRedirect",
                code = code
            });

            fb.AccessToken = result.access_token;

            dynamic me = fb.Get("/me?fields=name,email");
            string name = me.name;
            string email = me.email;
            string facebookid = me.id;

            var user = db.USERS.FirstOrDefault(u => u.FACEBOOKID == facebookid);

            if(user == null)
            {
                user = new USER
                {
                    USER_ID = Guid.NewGuid(),
                    USER_NAME = name,
                    USER_EMAIL = email,
                    FACEBOOKID = facebookid
                };

                db.USERS.Add(user);
                db.SaveChanges();
            }

            Session["UserId"] = user.USER_ID;
            Session["UserName"] = user.USER_NAME;
            Session["UserEmail"] = user.USER_EMAIL;
            Session["UserPhone"] = user.USER_PHONE;
            Session["Role"] = user.ROLE;
            Session.Remove("ShoppingCart");

            return RedirectToAction("Index", "Home");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(USER user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            var User = db.USERS.FirstOrDefault(u => u.USER_EMAIL == user.USER_EMAIL);
            if (User == null)
            {
                ModelState.AddModelError("USER_EMAIL", "Địa chỉ Email không tồn tại");
                return View(user);
            }
            var resetToken = Guid.NewGuid();

            User.RESETTOKEN = resetToken;
            db.SaveChanges();

            string resetLink = Url.Action("ResetPassword", "Account", new { token = resetToken }, Request.Url.Scheme);
            string emailBody = $"Mật khẩu mới đã được yêu cầu cho tài khoản khách hàng.<br><br> Để đặt lại mật khẩu của bạn, hãy nhấp vào liên kết bên dưới: <br><br> {resetLink}";

            emailService.SendEmail(user.USER_EMAIL, "Thay đổi mật khẩu", "Quên mật khẩu", emailBody);

            ViewBag.ShowAlert = 0;
            return View("Login");
        }

        public ActionResult ResetPassword(Guid? token)
        {
            if(token == null)
            {
                return PartialView("_NotFound");
            }
            var user = db.USERS.FirstOrDefault(u => u.RESETTOKEN == token);

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
            var User = db.USERS.FirstOrDefault(u => u.RESETTOKEN == token);
            if (User == null)
            {
                return PartialView("_NotFound");
            }

            if(user.USER_PASSWORD != user.CONFIRM_PASSWORD)
            {
                ModelState.AddModelError("CONFIRM_PASSWORD", "Mật khẩu và mật khẩu xác nhận không khớp");
                return View(user);
            }

            User.USER_PASSWORD = GetMd5Hash(user.USER_PASSWORD);
            User.RESETTOKEN = null;
            db.SaveChanges();

            ViewBag.ShowAlert = 1;
            return View("Login");
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

    }
}
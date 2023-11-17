using Facebook;
using FLowerShop.Context;
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

        public AccountController()
        {
            db = new FlowerShopEntities();
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

            return View("RegisterSucess");
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

            return RedirectToAction("Index", "Home");
        }
    }
}
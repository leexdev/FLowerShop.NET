using FlowerShop.Context;
using FlowerShop.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FlowerShop.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "1")]
    public class AccountController : Controller
    {
        private readonly FlowerShopEntities db;
        public AccountController()
        {
            db = new FlowerShopEntities();
        }

        public ActionResult Profiles()
        {
            return PartialView("_NotFound");
        }

        public ActionResult Index()
        {
            var users = db.USERS.AsNoTracking().Where(u => u.DELETED == false).ToList();
            ViewBag.SuccessFully = TempData["SuccessFully"];
            return View(users);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(USER user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var existingUser = db.USERS.AsNoTracking().Where(u => u.USER_EMAIL == user.USER_EMAIL && u.DELETED == false).FirstOrDefault();

            if (existingUser != null)
            {
                ModelState.AddModelError("USER_EMAIL", "Địa chỉ Email đã tồn tại");
                return View(user);
            }

            user.USER_ID = Guid.NewGuid();
            user.USER_PASSWORD = GetMd5Hash(user.USER_PASSWORD);
            db.USERS.Add(user);
            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();

            TempData["SuccessFully"] = "Thêm thành công!";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(Guid? userId)
        {
            var user = db.USERS.AsNoTracking().Where(f => f.USER_ID == userId && f.DELETED == false).FirstOrDefault();

            if (user != null)
            {
                return View(user);
            }
            return PartialView("_NotFound");
        }

        [HttpPost]
        public ActionResult Edit(USER user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var existingUser = db.USERS.AsNoTracking().Where(u => u.USER_EMAIL == user.USER_EMAIL && u.USER_ID != user.USER_ID && u.DELETED == false).FirstOrDefault();

            if (existingUser != null)
            {
                ModelState.AddModelError("USER_EMAIL", "Địa chỉ Email đã tồn tại");
                return View(user);
            }

            var userToUpdate = db.USERS.Where(u => u.USER_ID == user.USER_ID && u.DELETED == false).FirstOrDefault();

            if (userToUpdate != null)
            {
                userToUpdate.USER_NAME = user.USER_NAME;
                userToUpdate.USER_EMAIL = user.USER_EMAIL;
                userToUpdate.USER_PHONE = user.USER_PHONE;
                if(userToUpdate.USER_PASSWORD != user.USER_PASSWORD)
                {
                    userToUpdate.USER_PASSWORD = GetMd5Hash(user.USER_PASSWORD);
                }
                else
                {
                    userToUpdate.USER_PASSWORD = user.USER_PASSWORD;
                }
                userToUpdate.ROLE = user.ROLE;

                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
                TempData["SuccessFully"] = "Sửa thành công!";

                return RedirectToAction("Index");
            }

            return PartialView("_NotFound");
        }

        [HttpPost]
        public JsonResult DeleteMultiple(List<Guid> userIds)
        {
            try
            {
                var usersToDelete = db.USERS.Where(u => userIds.Contains(u.USER_ID) && u.DELETED == false).ToList();

                foreach (var item in usersToDelete)
                {
                    item.DELETED = true;
                }

                db.SaveChanges();
                TempData["SuccessFully"] = "Xóa thành công!";

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
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
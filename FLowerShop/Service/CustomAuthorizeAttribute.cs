using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FlowerShop.Service
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Nếu Session có tồn tại, tức là người dùng đã đăng nhập
            if (httpContext.Session["Role"] != null)
            {
                return true;
            }

            // Ngược lại, người dùng chưa đăng nhập
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }
    }

}
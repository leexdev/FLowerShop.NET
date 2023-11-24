using System.Web.Mvc;
using System.Web.Routing;

namespace FlowerShop.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                name: "DeleteFlower",
                url: "Admin/Flowers/Delete/{flowerId}",
                defaults: new { controller = "Flowers", action = "Delete", flowerId = UrlParameter.Optional },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );

            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "FlowerShop.Areas.Admin.Controllers" }
            );
        }
    }
}
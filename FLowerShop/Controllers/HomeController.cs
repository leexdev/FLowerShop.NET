using FLowerShop.Context;
using FLowerShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FLowerShop.Controllers
{
    public class HomeController : BaseController
    {

        FlowerShopEntities db = new FlowerShopEntities();
        public ActionResult Index()
        {
            HomeModel objHomeModel = new HomeModel();
            objHomeModel.FlowerTypes = db.FLOWERTYPES.ToList();
            objHomeModel.Flowers = db.FLOWERS.ToList();
            LoadCommonData();
            return View(objHomeModel);
        }
    }
}
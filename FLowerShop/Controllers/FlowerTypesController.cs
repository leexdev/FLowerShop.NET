using FlowerShop.Context;
using FlowerShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlowerShop.Controllers
{
    public class FlowerTypesController : BaseController
    {
        private readonly FlowerShopEntities db;

        public FlowerTypesController()
        {
            db = new FlowerShopEntities();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            LoadCommonData();
            base.OnActionExecuting(filterContext);
        }

        public ActionResult Index(Guid? flowerTypeId, int filterValue = 0)
        {
            var filteredFlowers = db.FLOWERS
                .AsNoTracking()
                .Where(f => f.FLOWERTYPE_ID == flowerTypeId && f.DELETED == false)
                .ToList();

            filteredFlowers = SortFlowers(filteredFlowers, filterValue);
            var flowerTypeName = db.FLOWERTYPES.Where(f => f.FLOWERTYPE_ID == flowerTypeId).FirstOrDefault().FLOWERTYPE_NAME; 
            ViewBag.FlowerTypeId = flowerTypeId;
            ViewBag.FlowerTypeName = flowerTypeName;
            return View(filteredFlowers);
        }

        public ActionResult FilterFLowers(Guid? flowerTypeId, int filterValue)
        {
            var filteredFlowers = db.FLOWERS
                .AsNoTracking()
                .Where(f => f.FLOWERTYPE_ID == flowerTypeId && f.DELETED == false)
                .ToList();

            // Sử dụng giá trị filterValue từ tham số URL để áp dụng sắp xếp
            filteredFlowers = SortFlowers(filteredFlowers, filterValue);

            return PartialView("_FlowerType", filteredFlowers);
        }

        private List<FLOWER> SortFlowers(List<FLOWER> flowers, int? filterValue)
        {
            switch (filterValue)
            {
                case 1:
                    return flowers.OrderBy(flower => flower.FLOWER_NAME).ToList();
                case 2:
                    return flowers.OrderByDescending(flower => flower.FLOWER_NAME).ToList();
                case 3:
                    return flowers.OrderBy(flower => flower.NEW_PRICE).ToList();
                case 4:
                    return flowers.OrderByDescending(flower => flower.NEW_PRICE).ToList();
                default:
                    return flowers;
            }
        }
    }
}
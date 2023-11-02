using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using FLowerShop.Context;
using FLowerShop.Models;

namespace FLowerShop.Controllers
{
    public class FlowersController : BaseController
    {
        private readonly FlowerShopEntities db;

        public FlowersController()
        {
            db = new FlowerShopEntities();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            LoadCommonData();
            base.OnActionExecuting(filterContext);
        }

        public ActionResult Detail(Guid? flowerId)
        {
            if (flowerId == null)
            {
                return View("_NotFound");
            }
            var flower = db.FLOWERS.AsNoTracking().FirstOrDefault(f => f.FLOWER_ID == flowerId);
            if (flower == null)
            {
                return View("_NotFound");
            }
            var detailModel = new DetailModel
            {
                Flower = flower,
                Flowers = db.FLOWERS.AsNoTracking().Where(f => f.FLOWERTYPE_ID == flower.FLOWERTYPE_ID && f.FLOWER_ID != flowerId).ToList(),
                DiscountCodes = db.DISCOUNTCODES.AsNoTracking().ToList()
            };
            return View(detailModel);
        }

        public ActionResult Search(string searchQuery, Guid? flowerTypeId)
        {
            var searchModel = new SearchModel();

            var filteredFlowers = db.FLOWERS.AsNoTracking()
                .Where(f => string.IsNullOrEmpty(searchQuery) || f.FLOWER_NAME.Contains(searchQuery))
                .Where(f => !flowerTypeId.HasValue || f.FLOWERTYPE_ID == flowerTypeId)
                .ToList();

            var flowerTypes = db.FLOWERTYPES.AsNoTracking().ToList();

            searchModel.Flowers = filteredFlowers;
            searchModel.FlowersType = flowerTypes;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.SelectedFlowerTypeId = flowerTypeId.ToString();

            return View(searchModel);
        }

        public ActionResult FilterFLowers(int filterValue, string searchQuery, Guid? flowerTypeId)
        {
            var Flowers = db.FLOWERS.AsNoTracking()
                .Where(f => string.IsNullOrEmpty(searchQuery) || f.FLOWER_NAME.Contains(searchQuery))
                .Where(f => !flowerTypeId.HasValue || f.FLOWERTYPE_ID == flowerTypeId)
                .ToList();
            var filteredFlowers = new List<FLOWER>();

            switch (filterValue)
            {
                case 1:
                    filteredFlowers = Flowers.OrderBy(flower => flower.FLOWER_NAME).ToList();
                    break;
                case 2:
                    filteredFlowers = Flowers.OrderByDescending(flower => flower.FLOWER_NAME).ToList();
                    break;
                case 3:
                    filteredFlowers = Flowers.OrderBy(flower => flower.NEW_PRICE).ToList();
                    break;
                case 4:
                    filteredFlowers = Flowers.OrderByDescending(flower => flower.NEW_PRICE).ToList();
                    break;
                default:
                    filteredFlowers = Flowers;
                    break;
            }
            var searchModel = new SearchModel()
            {
                Flowers = filteredFlowers.ToList()
            };

            return PartialView("_FlowerList", searchModel);
        }


    }
}

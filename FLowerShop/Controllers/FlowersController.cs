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
        private readonly FLowerShopEntities db;

        public FlowersController()
        {
            db = new FLowerShopEntities();
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

            var flower = db.FLOWERS
                .AsNoTracking()
                .FirstOrDefault(f => f.FLOWER_ID == flowerId);

            if (flower == null)
            {
                return View("_NotFound");
            }

            var flowersOfType = db.FLOWERS
                .AsNoTracking()
                .Where(f => f.FLOWERTYPE_ID == flower.FLOWERTYPE_ID && f.FLOWER_ID != flowerId)
                .ToList();

            var discountCodes = db.DISCOUNTCODES.AsNoTracking().ToList();

            var detailModel = new DetailModel
            {
                Flower = flower,
                Flowers = flowersOfType,
                DiscountCodes = discountCodes
            };

            return View(detailModel);
        }

        public ActionResult Search(string searchQuery, Guid? flowerTypeId)
        {
            var filteredFlowers = db.FLOWERS
                .AsNoTracking()
                .Where(f => (string.IsNullOrEmpty(searchQuery) || f.FLOWER_NAME.Contains(searchQuery))
                    && (!flowerTypeId.HasValue || f.FLOWERTYPE_ID == flowerTypeId))
                .ToList();

            var flowerTypes = db.FLOWERTYPES.AsNoTracking().ToList();

            var searchModel = new SearchModel
            {
                Flowers = filteredFlowers,
                FlowersType = flowerTypes
            };

            ViewBag.SearchQuery = searchQuery;
            ViewBag.SelectedFlowerTypeId = flowerTypeId?.ToString();

            return View(searchModel);
        }

        public ActionResult FilterFLowers(int filterValue, string searchQuery, Guid? flowerTypeId)
        {
            var filteredFlowers = db.FLOWERS
                .AsNoTracking()
                .Where(f => (string.IsNullOrEmpty(searchQuery) || f.FLOWER_NAME.Contains(searchQuery))
                    && (!flowerTypeId.HasValue || f.FLOWERTYPE_ID == flowerTypeId))
                .ToList();

            filteredFlowers = SortFlowers(filteredFlowers, filterValue);

            var searchModel = new SearchModel
            {
                Flowers = filteredFlowers.ToList()
            };

            return PartialView("_FlowerSearch", searchModel);
        }

        private List<FLOWER> SortFlowers(List<FLOWER> flowers, int filterValue)
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

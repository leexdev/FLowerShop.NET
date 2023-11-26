using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using FlowerShop.Context;
using FlowerShop.Models;
using PagedList;

namespace FlowerShop.Controllers
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

            var flower = db.FLOWERS
                .AsNoTracking()
                .FirstOrDefault(f => f.FLOWER_ID == flowerId);

            if (flower == null)
            {
                return View("_NotFound");
            }
             
            if(flower.DESCRIPTION != null)
            {
                flower.DESCRIPTION = flower.DESCRIPTION.Replace("\n", "<br>");
            }
            var flowersOfType = db.FLOWERS
                .AsNoTracking()
                .Where(f => f.FLOWERTYPE_ID == flower.FLOWERTYPE_ID && f.FLOWER_ID != flowerId && f.DELETED == false)
                .ToList();

            DateTime currentDate = DateTime.Now;

            var discountCodes = db.DISCOUNTCODES.AsNoTracking().Where(c =>
                            c.START_DATE <= currentDate &&
                            c.END_DATE >= currentDate &&
                            c.CODE_COUNT > 0).ToList();

            var detailModel = new DetailModel
            {
                Flower = flower,
                Flowers = flowersOfType,
                DiscountCodes = discountCodes
            };

            return View(detailModel);
        }

        public ActionResult Search(string searchQuery, Guid? flowerTypeId, int? page, int filterValue = 0)
        {
            var filteredFlowers = db.FLOWERS
                .AsNoTracking()
                .Where(f => (string.IsNullOrEmpty(searchQuery) || f.FLOWER_NAME.ToLower().Contains(searchQuery.ToLower()))
                    && (!flowerTypeId.HasValue || f.FLOWERTYPE_ID == flowerTypeId) && f.DELETED == false)
                .ToList();

            if (TempData["FilterValue"] != null)
            {
                filterValue = (Int32)TempData["FilterValue"];
            }

            filteredFlowers = SortFlowers(filteredFlowers, filterValue);

            var flowerTypes = db.FLOWERTYPES.AsNoTracking().ToList();

            ViewBag.SearchQuery = searchQuery;
            ViewBag.SelectedFlowerTypeId = flowerTypeId?.ToString();
            ViewBag.ListFlowerTypes = db.FLOWERTYPES.Where(f => f.DELETED == false).ToList();
            ViewBag.FilterValue = filterValue;

            int pageSize = 8;
            int pageNumber = (page ?? 1);

            return View(filteredFlowers.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult FilterFLowers(string searchQuery, Guid? flowerTypeId, int? page, int filterValue)
        {
            var filteredFlowers = db.FLOWERS
                .AsNoTracking()
                .Where(f => (string.IsNullOrEmpty(searchQuery) || f.FLOWER_NAME.ToLower().Contains(searchQuery.ToLower()))
                    && (!flowerTypeId.HasValue || f.FLOWERTYPE_ID == flowerTypeId) && f.DELETED == false)
                .ToList();

            filteredFlowers = SortFlowers(filteredFlowers, filterValue);

            var flowerTypes = db.FLOWERTYPES.AsNoTracking().ToList();

            ViewBag.SearchQuery = searchQuery;
            ViewBag.SelectedFlowerTypeId = flowerTypeId?.ToString();
            ViewBag.ListFlowerTypes = db.FLOWERTYPES.Where(f => f.DELETED == false).ToList();
            TempData["FilterValue"] = filterValue;

            int pageSize = 8;
            int pageNumber = (page ?? 1);

            return PartialView("_FlowerSearch", filteredFlowers.ToPagedList(pageNumber, pageSize));
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

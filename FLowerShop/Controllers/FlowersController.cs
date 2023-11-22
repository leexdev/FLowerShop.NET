using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using FlowerShop.Context;
using FlowerShop.Models;

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

        public ActionResult Search(string searchQuery, Guid? flowerTypeId, int filterValue = 0)
        {
            var filteredFlowers = db.FLOWERS
                .AsNoTracking()
                .Where(f => (string.IsNullOrEmpty(searchQuery) || f.FLOWER_NAME.Contains(searchQuery))
                    && (!flowerTypeId.HasValue || f.FLOWERTYPE_ID == flowerTypeId) && f.DELETED == false)
                .ToList();

            var flowerTypes = db.FLOWERTYPES.AsNoTracking().ToList();

            filteredFlowers = SortFlowers(filteredFlowers, filterValue);

            var searchModel = new SearchModel
            {
                Flowers = filteredFlowers,
                FlowersType = flowerTypes
            };

            ViewBag.SearchQuery = searchQuery;
            ViewBag.SelectedFlowerTypeId = flowerTypeId?.ToString();

            return View(searchModel);
        }

        public ActionResult FilterFLowers(string searchQuery, Guid? flowerTypeId, int filterValue)
        {
            var filteredFlowers = db.FLOWERS
                .AsNoTracking()
                .Where(f => (string.IsNullOrEmpty(searchQuery) || f.FLOWER_NAME.Contains(searchQuery))
                    && (!flowerTypeId.HasValue || f.FLOWERTYPE_ID == flowerTypeId) && f.DELETED == false) 
                .ToList();

            // Sử dụng giá trị filterValue từ tham số URL để áp dụng sắp xếp
            filteredFlowers = SortFlowers(filteredFlowers, filterValue);

            var searchModel = new SearchModel
            {
                Flowers = filteredFlowers.ToList()
            };

            return PartialView("_FlowerSearch", searchModel);
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

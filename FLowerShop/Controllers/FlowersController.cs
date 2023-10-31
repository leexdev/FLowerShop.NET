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
        FlowerShopEntities db = new FlowerShopEntities();

        public ActionResult Detail(Guid? flowerId)
        {
            LoadCommonData();
            if (flowerId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetailModel detailModel = new DetailModel();
            var flower = db.FLOWERS.Find(flowerId);
            if (flower == null)
            {
                return HttpNotFound();
            }
            detailModel.Flower = flower;
            detailModel.Flowers = db.FLOWERS.Where(f => f.FLOWERTYPE_ID == flower.FLOWERTYPE_ID && f.FLOWER_ID != flowerId).ToList();
            detailModel.DiscountCodes = db.DISCOUNTCODES.ToList();
            return View(detailModel);
        }

        public ActionResult Search(string searchQuery, Guid? flowerTypeId)
        {
            LoadCommonData();
            SearchModel searchModel = new SearchModel();

            var flowers = db.FLOWERS.ToList(); // Retrieve all flowers from the database

            // Normalize the search query
            var normalizedQuery = NormalizeString(searchQuery);

            var filteredFlowers = flowers
                .Where(f => NormalizeString(f.FLOWER_NAME).Contains(normalizedQuery) &&
                    (!flowerTypeId.HasValue || f.FLOWERTYPE_ID == flowerTypeId))
                .ToList();

            var flowerType = db.FLOWERTYPES.ToList();
            searchModel.Flowers = filteredFlowers;
            searchModel.FlowersType = flowerType;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.SelectedFlowerTypeId = flowerTypeId.ToString();

            return View(searchModel);
        }

        private string NormalizeString(string input)
        {
            return new string(input.Normalize(NormalizationForm.FormD).Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());
        }
    }
}

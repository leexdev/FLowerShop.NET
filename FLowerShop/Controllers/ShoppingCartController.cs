using FLowerShop.Context;
using FLowerShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FLowerShop.Controllers
{
    public class ShoppingCartController : BaseController
    {
        FlowerShopEntities db = new FlowerShopEntities();

        public ActionResult Index()
        {
            LoadCommonData();
            var ShoppingCarts = Session["ShoppingCart"] as List<SHOPPINGCART>;
            return View(ShoppingCarts);
        }

        [HttpPost]
        public ActionResult BuyFLower(int? quantity, Guid? flowerId)
        {
            var buyFlower = new SHOPPINGCART();
            buyFlower.FLOWER_ID = flowerId;
            buyFlower.QUANTITY = quantity;
            buyFlower.USER_ID = Session["UserId"] as Guid?;
            buyFlower.FLOWER = db.FLOWERS.FirstOrDefault(f => f.FLOWER_ID == flowerId);
            if (buyFlower.FLOWER != null)
            {
                buyFlower.SUBTOTAL = buyFlower.FLOWER.NEW_PRICE * buyFlower.QUANTITY;
            }

            Session["BuyFlower"] = buyFlower;
            return RedirectToAction("Index", "Checkout");
        }

        [HttpPost]
        public ActionResult AddFlowerSC(int? quantity, Guid? flowerId)
        {
            List<SHOPPINGCART> shoppingCarts = Session["ShoppingCart"] as List<SHOPPINGCART>;
            if (shoppingCarts == null)
            {
                shoppingCarts = new List<SHOPPINGCART>();
            }
            SHOPPINGCART shoppingCart = shoppingCarts.FirstOrDefault(item => item.FLOWER_ID == flowerId);

            if (shoppingCart == null)
            {
                shoppingCart = new SHOPPINGCART();
                shoppingCart.FLOWER_ID = flowerId;
                shoppingCart.CART_ID = Guid.NewGuid();
                shoppingCart.QUANTITY = quantity;
                shoppingCart.USER_ID = Session["UserId"] as Guid?;
                shoppingCart.FLOWER = db.FLOWERS.FirstOrDefault(f => f.FLOWER_ID == flowerId);

                shoppingCarts.Add(shoppingCart);
            }
            else
            {
                shoppingCart.QUANTITY += quantity;
            }

            if (shoppingCart.FLOWER != null)
            {
                shoppingCart.SUBTOTAL = shoppingCart.FLOWER.NEW_PRICE * shoppingCart.QUANTITY;
            }

            var totalPriceGrand = 0;
            foreach (var item in shoppingCarts)
            {
                totalPriceGrand += (int)item.SUBTOTAL.Value;
            }

            Session["ShoppingCart"] = shoppingCarts;
            LoadCommonData();
            int newCartCount = CalculateCartCount();
            var flowerList = Session["ShoppingCart"] as List<SHOPPINGCART>;
            var cartFlowerModel = new CartFlowerModel
            {
                FlowerCarts = shoppingCarts
            };

            var cartFlowerPartial = this.RenderToString("_CartFlower", cartFlowerModel);
            var flowerListPartial = this.RenderToString("_ShoppingCartFLower", flowerList);

            return Json(new { CartFlower = cartFlowerPartial, FlowerList = flowerListPartial, PriceGrand = totalPriceGrand, CartCount = newCartCount });
        }

        [HttpPost]
        public ActionResult UpdateShoppingCart(int? quantity, Guid? shoppingCartId)
        {
            var shoppingCarts = Session["ShoppingCart"] as List<SHOPPINGCART>;

            if (shoppingCarts != null)
            {
                var shoppingCart = shoppingCarts.FirstOrDefault(s => s.CART_ID == shoppingCartId);

                if (shoppingCart != null)
                {
                    if (quantity.HasValue)
                    {
                        shoppingCart.QUANTITY = quantity.Value;
                        shoppingCart.SUBTOTAL = quantity.Value * shoppingCart.FLOWER.NEW_PRICE;

                        var totalPriceGrand = 0;
                        foreach (var item in shoppingCarts)
                        {
                            totalPriceGrand += (int)item.SUBTOTAL.Value;
                        }

                        int newCartCount = CalculateCartCount();
                        return Json(new { TotalPrice = shoppingCart.SUBTOTAL.Value, Quantity = shoppingCart.QUANTITY.Value, CartCount = newCartCount, TotalPriceGrand = totalPriceGrand });
                    }
                }
            }
            return Json(new { error = "Không thể cập nhật giỏ hàng" });
        }

        [HttpPost]
        public ActionResult DeleteFLower(Guid? shoppingCartId)
        {
            var shoppingCarts = Session["ShoppingCart"] as List<SHOPPINGCART>;

            if (shoppingCarts != null)
            {
                var shoppingCart = shoppingCarts.FirstOrDefault(s => s.CART_ID == shoppingCartId);

                shoppingCarts.Remove(shoppingCart);
                Session["ShoppingCart"] = shoppingCarts;
                LoadCommonData();
                var CartFlowers = new CartFlowerModel
                {
                    FlowerCarts = shoppingCarts
                };
                var cartFlowerPartial = this.RenderToString("_CartFlower", CartFlowers);
                var flowerListPartial = this.RenderToString("_ShoppingCartFLower", shoppingCarts);

                return Json(new { CartFlower = cartFlowerPartial, FlowerList = flowerListPartial, CartCount = CalculateCartCount() });
            }

            return HttpNotFound();
        }

        private int CalculateCartCount()
        {
            var shoppingCarts = Session["ShoppingCart"] as List<SHOPPINGCART>;

            if (shoppingCarts.Count > 0)
            {
                int cartCount = shoppingCarts.Count;
                return cartCount;
            }

            return 0;
        }
    }
}
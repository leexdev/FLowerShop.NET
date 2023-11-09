using FLowerShop.Context;
using FLowerShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FLowerShop.Controllers
{
    public class ShoppingCartController : BaseController
    {
        private readonly FLowerShopEntities db;

        public ShoppingCartController()
        {
            db = new FLowerShopEntities();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            LoadCommonData();
            base.OnActionExecuting(filterContext);
        }

        public ActionResult Index()
        {
            var shoppingCarts = GetShoppingCarts();
            return View(shoppingCarts);
        }

        [HttpPost]
        public ActionResult BuyFLower(int? quantity, Guid? flowerId)
        {
            var buyFlower = CreateBuyFlower(quantity, flowerId);
            Session["BuyFlower"] = buyFlower;
            return RedirectToAction("Index", "Checkout");
        }

        [HttpPost]
        public ActionResult AddFlowerSC(int? quantity, Guid? flowerId)
        {
            var shoppingCarts = GetShoppingCarts();
            var shoppingCart = shoppingCarts.FirstOrDefault(item => item.FLOWER_ID == flowerId);

            if (shoppingCart == null)
            {
                shoppingCart = CreateShoppingCart(quantity, flowerId);
                shoppingCarts.Add(shoppingCart);
            }
            else
            {
                shoppingCart.QUANTITY += quantity;
            }

            shoppingCart.SUBTOTAL = shoppingCart.QUANTITY * shoppingCart.FLOWER.NEW_PRICE;
            UpdateShoppingCartData(shoppingCarts);
            var totalPriceGrand = CalculateTotalPrice(shoppingCarts);
            int newCartCount = shoppingCarts.Count;

            return Json(new
            {
                CartFlower = RenderToString("_CartFlower", new CartFlowerModel { ShoppingCarts = shoppingCarts }),
                FlowerList = RenderToString("_ShoppingCartFLower", shoppingCarts),
                PriceGrand = totalPriceGrand,
                CartCount = newCartCount
            });
        }

        [HttpPost]
        public ActionResult UpdateShoppingCart(int? quantity, Guid? shoppingCartId)
        {
            var shoppingCarts = GetShoppingCarts();
            var shoppingCart = shoppingCarts.FirstOrDefault(s => s.CART_ID == shoppingCartId);

            if (shoppingCart != null && quantity.HasValue)
            {
                shoppingCart.QUANTITY = quantity.Value;
                shoppingCart.SUBTOTAL = quantity.Value * shoppingCart.FLOWER.NEW_PRICE;
                UpdateShoppingCartData(shoppingCarts);
                int totalPriceGrand = CalculateTotalPrice(shoppingCarts);
                int newCartCount = shoppingCarts.Count;

                return Json(new
                {
                    TotalPrice = shoppingCart.SUBTOTAL,
                    Quantity = shoppingCart.QUANTITY,
                    CartCount = newCartCount,
                    TotalPriceGrand = totalPriceGrand
                });
            }

            return Json(new { error = "Không thể cập nhật giỏ hàng" });
        }

        [HttpPost]
        public ActionResult DeleteFLower(Guid? shoppingCartId)
        {
            var shoppingCarts = GetShoppingCarts();
            var shoppingCart = shoppingCarts.FirstOrDefault(s => s.CART_ID == shoppingCartId);

            if (shoppingCart != null)
            {
                shoppingCarts.Remove(shoppingCart);
                UpdateShoppingCartData(shoppingCarts);

                if (shoppingCarts.Count == 0)
                {
                    Session["ShoppingCart"] = null;
                }

                var cartFlowerModel = new CartFlowerModel
                {
                    ShoppingCarts = shoppingCarts
                };

                var checkoutFlowerModel = new OrderModel
                {
                    ShoppingCarts = shoppingCarts
                };

                return Json(new
                {
                    CartFlower = RenderToString("_CartFlower", cartFlowerModel),
                    ShoppingCartFlower = RenderToString("_ShoppingCartFLower", shoppingCarts),
                    CheckoutFlower = RenderToString("_CheckoutFlower", checkoutFlowerModel),
                    CartCount = shoppingCarts.Count
                });
            }

            return Json(new { error = "Mục không tồn tại trong giỏ hàng" });
        }

        private List<SHOPPINGCART> GetShoppingCarts()
        {
            if (Session["UserId"] != null)
            {
                Guid userId = (Guid)Session["UserId"];
                return db.SHOPPINGCARTs.Where(s => s.USER_ID == userId).ToList();
            }
            else if (Session["ShoppingCart"] != null)
            {
                return Session["ShoppingCart"] as List<SHOPPINGCART>;
            }
            return new List<SHOPPINGCART>();
        }

        private void UpdateShoppingCartData(List<SHOPPINGCART> shoppingCarts)
        {
            Session["ShoppingCart"] = shoppingCarts;
        }

        private int CalculateTotalPrice(List<SHOPPINGCART> shoppingCarts)
        {
            return shoppingCarts.Sum(cart => (int)cart.SUBTOTAL);
        }

        private SHOPPINGCART CreateBuyFlower(int? quantity, Guid? flowerId)
        {
            var buyFlower = new SHOPPINGCART();
            buyFlower.FLOWER_ID = flowerId;
            buyFlower.QUANTITY = quantity;
            buyFlower.USER_ID = Session["UserId"] as Guid?;
            buyFlower.FLOWER = GetFlowerDetails(flowerId);

            if (buyFlower.FLOWER != null)
            {
                buyFlower.SUBTOTAL = buyFlower.FLOWER.NEW_PRICE * buyFlower.QUANTITY;
            }

            return buyFlower;
        }

        private SHOPPINGCART CreateShoppingCart(int? quantity, Guid? flowerId)
        {
            var shoppingCart = new SHOPPINGCART();
            shoppingCart.FLOWER_ID = flowerId;
            shoppingCart.CART_ID = Guid.NewGuid();
            shoppingCart.QUANTITY = quantity;
            shoppingCart.USER_ID = Session["UserId"] as Guid?;
            shoppingCart.FLOWER = GetFlowerDetails(flowerId);

            if (shoppingCart.FLOWER != null)
            {
                shoppingCart.SUBTOTAL = shoppingCart.FLOWER.NEW_PRICE * shoppingCart.QUANTITY;
            }

            return shoppingCart;
        }

        private FLOWER GetFlowerDetails(Guid? flowerId)
        {
            return db.FLOWERS.FirstOrDefault(f => f.FLOWER_ID == flowerId);
        }
    }
}
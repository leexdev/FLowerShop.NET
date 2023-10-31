using FLowerShop.Context;
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
        public ActionResult AddProductSC(int? quantity, Guid? flowerId)
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
            Session["ShoppingCart"] = shoppingCarts;
            return RedirectToAction("Index", "Checkout");
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

                        var response = new { newTotalPrice = shoppingCart.SUBTOTAL.Value };
                        return Json(response);
                    }
                }
            }
            return Json(new { error = "Không thể cập nhật giỏ hàng" });
        }


        [HttpPost]
        public ActionResult DeleteProductSC(Guid? shoppingCartId)
        {
            var shoppingCarts = Session["ShoppingCart"] as List<SHOPPINGCART>;

            if (shoppingCarts != null)
            {
                var shoppingCart = shoppingCarts.FirstOrDefault(s => s.CART_ID == shoppingCartId);

                shoppingCarts.Remove(shoppingCart);

                return RedirectToAction("Index");
            }
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult DeleteProduct(Guid? shoppingCartId)
        {
            var shoppingCarts = Session["ShoppingCart"] as List<SHOPPINGCART>;

            if (shoppingCarts != null)
            {
                var shoppingCart = shoppingCarts.FirstOrDefault(s => s.CART_ID == shoppingCartId);

                if (shoppingCart != null)
                {
                    shoppingCarts.Remove(shoppingCart);
                    int newCartCount = CalculateCartCount();
                    return Json(new { success = true, message = "Xóa sản phẩm thành công", cartCount = newCartCount });
                }
            }

            return Json(new { success = false, message = "Không thể xóa sản phẩm" });
        }

        private int CalculateCartCount()
        {
            // Lấy danh sách giỏ hàng từ Session hoặc database
            var shoppingCarts = Session["ShoppingCart"] as List<SHOPPINGCART>;

            if (shoppingCarts != null)
            {
                int cartCount = (int)shoppingCarts.Sum(cart => cart.QUANTITY);
                return cartCount;
            }

            return 0;
        }


    }
}
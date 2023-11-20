using FlowerShop.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlowerShop.Models
{
    public class DetailModel
    {
        public FLOWER Flower { get; set; }
        public List<DISCOUNTCODE> DiscountCodes { get; set; }
        public List<FLOWER> Flowers { get; set; }
        public List<SHOPPINGCART> ShoppingCarts { get; set; }
    }
}
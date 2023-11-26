using FlowerShop.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlowerShop.Models
{
    public class OrderModel
    {
        public List<SHOPPINGCART> ShoppingCarts { get; set; }
        public List<ORDERDETAIL> OrderDetails { get; set; }
        public USER User { get; set; }
        public ORDER Order { get; set; }
        public DISCOUNTCODE DiscountCode { get; set; }
        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }
        public int? ProvinceCode { get; set; }

        public OrderModel()
        {
            Order = new ORDER();
        }
    }
}
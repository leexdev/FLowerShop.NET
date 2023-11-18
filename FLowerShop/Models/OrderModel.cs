using FlowerShop.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FLowerShop.Models
{
    public class OrderModel
    {
        public List<SHOPPINGCART> ShoppingCarts { get; set; }
        public List<ORDERDETAIL> OrderDetails { get; set; }
        public List<ORDERHISTORY> OrderHistories { get; set; }
        public USER User { get; set; }
        public ORDER Order { get; set; }
        public DISCOUNTCODE DiscountCode { get; set; }
        public OrderModel()
        {
            Order = new ORDER();
        }
    }
}
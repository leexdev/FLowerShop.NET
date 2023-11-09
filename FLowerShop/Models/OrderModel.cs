using FLowerShop.Context;
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
        public USER User { get; set; }
        public ORDER Order { get; set; }

        public OrderModel()
        {
            Order = new ORDER();
        }
    }
}
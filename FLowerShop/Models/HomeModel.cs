using FLowerShop.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FLowerShop.Models
{
    public class HomeModel
    {
        public List<FLOWERTYPE> FlowerTypes { get; set; }
        public List<FLOWER> Flowers { get; set; }
    }
}
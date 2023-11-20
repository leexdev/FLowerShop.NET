using FlowerShop.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlowerShop.Models
{
    public class HomeModel
    {
        public List<FLOWERTYPE> FlowerTypes { get; set; }
        public List<FLOWER> Flowers { get; set; }
    }
}
using FlowerShop.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlowerShop.Models
{
    public class SearchModel
    {
        public List<FLOWER> Flowers { get; set; }
        public List<FLOWERTYPE> FlowersType { get; set; }
    }
}
using FLowerShop.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FLowerShop.Models
{
    public class SearchModel
    {
        public List<FLOWER> Flowers { get; set; }
        public List<FLOWERTYPE> FlowersType { get; set; }
    }
}
﻿using FLowerShop.Service;
using System.Web;
using System.Web.Mvc;

namespace FLowerShop
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new NoCacheAttribute());
        }
    }
}

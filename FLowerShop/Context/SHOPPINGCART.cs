﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FlowerShop.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public partial class SHOPPINGCART
    {
        public System.Guid CART_ID { get; set; }
        public Nullable<System.Guid> USER_ID { get; set; }
        public Nullable<System.Guid> FLOWER_ID { get; set; }

        [DisplayName("Số lượng")]
        public Nullable<decimal> QUANTITY { get; set; }

        [DisplayName("Tổng tiền")]
        public Nullable<decimal> SUBTOTAL { get; set; }
        public bool DELETED { get; set; }
    
        public virtual FLOWER FLOWER { get; set; }
        public virtual USER USER { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FLowerShop.Context
{
    using System;
    using System.Collections.Generic;
    
    public partial class ORDERDETAIL
    {
        public System.Guid ORDERDETAIL_ID { get; set; }
        public Nullable<System.Guid> ORDER_ID { get; set; }
        public Nullable<System.Guid> FLOWER_ID { get; set; }
        public Nullable<decimal> QUANTITY { get; set; }
        public Nullable<decimal> SUBTOTAL { get; set; }
        public bool DELETED { get; set; }
    
        public virtual FLOWER FLOWER { get; set; }
        public virtual ORDER ORDER { get; set; }
    }
}

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
    
    public partial class USER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public USER()
        {
            this.ORDERS = new HashSet<ORDER>();
            this.SHOPPINGCARTs = new HashSet<SHOPPINGCART>();
        }
    
        public System.Guid USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public string USER_EMAIL { get; set; }
        public string USER_PASSWORD { get; set; }
        public Nullable<bool> DELETED { get; set; }
        public string USER_PHONE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDER> ORDERS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SHOPPINGCART> SHOPPINGCARTs { get; set; }
    }
}

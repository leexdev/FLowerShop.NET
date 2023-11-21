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
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class FLOWER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FLOWER()
        {
            this.SHOPPINGCARTs = new HashSet<SHOPPINGCART>();
            this.ORDERDETAILS = new HashSet<ORDERDETAIL>();
        }
    
        public System.Guid FLOWER_ID { get; set; }

        [DisplayName("Tên sản phẩm")]
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [MinLength(3, ErrorMessage = "Tên sản phẩm phải có ít nhất {1} ký tự")]
        [MaxLength(100, ErrorMessage = "Giới hạn {1} ký tự")]
        public string FLOWER_NAME { get; set; }

        [DisplayName("Hình ảnh")]
        [Required(ErrorMessage = "Hình ảnh không được để trống")]
        public string FLOWER_IMAGE { get; set; }

        [DisplayName("Mô tả")]
        [MaxLength(2000, ErrorMessage = "Giới hạn {1} ký tự")]
        public string DESCRIPTION { get; set; }

        [DisplayName("Giá cũ")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Vui lòng nhập một số hợp lệ")]
        public Nullable<decimal> OLD_PRICE { get; set; }

        [DisplayName("Giá mới")]
        [Required(ErrorMessage = "Vui lòng nhập giá tiền")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Vui lòng nhập một số hợp lệ")]
        public Nullable<decimal> NEW_PRICE { get; set; }

        [DisplayName("Loại hoa")]
        [Required(ErrorMessage = "Vui lòng chọn loại hoa")]
        public Nullable<System.Guid> FLOWERTYPE_ID { get; set; }
        public bool DELETED { get; set; }
    
        public virtual FLOWERTYPE FLOWERTYPE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SHOPPINGCART> SHOPPINGCARTs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDERDETAIL> ORDERDETAILS { get; set; }
    }
}

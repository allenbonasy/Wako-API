//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DTransAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ShopLogin
    {
        public int LoginID { get; set; }
        public Nullable<int> ShopID { get; set; }
        public string UserEmail { get; set; }
        public string Password { get; set; }
        public string Repeat_Password { get; set; }
    
        public virtual Shop Shop { get; set; }
    }
}
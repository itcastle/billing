//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GestionCommerciale.DomainModel.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Telephone
    {
        public int TELEPHONEID { get; set; }
        public Nullable<int> SupplierID { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public string TELEPHONENUMBER { get; set; }
        public string TELEPHONETYPE { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Provider Provider { get; set; }
    }
}

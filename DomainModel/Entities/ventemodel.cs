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
    
    public partial class ventemodel
    {
        public int VenteModelID { get; set; }
        public string VenteModelName { get; set; }
        public Nullable<System.DateTime> VenteModelDate { get; set; }
        public Nullable<int> ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductFamille { get; set; }
        public Nullable<int> ProductQte { get; set; }
        public Nullable<double> ProductPriceHT { get; set; }
        public Nullable<double> VenteTVA { get; set; }
        public string ProductDescription { get; set; }
    }
}

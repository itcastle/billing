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
    
    public partial class Purchase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Purchase()
        {
            this.Commands = new HashSet<Command>();
            this.PurchaseStores = new HashSet<PurchaseStore>();
        }
    
        public int PurchaseID { get; set; }
        public Nullable<int> SupplierID { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        public string CommandeNum { get; set; }
        public Nullable<System.DateTime> CommandeDate { get; set; }
        public string FactureNum { get; set; }
        public string ModePaiement { get; set; }
        public string ModeAchat { get; set; }
        public string EtatPaiement { get; set; }
        public Nullable<double> FacturePrice { get; set; }
        public Nullable<double> PurchaseMontant { get; set; }
        public Nullable<double> TvaValue { get; set; }
        public Nullable<double> Timbre { get; set; }
        public Nullable<float> Discount { get; set; }
        public Nullable<bool> PurchaseValidite { get; set; }
        public string Description { get; set; }
        public Nullable<int> Status { get; set; }
        public bool IsCommand { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Command> Commands { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Provider Provider { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseStore> PurchaseStores { get; set; }
    }
}

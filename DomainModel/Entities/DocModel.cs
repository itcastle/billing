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
    
    public partial class DocModel
    {
        public int DocModelID { get; set; }
        public byte[] DocFile { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}

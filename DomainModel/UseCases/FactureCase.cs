using System;
using System.Collections.Generic;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.DomainModel;

namespace GestionCommerciale.DomainModel
{
    public class FactureCase
    {
        public String Numbre;
        public Facture Facture;
        public Order Order;
        public Decimal Tva;
        public Decimal TotalHt = 0;
        public Decimal Ttc = 0;
        public Decimal Discount = 0;
        public Decimal Timbre = 0;
    }
}

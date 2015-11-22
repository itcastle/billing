using System;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.UseCases
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

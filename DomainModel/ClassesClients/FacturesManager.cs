using System;
using System.Collections.Generic;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.DomainModel.Invoice;
using GestionCommerciale.DomainModel.UseCases;

namespace GestionCommerciale.DomainModel.ClassesClients
{
    public class FacturesClient
    {
        private GcdbEntities _gestionDb;
      
        public Facture GetFactureByOrdre(Order ordre)
        {
            try
            {
                _gestionDb = new GcdbEntities();

                return _gestionDb.Factures.First(fact => fact.OrdresID == ordre.OrderID);
            }
            catch (Exception)
            {
                return null;
            }
        }
     
        public String AddFacture(Facture fact)
        {

            _gestionDb = new GcdbEntities();
            if (IsFactureExist(fact)) return "Facture existe déjà";

            _gestionDb.Factures.Add(fact);
            _gestionDb.SaveChanges();
            
           
            return "Facture ajouter avec succes";
        }

        private static bool IsFactureExist(Facture fact)
        {
             try
            {
              var  thegestionDb = new GcdbEntities();

              var requete = from t in thegestionDb.Factures
                          where t.FactureID.Equals(fact.FactureID)
                          select t;

            if (requete.ToList().Count != 0) return true;
            }
             catch (Exception)
             {

                 return false;
             }

            return false;
        }

        public int GetFactureNumber()
        {
            _gestionDb = new GcdbEntities();
            SettingsClient sc = new SettingsClient();
            Setting settings = sc.GetSetting();
            int nextNumber = 0;
            if (settings != null)
            {
                nextNumber = Convert.ToInt16(settings.FactureNumber) + 1;
            }
            return nextNumber;
        }


        public List<InvoiceDetail> GetFactureDetails(FactureCase fc)
        {
            var details = new List<InvoiceDetail>();
            try
            {
                foreach (OrderDetail od in fc.Facture.Order.OrderDetails)
                {
                    OrderDetail getDetail = od;
                    Customer getCustomer = fc.Facture.Order.Customer;
                    Product getProduct = getDetail.Product;
                    Facture getFacture = fc.Facture;
                    if (getFacture.FactureDate == null) continue;

                    InvoiceDetail id = new InvoiceDetail
                    {
                        InvoiceDate = (DateTime) getFacture.FactureDate,
                        InvoiceNumber = getFacture.FactureID,
                        PayMode = fc.Facture.TypePayment,
                        RS = getCustomer.CompanyName,
                        ProductNAme = getProduct.ProductName,
                        Quantity = getDetail.Quantity,
                        UnitPrice = getDetail.UnitPrice,
                        Amount = getDetail.UnitPrice*getDetail.Quantity,
                        TotalHT = fc.TotalHt,
                        PTVA = fc.Tva*fc.TotalHt,
                        TVA = fc.Tva,
                        Total = fc.Ttc,
                        Timbre = fc.Timbre
                    };
                    details.Add(id);
                }
                return details;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public int RetrieveFactureNumber(string format)
        {
            string s = format.Remove(0, 5);
            return int.Parse(s);
        }
        
        public void AddDocToFacture(Facture invoice , byte[] doc)
        {
            _gestionDb = new GcdbEntities();
            var fact = _gestionDb.Factures.FirstOrDefault(f => f.FactureID == invoice.FactureID);
            Document d = new Document()
            {
                Type = "facture",DocFile = doc
            };
            fact.Document = d;
            _gestionDb.SaveChanges();
        }

        public Facture GetFactureById(int id)
        {
            _gestionDb = new GcdbEntities();
            var fact = _gestionDb.Factures.FirstOrDefault(f => f.FactureID == id);
            return fact;
        }

    }

}



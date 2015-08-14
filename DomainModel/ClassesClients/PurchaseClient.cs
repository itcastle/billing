using System;
using System.Collections.Generic;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel
{
    public class PurchaseClient
    {
        private GcdbEntities _gestionDb;
        public List<Purchase> GetPurchase()
        {
            try
            {
               _gestionDb = new GcdbEntities();

                return _gestionDb.Purchases.Where(c => c.Status == 0).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Double GetTotalPurchase(Purchase p)
        {
            try
            {
               _gestionDb = new GcdbEntities();

                var list = from purchase in _gestionDb.PurchaseStores
                           where (purchase.PurchaseID == p.PurchaseID)
                           select purchase;
                Double somme = 0;

              //  foreach (PurchaseStore pd in list) somme += (Double)pd.UnitPrice * pd.Quantity;
                    

                return somme;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public String DesactivatePurchase(Purchase purchase)
        {

           _gestionDb = new GcdbEntities();

            purchase.Status = 1;
            MajPurchase(purchase);

            return "Achat désactiver avec succes";
        }



        public List<Purchase> GetPurchase(DateTime debut, DateTime fin)
        {
            try
            {

               _gestionDb = new GcdbEntities();
                var list = (from purchase in _gestionDb.Purchases
                            where (purchase.PurchaseDate <= fin) && (purchase.PurchaseDate >= debut)
                            select purchase).ToList();

                return list;
            }
            catch (Exception)
            {
                return new List<Purchase>();
            }
        }


        public Purchase GetPurchaseById(int id)
        {
            try
            {
               _gestionDb = new GcdbEntities();
                return _gestionDb.Purchases.Single(p => p.PurchaseID == id);
            }
            catch (Exception)
            {

                return null;
            }
        }

        public String AddPurchase(Purchase pur)
        {

           _gestionDb = new GcdbEntities();
            if (IsPurchaseExist(pur)) return "Achat existe déjà";

            _gestionDb.Purchases.Add(pur);
            _gestionDb.SaveChanges();

            return "Achat ajouter avec succes";
        }
      
        public String DelPurchase(Purchase pur)
        {

           _gestionDb = new GcdbEntities();
            if (!IsPurchaseExist(pur)) return "Achat n'existante pas";

            Purchase purchase = _gestionDb.Purchases.Single(p => p.PurchaseID == pur.PurchaseID);
            _gestionDb.Purchases.Remove(pur);
            _gestionDb.SaveChanges();


            return "Achat supprimer avec succes";
        }

        private String MajPurchase(Purchase pur)
        {
           _gestionDb = new GcdbEntities();

            Purchase purchase = _gestionDb.Purchases.Single(p => p.PurchaseID == pur.PurchaseID);
            try
            {
                purchase = pur; // si sa marche pas alors attache detache sinon affactation champ par champ
            }
            catch (Exception )
            {
                return "erreur";
            }


            return "Achat modifier avec succes";
        }

        private static bool IsPurchaseExist(Purchase pur)
        {
            try
            {

                GcdbEntities gestionDb = new GcdbEntities();

                var query = from t in gestionDb.Purchases
                          where t.PurchaseID.Equals(pur.PurchaseID)
                          select t;

             return query.Any();
            }
            catch (Exception)
            {
                return false;

                
            }


            return false;
        }

        public List<Purchase> GetPurchasesByProvider(Provider receiveProvider)
        {
            
           _gestionDb=new GcdbEntities();
            var query = from t in _gestionDb.Purchases
                where t.SupplierID == receiveProvider.SupplierID
                select t;
            if(!query.Any())return new List<Purchase>();
            return query.ToList();

        }

        public List<PurchaseStore> GetPurchaseStoresList(Purchase getPurchase)
        {

            try
            {
                _gestionDb = new GcdbEntities();
                var query = from t in _gestionDb.PurchaseStores
                            where t.PurchaseID == getPurchase.PurchaseID
                            select t;
                return !query.Any() ? new List<PurchaseStore>() : query.ToList();
            }
            catch (Exception)
            {
                return new List<PurchaseStore>();
            }
        }

        public string DelPurchase(int purchaseId)
        {

            try
            {
                _gestionDb = new GcdbEntities();
                var query = from t in _gestionDb.Purchases
                            where t.PurchaseID == purchaseId
                            select t;
                if (!query.Any()) return "Essai plus tards!";
                _gestionDb.Purchases.Remove(query.First());
                _gestionDb.SaveChanges();
                return "Supprimé avec succés";
            }
            catch (Exception exe)
            {
                return exe.ToString();
            }
        }

        public PurchaseStore GetPurchaseStoreById(int purchaseStoreId)
        {

            try
            {
                _gestionDb = new GcdbEntities();
                var query = from t in _gestionDb.PurchaseStores
                            where t.PurchaseStoreID == purchaseStoreId
                            select t;
                return !query.Any() ? null : query.First();
            }
            catch (Exception exe)
            {
                return null;
            }
        }
    }

}



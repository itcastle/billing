using System;
using System.Collections.Generic;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.ClassesClients
{
    public class PurchaseDetailsManager
    {
        private GcdbEntities _gestionDb;
        public List<PurchaseStore> GetPurchaseDetails()
        {
            try
            {
                 _gestionDb = new GcdbEntities();

                 return _gestionDb.PurchaseStores.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }


        public PurchaseStore GetPurchaseDetailsById(int idPurchase, int idProduct)
        {
            try
            {
                 _gestionDb = new GcdbEntities();
                 return _gestionDb.PurchaseStores.Single(pd => (pd.PurchaseID == idPurchase) && (pd.ProductID == idProduct));
            }
            catch (Exception)
            {

                return null;
            }
        }

        public String AddPurchaseDetails(PurchaseStore pd)
        {

             _gestionDb = new GcdbEntities();
            if (IsPurchaseDetailsExist(pd)) return "PurchaseDetails existe déjà";

            _gestionDb.PurchaseStores.Add(pd);
            _gestionDb.SaveChanges();

            return "PurchaseDetails ajouter avec succes";
        }

        public String DelPurchaseDetails(PurchaseStore pd)
        {

             _gestionDb = new GcdbEntities();
            if (!IsPurchaseDetailsExist(pd)) return "PurchaseDetails n'existante pas";

            PurchaseStore purchaseDetails = _gestionDb.PurchaseStores.Single(pd1 => (pd1.PurchaseID == pd.PurchaseID) && (pd1.ProductID == pd.ProductID));
            _gestionDb.PurchaseStores.Remove(purchaseDetails);
            _gestionDb.SaveChanges();


            return "PurchaseDetails supprimer avec succes";
        }

        public String MajPurchaseDetails(PurchaseStore pd)
        {
             _gestionDb = new GcdbEntities();

             PurchaseStore purchaseDetails = _gestionDb.PurchaseStores.Single(pd1 => (pd1.PurchaseID == pd.PurchaseID) && (pd1.ProductID == pd.ProductID));
            try
            {
                purchaseDetails = pd; // si sa marche pas alors attache detache sinon affactation champ par champ
            }
            catch (Exception )
            {
                return "erreur";
            }


            return "PurchaseDetails modifier avec succes";
        }

        private static bool IsPurchaseDetailsExist(PurchaseStore pd)
        {
            try
            {
                var gestionDb = new GcdbEntities();

                var query = from pd1 in gestionDb.PurchaseStores
                              where (pd1.PurchaseID == pd.PurchaseID) && (pd1.ProductID == pd.ProductID)
                              select pd1;

               return query.Any();
            }
            catch (Exception)
            {
                return false;

            }
        }

        public static void SaySomthing(string text)
        {
            Console.WriteLine(text);
        }

    }

}



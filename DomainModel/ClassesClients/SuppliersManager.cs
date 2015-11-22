using System;
using System.Collections.Generic;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.ClassesClients
{
    public class SuppliersManager
    {
        private GcdbEntities _gestionDb;
        public List<Customer> GetCustomers()
        {
            try
            {
               _gestionDb = new GcdbEntities();

               return _gestionDb.Customers.Where(c => c.Status == 0).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<Provider> GetTheSuppliers()
        {

            try
            {
                _gestionDb = new GcdbEntities();

                return _gestionDb.Providers.Where(c => c.Status == 0).ToList();
            }
            catch (Exception)
            {
                return null;
            }

        }
        public Double GetTotalPerCustomer(Customer sup)
        {
            try
            {
               _gestionDb = new GcdbEntities();

                var list = from pd in _gestionDb.OrderDetails
                           where (pd.Order.Customer.CustomerID == sup.CustomerID)
                           select pd;
                Double somme = 0;

             //   foreach (PurchaseStore pd in list) somme += (Double)pd.UnitPrice * pd.Quantity;


                return somme;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public String DesactivateProvider(Provider provider)
        {

            try
            {
                _gestionDb = new GcdbEntities();
                var query = from t in _gestionDb.Providers
                    where t.SupplierID == provider.SupplierID
                    select t;
                if (!query.Any()) return "Erreur";
                query.First().Status = 1;
                _gestionDb.SaveChanges();
                return "Fournisseur désactiver avec succés";
            }
            catch (Exception e)
            {

                return "Erreur";
            }
        }

        public Provider GetProviderByName(String companyName)  // s'il ya plusieurs nom alors il faut modifier la valeur de retour par list et signle par where
        {
            try
            {
               _gestionDb = new GcdbEntities();
               return _gestionDb.Providers.Single(s => s.CompanyName.Equals(companyName));
            }
            catch (Exception)
            {
                return null;
            }

        }

      
        public string AddNewProvider(string comanyName, string contactTitle, string contactName, string address,
            string country, string region, string city, string postalCode, string phone, string homePage, string email,
            string fax, string rC, string nF, string nIs, string aI, int status, byte[] photo)
        {
            try
            {
                _gestionDb = new GcdbEntities();
                Provider newProvider = new Provider
                {
                    CompanyName = comanyName,
                    ContactTitle = contactTitle,
                    ContactName = contactName,
                    Address = address,
                    Country = country,
                    Region = region,
                    City = city,
                    PostalCode = postalCode,
                    Photo = photo,
                    HomePage = homePage,
                    Email = email,
                    RC = rC,
                    NF = nF,
                    NIS = nIs,
                    AI = aI,
                    Status = status,
                   
                };
                _gestionDb.Providers.Add(newProvider);
                _gestionDb.SaveChanges();
                Telephone newTelephone = new Telephone
                {
                    SupplierID = newProvider.SupplierID,
                    TELEPHONETYPE = "Fix",
                    TELEPHONENUMBER = fax

                };
                _gestionDb.Telephones.Add(newTelephone);
                _gestionDb.SaveChanges();
                Telephone newTelephone2 = new Telephone
                {
                    SupplierID = newProvider.SupplierID,
                    TELEPHONETYPE = "mobile",
                    TELEPHONENUMBER = phone

                };
                _gestionDb.Telephones.Add(newTelephone2);
                _gestionDb.SaveChanges();
                return "Ajouté avec succés";
            }
            catch (Exception)
            {
                return "Erreur";
            }

        }

        public string UpdateProvider(Provider provider, string comanyName, string contactTitle,
            string contactName, string address, string country, string region, string city,
            string postalCode, string phone, string homePage, string email, string fax, string rC,
            string nF, string nIs, string aI, int status, byte[] photo)
        {

            try
            {
                _gestionDb = new GcdbEntities();
                var query = from t in _gestionDb.Providers
                    where t.SupplierID == provider.SupplierID
                    select t;
                if (!query.Any()) return "Erreur";
                query.First().CompanyName = comanyName;
                query.First().ContactTitle = contactTitle;
                query.First().ContactName = contactName;
                query.First().Address = address;
                query.First().Country = country;
                query.First().Region = region;
                query.First().City = city;
                query.First().PostalCode = postalCode;
                query.First().Photo = photo;
                query.First().HomePage = homePage;
                query.First().Email = email;
                query.First().RC = rC;
                query.First().NF = nF;
                query.First().NIS = nIs;
                query.First().AI = aI;
                query.First().Status = status;
                // remains the phone, voir avec mourad!

                _gestionDb.SaveChanges();
                return "Mise à jour avec succés";
            }
            catch (Exception)
            {
                return "Erreur";
            }
        }

       
    }

}



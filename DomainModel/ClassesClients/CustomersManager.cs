using System;
using System.Collections.Generic;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel

{
    public class CustomersManager
    {
        private GcdbEntities _gestionDb;
        public List<Customer> GetCustomers()
        {
            try
            {
                 _gestionDb = new GcdbEntities();

                return _gestionDb.Customers.Where(c => c.Status==0).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<Customer> GetAllCustomers()
        {
            try
            {
                _gestionDb = new GcdbEntities();

                return _gestionDb.Customers.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Double GetTotalPerCustomer(Customer cust)
        {
            try
            {
                _gestionDb = new GcdbEntities();

                var list = from od in _gestionDb.OrderDetails
                           where (od.Order.CustomerID == cust.CustomerID)
                           select od;
                Double somme = 0;

                foreach (OrderDetail od in list) somme += (Double)od.UnitPrice * od.Quantity;


                return somme;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public Customer GetCustomerByName(String name)  // s'il ya plusieurs nom alors il faut modifier la valeur de retour par list et signle par where
        {
            try
            {
                _gestionDb = new GcdbEntities();
                return _gestionDb.Customers.Single(c => c.CompanyName.Equals(name));
            }
            catch (Exception)
            {
                return null;
            }

        }

        public Customer GetCustomerByCompany(String companyName)   // s'il ya plusieurs nom alors il faut modifier la valeur de retour par list et signle par where
        {
            try
            {
                _gestionDb = new GcdbEntities();
                return _gestionDb.Customers.Single(c => c.CompanyName.Equals(companyName));
            }
            catch (Exception)
            {
                return null;
            }

        }

        public Customer GetCustomerById(int id)
        {
            try
            {
                _gestionDb = new GcdbEntities();
                return _gestionDb.Customers.Single(c => c.CustomerID == id);
            }
            catch (Exception)
            {

                return null;
            }
        }

        public String AddCustomer(Customer c)
        {

            _gestionDb = new GcdbEntities();
            if (IsCustomerExist(c)) return "Client existe déjà";

            _gestionDb.Customers.Add(c);
            _gestionDb.SaveChanges();

            return "Ajouté avec succes";
        }

        public String DesactivateCustomer(Customer cust)
        {

            _gestionDb = new GcdbEntities();
            var query = from t in _gestionDb.Customers
                where t.CustomerID == cust.CustomerID
                select t;
            if (!query.Any()) return "Erreur";
            query.First().Status = 1;
            _gestionDb.SaveChanges();
            return "Client désactivé avec succes";
        }

        public String DelCustomer(Customer c)
        {

            _gestionDb = new GcdbEntities();
            if (!IsCustomerExist(c)) return "Client n'existante pas";
            var customer = _gestionDb.Customers.Single(cus => cus.CustomerID.Equals(c.CustomerID));
            _gestionDb.Customers.Remove(customer);
               
            _gestionDb.SaveChanges();


            return "Client supprimer avec succes";
        }

        private bool IsCustomerExist(Customer c)
        {
         
            try
            {

                _gestionDb = new GcdbEntities();
            
            var query = from t in _gestionDb.Customers
                          where (t.CompanyName.Equals(c.CompanyName) )
                          select t;

             return query.Any();
            
            }
            catch (Exception)
            {

                return false;
            }

                return false;
        }

        public static void SaySomthing(string text)
        {
            Console.WriteLine(text);
        }

        public string AddNewCustomer(string companName, string contactTitle, string contactName, string address, string country, string region, string city, string postalCode, string phone, string homePage, string email, string fax, string rC, string nF, string nIs, string aI, int status, byte[] photo)
        {

            try
            {
                _gestionDb = new GcdbEntities();
                Customer newCustomer = new Customer();
                newCustomer.CompanyName = companName;
                newCustomer.ContactTitle = contactTitle;
                newCustomer.ContactName = contactName;
                newCustomer.Address = address;
                newCustomer.Country = country;
                newCustomer.Region = region;
                newCustomer.City = city;
                newCustomer.PostalCode = postalCode;
                newCustomer.Phone = phone;
                newCustomer.HomePage = homePage;
                newCustomer.Email = email;
                newCustomer.Fax = fax;
                newCustomer.RC = rC;
                newCustomer.NF = nF;
                newCustomer.NIS = nIs;
                newCustomer.AI = aI;
                newCustomer.Photo = photo;
                newCustomer.Status = status;
                _gestionDb.Customers.Add(newCustomer);
                _gestionDb.SaveChanges();
                return "Ajouté avec succés";
            }
            catch (Exception)
            {
                return "Erreur";
            }

        }

        public string UpdateCustomer(Customer cust, string companName, string contactTitle, string contactName,
            string address, string country, string region, string city, string postalCode, string phone, 
            string homePage,
            string email, string fax, string rC, string nF, string nIs, string aI, int status, byte[] photo)
        {

            try
            {
                _gestionDb = new GcdbEntities();
                var query = from t in _gestionDb.Customers
                    where t.CustomerID == cust.CustomerID
                    select t;
                if (!query.Any()) return "Erreur";
                query.First().CompanyName = companName;
                query.First().ContactTitle = contactTitle;
                query.First().ContactName = contactName;
                query.First().Address = address;
                query.First().Country = country;
                query.First().Region = region;
                query.First().City = city;
                query.First().PostalCode = postalCode;
                query.First().Phone = phone;
                query.First().HomePage = homePage;
                query.First().Email = email;
                query.First().Fax = fax;
                query.First().RC = rC;
                query.First().NF = nF;
                query.First().NIS = nIs;
                query.First().AI = aI;
                query.First().Photo = photo;
                query.First().Status = status;
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



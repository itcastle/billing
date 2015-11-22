using System;
using System.Collections.Generic;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.ClassesClients
{
    public class OrderClient
    {
        private GcdbEntities _gestionDb;
        public List<Order> GetOrder()
        {
            try
            {
                 _gestionDb = new GcdbEntities();

                return _gestionDb.Orders.Where(c => c.Status == 0).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Double GetTotalOrders(Order ordre)
        {
            try
            {
                _gestionDb = new GcdbEntities();

                var list = from od in _gestionDb.OrderDetails
                           where (ordre.OrderID == od.OrderID)
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

        public int GetCountOrder()
        {
            try
            {
                _gestionDb = new GcdbEntities();

                return _gestionDb.Orders.ToList().Count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

      

        public List<Order> GetOrderNotFactured()
        {
            try
            {

                _gestionDb = new GcdbEntities();
                var list = (from ordre in _gestionDb.Orders
                             where ordre.Factures.Count == 0 
                             select ordre).ToList();
                                
                return list;
            }
            catch (Exception)
            {
             return null;
            }
        }

        public List<Order> GetOrderFactured()
        {
            try
            {

                _gestionDb = new GcdbEntities();
                var list = (from ordre in _gestionDb.Orders
                            where ordre.Factures.Count != 0
                            select ordre).ToList();

                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }

 

        public Order GetOrderById(int id)
        {
            try
            {
                _gestionDb = new GcdbEntities();
                return _gestionDb.Orders.Single(o => o.OrderID == id);
            }
            catch (Exception)
            {

                return null;
            }
        }

        public int GetLastOrder(Order ordre)
        {
            try
            {
                _gestionDb = new GcdbEntities();
                return _gestionDb.Orders.ToList().Last().OrderID;
            }
            catch (Exception)
            {

                return -1;
            }
        }


        public String AddOrder(Order order)
        {

            _gestionDb = new GcdbEntities();
            if (IsOrderExist(order)) return "Ordre existe déjà";

            _gestionDb.Orders.Add(order);
            _gestionDb.SaveChanges();

            return "Order ajouter avec succes";
        }

   

       
        private static bool IsOrderExist(Order order)
        {
            try
            {

                GcdbEntities gestionDb = new GcdbEntities();

                var requete = from t in gestionDb.Orders
                              where t.OrderID.Equals(order.OrderID)
                              select t;

                 return requete.Any();
            }
            catch (Exception)
            {
                return false;
            }


          
        }
  

    
    }

}



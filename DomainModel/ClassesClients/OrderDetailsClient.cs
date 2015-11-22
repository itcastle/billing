using System;
using System.Collections.Generic;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.ClassesClients
{
    public class OrderDetailsClient
    {
        GcdbEntities _gestionDb;
        public List<OrderDetail> GetOrderDetails()
        {
         
            try
            {
                 _gestionDb = new GcdbEntities();

                return _gestionDb.OrderDetails.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }


        public OrderDetail GetOrderDetailsById(int IDOrder, int IDProduct)
        {
            try
            {
                _gestionDb = new GcdbEntities();
                return _gestionDb.OrderDetails.Single(od => (od.OrderID == IDOrder)&&(od.ProductID == IDProduct) );
            }
            catch (Exception)
            {

                return null;
            }
        }

        public List<OrderDetail> GetOrderDetailsByOrder(Order ordre)
        {
            try
            {
                _gestionDb = new GcdbEntities();
                return _gestionDb.OrderDetails.Where(od => (od.OrderID == ordre.OrderID)).ToList();
            }
            catch (Exception)
            {

                return null;
            }
        }

        public String AddOrderDetails(OrderDetail od)
        {

            _gestionDb = new GcdbEntities();
            if (IsOrderDetailsExist(od)) return "OrderDetails existe déjà";

            _gestionDb.OrderDetails.Add(od);
            _gestionDb.SaveChanges();

            return "OrderDetails ajouter avec succes";
        }

        public String DelOrderDetails(OrderDetail od)
        {

            _gestionDb = new GcdbEntities();
            if (!IsOrderDetailsExist(od)) return "OrderDetails n'existante pas";

            OrderDetail orderDetails = _gestionDb.OrderDetails.Single(od1 => (od1.OrderID == od.OrderID) && (od1.ProductID == od.ProductID));
            _gestionDb.OrderDetails.Remove(orderDetails);
            _gestionDb.SaveChanges();


            return "OrderDetails supprimer avec succes";
        }

        public String MajOrderDetails(OrderDetail od)
        {
            _gestionDb = new GcdbEntities();

            OrderDetail orderDetails = _gestionDb.OrderDetails.Single(od1 => (od1.OrderID == od.OrderID) && (od1.ProductID == od.ProductID));
            try
            {
                orderDetails = od; // si sa marche pas alors attache detache sinon affactation champ par champ
            }
            catch (Exception )
            {
                return "erreur";
            }


            return "OrderDetails modifier avec succes";
        }

        private static bool IsOrderDetailsExist(OrderDetail od)
        {
            try
            {

                GcdbEntities ggestionDb = new GcdbEntities();

                var requete = from od1 in ggestionDb.OrderDetails
                              where (od1.OrderID == od.OrderID) && (od1.ProductID == od.ProductID)
                              select od1;

             return requete.Any();
            }
            catch (Exception)
            {
                return false;

            }
        }

        public static void saySomthing(string text)
        {
            Console.WriteLine(text);
        }

    }

}



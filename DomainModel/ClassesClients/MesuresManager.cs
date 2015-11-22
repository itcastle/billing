using System;
using System.Collections.Generic;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.ClassesClients
{
    public class CategorysManager
    {
        private GcdbEntities _gestionDb;
        public List<ProductMeasure> GetMeasures()
        {
            try
            {

             _gestionDb = new GcdbEntities();
             return _gestionDb.ProductMeasures.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ProductMeasure GetMeasureByName(String name)
        {
            try
            {
                _gestionDb = new GcdbEntities();

                return _gestionDb.ProductMeasures.Single(m => m.ProMeasureType.Equals(name));

            }
            catch (Exception)
            {

                return null;
            }
        }


        public String AddMeasure(ProductMeasure mes)
        {
            _gestionDb = new GcdbEntities();
            if (IsMeasureExist(mes)) return "Mesure existe déjà";

            _gestionDb.ProductMeasures.Add(mes);
            _gestionDb.SaveChanges();

            return "Mesure ajouter avec succes";
        }

        public String DelMeasure(ProductMeasure mes)
        {

            _gestionDb = new GcdbEntities();
            if (!IsMeasureExist(mes)) return "Mesure n'existante pas";

            ProductMeasure mesure = _gestionDb.ProductMeasures.Single(m => m.ProMeasureType.Equals(mes.ProMeasureType));
            _gestionDb.ProductMeasures.Remove(mesure);
            _gestionDb.SaveChanges();


            return "Mesure supprimer avec succes";
        }

        public String MajMeasure(ProductMeasure mes)
        {
            _gestionDb = new GcdbEntities();

            if (!IsMeasureExist(mes)) return "Mesure n'existante pas";

            ProductMeasure mesure = _gestionDb.ProductMeasures.Single(m => m.ProMeasureType.Equals(mes.ProMeasureType));
            try
            {
                mesure = mes; //si sa merche pas alors soi champ par champ soi attache detache
            }
            catch (Exception )
            {
                return "erreur";
            }

            return "Mesure modifier avec succes";
        }

        private bool IsMeasureExist(ProductMeasure mes)
        {
            try
            {


                _gestionDb = new GcdbEntities();

            var requete = from mesure in _gestionDb.ProductMeasures
                          where mesure.ProMeasureType.Equals(mes.ProMeasureType)
                          select mesure;

            if (requete.ToList().Count != 0) return true;
            }
            catch (Exception)
            {

                return false;
            }

            return false;
        }

        public bool IsMeasureExist(String nom)
        {
            try
            {


                _gestionDb = new GcdbEntities();

                var requete = from mesure in _gestionDb.ProductMeasures
                              where mesure.ProMeasureType.Equals(nom)
                              select mesure;

                if (requete.ToList().Count != 0) return true;
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
        public static ProductMeasure NewestMeasure { get; set; }

    }

}



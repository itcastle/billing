using System;
using System.Collections.Generic;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel
{
    public class TvaClient
    {
        private GcdbEntities _gestionDb;
        public List<TVA> GetTva()
        {
            try
            {
                _gestionDb = new GcdbEntities();

                return _gestionDb.TVAs.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<float> GetTvaValues()
        {
            try
            {
                _gestionDb = new GcdbEntities();
                var query = from t in _gestionDb.TVAs
                    select t.TauxTVA;
                if (!query.Any()) return new List<float>();
                return query.ToList();
            }
            catch (Exception)
            {
                return new List<float>();
            }
        }
        public TVA GetTvaByName(String name)
        {
            try
            {
                _gestionDb = new GcdbEntities();

                return _gestionDb.TVAs.Single(tva => tva.Description.Equals(name));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public TVA GetTvabyId(int id)
        {
            try
            {
                _gestionDb = new GcdbEntities();

                return _gestionDb.TVAs.Single(tva => tva.TVAID == id );
            }
            catch (Exception)
            {
                return null;
            }
        }

        public String AddTva(TVA tva)
        {
            _gestionDb = new GcdbEntities();
            if (IsTvaExist(tva)) return "TVA existe déjà";

            _gestionDb.TVAs.Add(tva);
            _gestionDb.SaveChanges();

            return "TVA ajouter avec succes";
        }

        public String DelTva(TVA tva)
        {

            _gestionDb = new GcdbEntities();
            if (!IsTvaExist(tva)) return "TVA n'existante pas";
            var tva1 = _gestionDb.TVAs.Single(t => t.TVAID == tva.TVAID);
            _gestionDb.TVAs.Remove(tva);
            _gestionDb.SaveChanges();


            return "TVA supprimer avec succes";
        }

        public String Majtva(TVA tva)
        {
            _gestionDb = new GcdbEntities();

            if (!IsTvaExist(tva)) return "TVA n'existante pas";

            TVA tva1 = _gestionDb.TVAs.Single(t => t.TVAID == tva .TVAID);
            try
            {
                // si afféctation objet à objet marche bien alors c mieu d'utiliser tva1= tva;
                tva1.TauxTVA = tva.TauxTVA;
                tva1.Description = tva.Description;
                
            }
            catch (Exception )
            {
                return "erreur";
            }

           return "TVA modifier avec succes";
        }

        private static bool IsTvaExist(TVA tva)
        {

            GcdbEntities thegestionDb = new GcdbEntities();

            var requete = from t in thegestionDb.TVAs
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                          where t.TauxTVA == tva.TauxTVA
                          select t;

            if (requete.ToList().Count != 0) return true;


            return false;
        }

        public static bool IsTvaExist(double tva)
        {

            GcdbEntities gestionDb = new GcdbEntities();
            double tvva = tva;
            var requete = from t in gestionDb.TVAs
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                          where t.TauxTVA == tvva
                          select t;




            return requete.Any();
        }


        public static void saySomthing(string text)
        {
            Console.WriteLine(text);
        }

    }

}



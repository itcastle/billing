using System.Collections.Generic;
using System.Data;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.ClassesClients
{
    public class MeasureManager
    {
        private GcdbEntities _gestionDb;
        public List<string> GetMeasureList()
        {
             _gestionDb = new GcdbEntities();
            var query = from t in _gestionDb.ProductMeasures
                        orderby t.ProMeasureUnit ascending
                        select t.ProMeasureUnit;
            return query.Any() ? query.ToList() : null;
        }
         
        internal string GetMeasureById(int? measureId)
        {
             _gestionDb = new GcdbEntities();
            var query = from t in _gestionDb.ProductMeasures
                        where t.ProMeasureID == measureId
                select t;
            return query.Any() ? query.First().ProMeasureUnit : "";
        }

        public void AddMeasure(string name)
        {
             _gestionDb = new GcdbEntities();
            var theMeasure = new ProductMeasure
            {
              
                ProMeasureUnit = name
            };
            _gestionDb.ProductMeasures.Add(theMeasure);
            _gestionDb.SaveChanges();
        }
         
        public void RemoveMeasure(int measureId)
        {
             _gestionDb = new GcdbEntities();
            var query = from t in _gestionDb.ProductMeasures
                        where t.ProMeasureID == measureId
                select t;
            if (!query.Any()) return;
            _gestionDb.ProductMeasures.Remove(query.First());
            _gestionDb.SaveChanges();
        }

        public void UpdateMeasure(int measureId, string name)
        {
             _gestionDb = new GcdbEntities();
            var query = from t in _gestionDb.ProductMeasures
                        where t.ProMeasureID == measureId
                select t;
            if (!query.Any()) return;
            query.First().ProMeasureUnit = name;
            _gestionDb.SaveChanges();
        }

        public DataTable GetMeasureDataTable()
        {
             _gestionDb = new GcdbEntities();
            var query = from t in _gestionDb.ProductMeasures
                select t;

            var dataTable = new DataTable();
            dataTable.Columns.Add("N°", typeof (int));
            dataTable.Columns.Add("Nom", typeof (string));

            if (!query.Any()) return dataTable;
            foreach (var entity in query)
            {
                var row = dataTable.NewRow();
                row["N°"] = entity.ProMeasureID;
                row["Nom"] = entity.ProMeasureUnit;

                dataTable.Rows.Add(row);
            }
            return dataTable;
        }
    }
}
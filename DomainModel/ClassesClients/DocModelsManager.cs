using System;
using System.Collections.Generic;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.ClassesClients
{
    public class DocModelsManager
    {
        readonly GcdbEntities _gcdb = new GcdbEntities();
        public List<DocModel> GetDocModelsByType(string type)
        {

            try
            {
                var models = _gcdb.DocModels.Where(m => m.Type == type);
                return models.ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
}

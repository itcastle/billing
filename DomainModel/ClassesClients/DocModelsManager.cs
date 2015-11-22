using System.Collections.Generic;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.ClassesClients
{
    public class DocModelsManager
    {
        GcdbEntities db = new GcdbEntities();
        public List<DocModel> getDocModelsByType(string type)
        {

            var models = db.DocModels.Where(m => m.Type == type);
            return models.ToList();
        }

    }
}

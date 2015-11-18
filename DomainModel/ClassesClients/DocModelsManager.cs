using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel
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

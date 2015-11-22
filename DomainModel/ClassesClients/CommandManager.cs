using System;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.ClassesClients
{
    public class CommandsManager
    {
        public Command GetCommandByID(int id)
        {
            GcdbEntities Db = new GcdbEntities();
            Command cmd = Db.Commands.FirstOrDefault(c => c.CommandID == id);
            return cmd;
        }

        public Command CreateCommand(Purchase ThePurchase, DateTime DateCreation,string CommandNum )
        {
            Command cmd = new Command()
            {
                PurchaseID = ThePurchase.PurchaseID,
                Date = DateCreation,
                Num = CommandNum
            };
            GcdbEntities Db = new GcdbEntities();
            Db.Commands.Add(cmd);
            Db.SaveChanges();
            return cmd;
        }

        public void AddDocToCommand(Command TheCommand, byte[] doc)
        {
            GcdbEntities _gestionDb = new GcdbEntities();
            var fact = _gestionDb.Commands.FirstOrDefault(c => c.CommandID == TheCommand.CommandID);
            Document d = new Document()
            {
                Type = "command",
                DocFile = doc
            };
            fact.Document = d;
            _gestionDb.SaveChanges();
        }

    }
}

using System;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.ClassesClients
{
    public class CommandsManager
    {
        public Command GetCommandByID(int id)
        {
            GcdbEntities gcdb = new GcdbEntities();
            Command cmd = gcdb.Commands.FirstOrDefault(c => c.CommandID == id);
            return cmd;
        }

        public Command CreateCommand(Purchase thePurchase, DateTime dateCreation,string commandNum )
        {
            try
            {
                Command newCommand = new Command()
                {
                    PurchaseID = thePurchase.PurchaseID,
                    Date = dateCreation,
                    Num = commandNum
                };
                GcdbEntities gcdb = new GcdbEntities();
                gcdb.Commands.Add(newCommand);
                gcdb.SaveChanges();
                return newCommand;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void AddDocToCommand(Command theCommand, byte[] doc)
        {
            try
            {
                GcdbEntities gcdb = new GcdbEntities();
                var fact = gcdb.Commands.FirstOrDefault(c => c.CommandID == theCommand.CommandID);
                if (fact == null) return;
                Document d = new Document()
                {
                    Type = "command",
                    DocFile = doc
                };
                fact.Document = d;
                gcdb.SaveChanges();
            }
            catch (Exception e)
            {
                
                //
            }
        }

    }
}

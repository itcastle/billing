using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.ClassesClients
{
    public class ClientManager
    {
        public string AddClient(string clientRs, string clientRc, string clientNis, string clientNif,
            string clientAi, string clientAddress, string clientMobile, string clientFix,
            string clientFirstName,
            string clientLastName, string clientEmail, DateTime dateinsertion, int professionID,
            int clientType, string clientFax)
        {
            try
            {
                if (IsClientExist(clientRs))
                    return "client exist";
                var newClient = new client
                {
                    ClientRS = clientRs,
                    ClientRC = clientRc,
                    ClientNIS = clientNis,
                    ClientNIF = clientNif,
                    ClientAI = clientAi,
                    ClientAddress = clientAddress,
                    ClientFirstName = clientFirstName,
                    ClientLastName = clientLastName,
                    ClientEmail = clientEmail,
                    ClientTypeID = clientType,
                    ClientProfessionID = professionID,
                    ClientInsertionDate = dateinsertion,
                    ClientMobile = clientMobile,
                    ClientFix = clientFix,
                    ClientFax = clientFax
                };


                var gp = new GcdbEntities();
                gp.clients.Add(newClient);
                gp.SaveChanges();
                return "Client ajouter avec succés";
            }
            catch (Exception e)
            {
                return "Erreur";
            }
        }

        private static bool IsClientExist(string clientRs)
        {
            var gp = new GcdbEntities();
            try
            {
                var query = from t in gp.clients
                    where t.ClientRS == clientRs
                    select t;
                return query.Any();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string UpdateClient(int clientID, string clientRs, string clientRc, string clientNis, string clientNif,
            string clientAi, string clientAddress, string clientMobile, string clientFix,
            string clientFirstName, string clientLastName, string clientEmail,
            DateTime dateinsertion, int professionID, int clientType, string clientFax)
        {
            try
            {
                var gp = new GcdbEntities();
                var query = from t in gp.clients
                    where t.ClientID == clientID
                    select t;
                if (!query.Any()) return "Erreur";
                query.First().ClientRS = clientRs;
                query.First().ClientRC = clientRc;
                query.First().ClientNIS = clientNis;
                query.First().ClientNIF = clientNif;
                query.First().ClientAI = clientAi;
                query.First().ClientAddress = clientAddress;
                query.First().ClientFirstName = clientFirstName;
                query.First().ClientLastName = clientLastName;
                query.First().ClientEmail = clientEmail;
                query.First().ClientTypeID = clientType;
                query.First().ClientProfessionID = professionID;
                query.First().ClientInsertionDate = dateinsertion;
                query.First().ClientMobile = clientMobile;
                query.First().ClientFix = clientFix;
                query.First().ClientFax = clientFax;

                gp.SaveChanges();
                return "Mise à jour avec succés";
            }
            catch (Exception e)
            {
                return "Erreur";
            }
        }

        private DataTable GetClientDataTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("ID", typeof (int));
            dataTable.Columns.Add("RS", typeof (string));
            dataTable.Columns.Add("RC", typeof (string));
            dataTable.Columns.Add("NF", typeof (string));
            dataTable.Columns.Add("NIS", typeof (string));
            dataTable.Columns.Add("AI", typeof (string));
            dataTable.Columns.Add("Nom", typeof (string));
            dataTable.Columns.Add("Prenom", typeof (string));
            dataTable.Columns.Add("Profession", typeof (string));
            dataTable.Columns.Add("Mobile", typeof (string));
            dataTable.Columns.Add("Fix", typeof (string));
            dataTable.Columns.Add("Fax", typeof (string));
            dataTable.Columns.Add("Email", typeof (string));
            dataTable.Columns.Add("Type", typeof (string));
            dataTable.Columns.Add("Date", typeof (DateTime));
            dataTable.Columns.Add("Adresse", typeof (string));
            try
            {
                var gmdb = new GcdbEntities();
                var query = from t in gmdb.clients
                    select t;

                if (!query.Any()) return dataTable;
                foreach (var entity in query)
                {
                    var row = dataTable.NewRow();
                    row["ID"] = entity.ClientID;
                    row["RS"] = entity.ClientRS;
                    row["RC"] = entity.ClientRC;
                    row["NF"] = entity.ClientNIF;
                    row["NIS"] = entity.ClientNIS;
                    row["AI"] = entity.ClientAI;
                    row["Nom"] = entity.ClientFirstName;
                    row["Prenom"] = entity.ClientLastName;
                    if (entity.ClientProfessionID != null)
                        row["Profession"] = GetProfessionName((int) entity.ClientProfessionID + 1);
                    else row["Profession"] = "";
                    row["Mobile"] = entity.ClientMobile;
                    row["Fix"] = entity.ClientFix;
                    row["Fax"] = entity.ClientFax;
                    row["Email"] = entity.ClientEmail;
                    if (entity.ClientTypeID != null)
                        row["Type"] = GetTypeName((int) entity.ClientTypeID + 1);
                    else row["Type"] = "";
                    if (entity.ClientInsertionDate.HasValue)
                        row["Date"] = entity.ClientInsertionDate;
                    else row["Date"] = DateTime.Now.Date;
                    row["Adresse"] = entity.ClientAddress;
                    dataTable.Rows.Add(row);
                }
                return dataTable;
            }
            catch (Exception e)
            {
                return dataTable;
            }
        }

        private string GetTypeName(int typeID)
        {
            try
            {
                var gp = new GcdbEntities();
                var query = from t in gp.clientprovidertypes
                    where t.TypeID == typeID
                    select t.TypeName;
                return query.Any() ? query.First() : "";
            }
            catch (Exception e)
            {
                return "";
            }
        }

        private string GetProfessionName(int professionID)
        {
            try
            {
                var gp = new GcdbEntities();
                var query = from t in gp.professions
                    where t.ProfessionID == professionID
                    select t.professionName;
                return query.Any() ? query.First() : "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public client GetClientByID(int clientID)
        {
            var gp = new GcdbEntities();
            var query = from t in gp.clients
                where t.ClientID == clientID
                select t;
            return query.Any() ? query.First() : null;
        }

        public string DeleteClient(int clientID)
        {
            var gp = new GcdbEntities();
            var query = from t in gp.clients
                where t.ClientID == clientID
                select t;
            if (!query.Any()) return "Erreur";
            gp.clients.Remove(query.First());
            gp.SaveChanges();
            return "Supprimé avec succés";
        }

        public IEnumerable<string> GetClientList(string clientType)
        {
            var gp = new GcdbEntities();
            var list = new List<string>();
            try
            {
                switch (clientType)
                {
                    case "Externe":
                    {
                        var query = from t in gp.clients
                            select t;
                        if (!query.Any()) return list;

                        foreach (var variable in query)
                        {
                            string id = variable.ClientID.ToString();
                            string clientRs = variable.ClientRS;
                            list.Add(id + "-" + " " + clientRs);
                        }
                        return list;
                    }
                
                }
                return null;
            }
            catch (Exception e)
            {
                return list;
            }
        }

        public object GetClientDataTableAndOthers()
        {
            try
            {
                var getss = new List<object>
                {
                    GetClientDataTable(),
                    GetProfessionList(),
                };
                return getss;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private List<string> GetProfessionList()
        {
            try
            {
                var gp = new GcdbEntities();
                var query = from t in gp.professions
                    select t.professionName;
                return query.Any() ? query.ToList() : null;
            }
            catch (Exception e)
            {
                return new List<string>();
            }
        }

        public string GetClientName(int? clientID)
        {
            try
            {
                var gp = new GcdbEntities();
                var query = from t in gp.clients
                    where t.ClientID == clientID
                    select t;
                if (!query.Any()) return "";
                string name = query.First().ClientID.ToString(CultureInfo.InvariantCulture) + "-" + " " +
                              query.First().ClientRS;
                return name;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public static string GetClientFullName(int? clientID)
        {
            try
            {
                var gp = new GcdbEntities();
                var query = from t in gp.clients
                    where t.ClientID == clientID
                    select t;
                if (!query.Any()) return null;
                string id = query.First().ClientID.ToString(CultureInfo.InvariantCulture);
                string clientRs = query.First().ClientRS;
                string result = id + "-" + " " + clientRs;
                return result;
            }
            catch (Exception e)
            {
                return "";
            }
        }
    }
}
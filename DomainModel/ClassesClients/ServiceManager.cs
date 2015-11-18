using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.ClassesClients
{
   public class ServiceManager
    {
    
   

       public static List<service> GetAllServices()
       {
           try
           {
               var gpdb = new GcdbEntities();
               var mds = from m in gpdb.services
                         orderby m.ServiceName descending
                         select m;
             return mds.Any() ? mds.ToList() : new List<service>();
           }
           catch (Exception)
           {
               return new List<service>();
           }
       
       }

       public bool IsServiceModelNameExist(string modelName)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var query = from m in gpdb.servicemodels
                           select m.ServiceModelName;
               List<string> courrierNamesList = query.ToList();
               return courrierNamesList.Contains(modelName);
           }
           catch (Exception)
           {
               return false;
           }
        
       }

       public static int GetServiceID(string serviceName, string serviceFamille, int servicePrice)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var query = from m in gpdb.services
                           where m.ServiceFamilly == serviceFamille && m.ServiceName == serviceName && m.ServicePrice == servicePrice
                           select m;
               if (query.Any()) return query.First().ServiceID;
               return -1;
           }
           catch (Exception)
           {
               return -1;
           }
          
       }

       public static void AttachServiceToServiceModel(string modelName, int serviceID, string serviceName, string serviceFamille, 
           int servicePrice, int serviceQte, float serviceTva, string serviceDescription)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var serviceModel = new servicemodel
               {
                   ServiceModelName = modelName,
                   ServiceName = serviceName,
                   ServiceID = serviceID,
                   ServiceFamille = serviceFamille,
                   ServicePriceHT = servicePrice,
                   ServiceQte = serviceQte,
                   ServiceTVA = serviceTva,
                   ServiceDescription = serviceDescription
               };
               gpdb.servicemodels.Add(serviceModel);
               gpdb.SaveChanges();
           }
           catch (Exception)
           {

           }

       }

       public static DataTable GetAllServiceModels(DataTable dataTable)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var query = from m in gpdb.servicemodels
                   select m.ServiceModelName;
               if (!query.Any()) return dataTable;
               query = query.Distinct();

               foreach (string modelName in query)
               {
                   var row = dataTable.NewRow();
                   row[0] = modelName;
                   dataTable.Rows.Add(row);
               }
               return dataTable;
           }
           catch (Exception)
           {
               return dataTable;
           }

       }

       public static DataTable GetservicesOfModel(string modelName, DataTable selectedServices)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var query = from m in gpdb.servicemodels
                           where m.ServiceModelName == modelName
                           select m;
            
               var cpt = 0;
               if (!query.Any()) return selectedServices;
               foreach (var entry in query)
               {
                   var row = selectedServices.NewRow();

                   row["Ordre"] = ++cpt;
                   row["Name"] = entry.ServiceName;
                   row["Famille"] = entry.ServiceFamille;
                   row["Price HT"] = entry.ServicePriceHT;
                   row["Qte"] = entry.ServiceQte;
                   row["TVA"] = entry.ServiceTVA;
                   row["Total TTC"] = (entry.ServiceTVA + 1) * entry.ServicePriceHT * entry.ServiceQte;
                   row["Description"] = entry.ServiceDescription;

                   selectedServices.Rows.Add(row);
               }

               return selectedServices;
           }
           catch (Exception )
           {
               return selectedServices;
           }

       }

       public static string RemoveServiceModel(string modelName)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var query = from m in gpdb.servicemodels
                           where m.ServiceModelName == modelName
                           select m;
               if (!query.Any()) return " Model n'existe pas";
               foreach (var modelname in query)
               {
                   gpdb.servicemodels.Remove(modelname);
               }

               gpdb.SaveChanges();
               return "modèle supprimé avec succès";
           }
           catch (Exception)
           {
               return " Model n'existe pas";
           }
       }

       public static int CreateFactureService(DateTime serviceFacturedate, string serviceFactureType, string serviceModePayment,
           string serviceEtatPayement, float serviceTvaValue, string serviceClientFullName, string serviceCommandeNumber,
           DateTime serviceCommandeDate)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var newServicefacture = new servicefacture
               {
                   ServiceFactureDate = serviceFacturedate,
                   ServiceFactureType = serviceFactureType,
                   ServiceModePayment = serviceModePayment,
                   ServiceEtatPayement = serviceEtatPayement,
                   ServiceTvaValue = serviceTvaValue,
                   ServiceClientFullName = serviceClientFullName,
                   ServiceCommandeNumber = serviceCommandeNumber,
                   ServiceCommandeDate = serviceCommandeDate
               };

               gpdb.servicefactures.Add(newServicefacture);
               gpdb.SaveChanges();
               return newServicefacture.ServiceFactureID;
           }
           catch (Exception)
           {
               return -1;
           }
        
       }

       public static void AttachServiceToServiceFacture(DateTime serviceDate, int serviceFactureID, int serviceID, 
           string serviceName, string serviceFamille, int servicePrice, int serviceQte, float serviceTva, string serviceDescription)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var newServicefacturestore = new servicefacturestore
               {
                   ServiceDate = serviceDate,
                   ServiceFactureID = serviceFactureID,
                   ServiceID = serviceID,
                   ServiceName = serviceName,
                   ServiceFamille = serviceFamille,
                   ServicePriceHT = servicePrice,
                   ServiceQte = serviceQte,
                   ServiceTva = serviceTva,
                   ServiceDescription = serviceDescription
               };
               gpdb.servicefacturestores.Add(newServicefacturestore);
               gpdb.SaveChanges();

           }
           catch (Exception)
           {
               
           }  
          
       }

       public static DataTable GetAllServiceFactures(DataTable dataTable)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var query = from m in gpdb.servicefactures
                           select m;
               if (!query.Any()) return dataTable;

              
               int counter = 1;
               foreach (var entity in query) 
               {
                   var row = dataTable.NewRow();
                   row["Ordre"] = counter++;
                   row["Client"] = entity.ServiceClientFullName;
                   row["Date"] = entity.ServiceFactureDate;
                   row["Type"] = entity.ServiceFactureType;
                   row["N°"] = entity.ServiceFactureID;

                   dataTable.Rows.Add(row);
               }
               return dataTable;
           }
           catch (Exception)
           {
               return dataTable;
           }
       }

       public DataTable GetServiceFactureStore(DataTable selectedServices, int serviceFactureID)
       {
           try
           {


               var gpdb = new GcdbEntities();
               var query = from m in gpdb.servicefacturestores
                   where m.ServiceFactureID == serviceFactureID
                   select m;
               if (!query.Any()) return selectedServices;
               int counter = 1;

               foreach (var entity in query)
               {
                   var row = selectedServices.NewRow();
                   row["Ordre"] = counter++;
                   row["Name"] = entity.ServiceName;
                   row["Famille"] = entity.ServiceFamille;
                   row["Price HT"] = entity.ServicePriceHT;
                   row["Qte"] = entity.ServiceQte;
                   row["TVA"] = entity.ServiceTva;
                   row["Total TTC"] = entity.ServiceQte*entity.ServicePriceHT*(entity.ServiceTva + 1);
                   row["Description"] = entity.ServiceDescription;
                   selectedServices.Rows.Add(row);
               }
               return selectedServices;
           }
           catch (Exception)
           {
               return selectedServices;
           }
       }

       private client GetClient(string clientFullName)
       {
           
           string[] clientSplit = clientFullName.Split('-');
           int clientID = Convert.ToInt32(clientSplit[0]);
           var clientManager = new ClientManager();
         return clientManager.GetClientByID(clientID);
       }

       public servicefacture GetServiceFacture(int serviceFactureID)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var query = from m in gpdb.servicefactures
                           where m.ServiceFactureID == serviceFactureID
                           select m;
               return query.Any() ? query.First() : null;
           }
           catch (Exception)
           {
               return null;
               
           }
         
       }

       public void CreateService(string serviceName, string serviceFamille, int servicePriceHTtxt)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var newService = new service
               {
                   ServiceName = serviceName,
                   ServiceFamilly = serviceFamille,
                   ServicePrice = servicePriceHTtxt
               };
               gpdb.services.Add(newService);
               gpdb.SaveChanges();
           }
           catch (Exception)
           {
               
           }
          

       }

       public static void UpdateService(int serviceID, string serviceName, string serviceFamille, decimal servicePriceHT)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var query = from t in gpdb.services
                           where t.ServiceID == serviceID
                           select t;
               if (!query.Any()) return;
               int price = ConvertToInteger(servicePriceHT);
               query.First().ServiceName = serviceName;
               query.First().ServiceFamilly = serviceFamille;
               query.First().ServicePrice = price;
               gpdb.SaveChanges();
           }
           catch (Exception)
           {
               
           }
         
       }
     
       private static int ConvertToInteger(decimal valueDecimal)
       {
           try
           {
               return Convert.ToInt32(valueDecimal);

           }
           catch (Exception)
           {
               return 0;

           }

       }

       public static void DeleteService(int serviceID)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var query = from t in gpdb.services
                           where t.ServiceID == serviceID
                           select t;
               if (!query.Any()) return;
               gpdb.services.Remove(query.First());
               gpdb.SaveChanges();
           }
           catch (Exception)
           {
               
           }
         

       }

       public service GetServiceByID(int serviceID)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var query = from t in gpdb.services
                           where t.ServiceID == serviceID
                           select t;
               return !query.Any() ? null : query.First();
           }
           catch (Exception)
           {
               return null;
           }

          
       }

       public static int UpdateFactureService(int serviceFactureID, DateTime serviceFacturedate,
           string serviceFactureType, string serviceModePayment, string serviceEtatPayement, 
           float serviceTvaValue, string serviceClientFullName, string serviceCommandeNumber, 
           DateTime serviceCommandeDate)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var query = from t in gpdb.servicefactures
                           where t.ServiceFactureID == serviceFactureID
                           select t;
               if (!query.Any()) return -1;
               query.First().ServiceFactureDate = serviceFacturedate;
               query.First().ServiceFactureType = serviceFactureType;
               query.First().ServiceModePayment = serviceModePayment;
               query.First().ServiceEtatPayement = serviceEtatPayement;
               query.First().ServiceTvaValue = serviceTvaValue;
               query.First().ServiceClientFullName = serviceClientFullName;
               query.First().ServiceCommandeNumber = serviceCommandeNumber;
               query.First().ServiceCommandeDate = serviceCommandeDate;
               gpdb.SaveChanges();
               return serviceFactureID;
           }
           catch (Exception)
           {
               return serviceFactureID;
           }
         
       }

       public  void AttachServiceToServiceFactureStore(DateTime serviceDate, int theserviceFactureID,
           int serviceID, string serviceName, string serviceFamille, int servicePrice, int serviceQte,
           float serviceTva, string serviceDescription)
       {
           try
           {
               var gpdb = new GcdbEntities();

               var newServicefacturestore = new servicefacturestore
               {
                   ServiceDate = serviceDate,
                   ServiceFactureID = theserviceFactureID,
                   ServiceID = serviceID,
                   ServiceName = serviceName,
                   ServiceFamille = serviceFamille,
                   ServicePriceHT = servicePrice,
                   ServiceQte = serviceQte,
                   ServiceTva = serviceTva,
                   ServiceDescription = serviceDescription
               };
               gpdb.servicefacturestores.Add(newServicefacturestore);
               gpdb.SaveChanges();
           }
           catch (Exception)
           {
               
           }
          
           
       }

       public void UpdateServiceFactureStore(DateTime serviceDate, int theserviceFactureID,
           int serviceID, string serviceName, string serviceFamille, int servicePrice, int serviceQte,
           float serviceTva, string serviceDescription)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var query = from t in gpdb.servicefacturestores
                           where t.ServiceFactureID == theserviceFactureID && t.ServiceID == serviceID
                           select t;
               if (!query.Any()) return;
               query.First().ServiceDate = serviceDate;
               query.First().ServiceName = serviceName;
               query.First().ServiceFamille = serviceFamille;
               query.First().ServicePriceHT = servicePrice;
               query.First().ServiceQte = serviceQte;
               query.First().ServiceTva = serviceTva;
               query.First().ServiceDescription = serviceDescription;
               gpdb.SaveChanges();
           }
           catch (Exception)
           {
               
           }
          
       }
       public static bool IsServiceExiste(int theserviceFactureID, int serviceID)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var query = from t in gpdb.servicefacturestores
                           where t.ServiceFactureID == theserviceFactureID && t.ServiceID == serviceID
                           select t;
               return query.Any();
           }
           catch (Exception)
           {
               return false;
           }
           
       }

       public static string RemoveServiceStore(int serviceFactureID, int serviceID)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var query = from t in gpdb.servicefacturestores
                           where t.ServiceFactureID == serviceFactureID && t.ServiceID == serviceID
                           select t;
               if (!query.Any()) return "service n'existe pas!";
               gpdb.servicefacturestores.Remove(query.First());
               gpdb.SaveChanges();
               return "supprimé avec succés";
           }
           catch (Exception)
           {
               return "erreur";
           }
          
       }

       public static int GetLastServiceFactureIDPlusOne()
       {
           try
           {
               var gpdb = new GcdbEntities();
               var query = from t in gpdb.servicefactures
                           select t;
               if (!query.Any()) return 1;
               List<servicefacture> getList = query.ToList();
               servicefacture getServicefacture = getList.LastOrDefault();
               if (getServicefacture == null) return 1;
               int x = getServicefacture.ServiceFactureID;
               return x + 1;
           }
           catch (Exception)
           {
               return 1;
           }

        
       }

       public static servicemodel GetServiceModel(string modelName)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var query = from m in gpdb.servicemodels
                           where m.ServiceModelName == modelName
                           select m;
               return query.Any() ? query.First() : null;
           }
           catch (Exception)
           {
               return null;
           }
          
       }

       public static DataTable GetAllVenteFactures(DataTable dataTable)
       {

           try
           {
               var gpdb = new GcdbEntities();
               var query = from m in gpdb.ventefactures
                           select m;
               if (!query.Any()) return dataTable;


               int counter = 1;
               foreach (var entity in query)
               {
                   var row = dataTable.NewRow();
                   row["Ordre"] = counter++;
                   row["Client"] = entity.ServiceClientFullName;
                   row["Date"] = entity.VenteFactureDate;
                   row["Type"] = entity.ServiceFactureType;
                   row["N°"] = entity.VenteFactureID;

                   dataTable.Rows.Add(row);
               }
               return dataTable;
           }
           catch (Exception)
           {
               return dataTable;
           }
       }

       public static DataTable GetAllVenteModels(DataTable dataTable)
       {
           try
           {
               var gpdb = new GcdbEntities();
               var query = from m in gpdb.ventemodels
                           select m.VenteModelName;
               if (!query.Any()) return dataTable;
               query = query.Distinct();

               foreach (string modelName in query)
               {
                   var row = dataTable.NewRow();
                   row[0] = modelName;
                   dataTable.Rows.Add(row);
               }
               return dataTable;
           }
           catch (Exception)
           {
               return dataTable;
           }

       }
    }
}

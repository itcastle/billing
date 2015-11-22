using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.ClassesClients
{
   public class StockManager
    {
       public decimal GetStockQte(int getProductId)
       {
           
           GcdbEntities gestionDb=new GcdbEntities();
           var query = from t in gestionDb.StockStores
                       where t.ProductID == getProductId
               select t;
           if (!query.Any()) return 0;
           decimal qte = 0;
           foreach (var entity in query )
           {
               if (entity.UnitsOnOrder == null) continue;
               decimal unitsnorder = (decimal) entity.UnitsOnOrder;
               qte += unitsnorder;
           }
           return qte;
       }

       public string AddNewProductToStock(Product product, float prixAchat, int qteAchter, float prixVenteGros, float prixVenteDetail, float prixVenteComptoire, float totalPriceHt, string stockageId, string productSnumber, string productState, string stockObs, DateTime insertionDate, string refrenceNum, DateTime starTime, DateTime endTime)
       {

           try
           {
               var gestionDb = new GcdbEntities();

               var newsStockStore = new StockStore
               {
                   ProductID = product.ProductID,
                   PurchasePrice = prixAchat,
                   UnitsOnOrder = qteAchter,
                   VentePriceGros = prixVenteGros,
                   VentePriceDetail = prixVenteDetail,
                   VentePriceComptoire = prixVenteComptoire,
                   TotalPriceAchat = totalPriceHt,
                   Status = 0,
                   TvaValue = 0,
                   Discount = 0,
                   ProductState = productState,
                   Serialnumber = productSnumber,
                   Observation = stockObs,
                   InsertionDate = insertionDate,
                   RefrenceNum = refrenceNum,
                   StockageID = stockageId,
                   DateOfEndPremption = endTime,
                   DateOfStartPremption = starTime
               };

               gestionDb.StockStores.Add(newsStockStore);
               gestionDb.SaveChanges();
               return "Ajouté avec succés";
               
           }
           catch (Exception)
           {
               return "Erreur";
           }
       }

       public DataTable GetStockDataTable(DataTable dataTable)
       {
           try
           {
               var gestionDb = new GcdbEntities();
               var query = from t in gestionDb.StockStores
                           select t;


               if (!query.Any()) return dataTable;
               int counter = 1;
               foreach (var entity in query)
               {
                   var row = dataTable.NewRow();
                   row["Ordre"] = counter++;
                   row["N° Stock"] = entity.StockStoreID;
                   row["Catégorie"] = entity.Product.SubCategory.Category.CategoryName;
                   row["Sous Catégorie"] = entity.Product.SubCategory.SubCategoryName;
                   row["Produit"] = entity.Product.ProductName;
                   row["Référence"] = entity.RefrenceNum;
                   row["Designation"] = entity.Product.Designation;
                   row["Unité"] = entity.Product.MeasureUnit;
                   row["Qte Min"] = entity.Product.ProductMinQte;
                   row["Qte Max"] = entity.Product.ProductMaxQte;
                   row["Qte en stock"] = entity.UnitsOnOrder;
                   row["Prix d'achat"] = entity.VentePriceGros;
                   row["Prix de vente"] = entity.VentePriceGros;
                   row["Prix Total"] = entity.TotalPriceAchat;
                   row["Etat"] = entity.ProductState;
                   row["N° serie"] = entity.Serialnumber;
                   row["OBS"] = entity.Observation;
                   row["Date Mise à jour"] = entity.InsertionDate;
                   row["Stockage ID"] = entity.StockageID;
                   dataTable.Rows.Add(row);
               }
               return dataTable;
           }
           catch (Exception exe)
           {
               MessageBox.Show(exe.ToString());
               return dataTable;
           }
       }


       public static StockStore GetStockStoreById(int stockStoreId)
       {
           try
           {
               var gestionDb = new GcdbEntities();
               var query = from t in gestionDb.StockStores
                   where t.StockStoreID == stockStoreId
                   select t;
               return query.Any() ? query.First() : null;
           }
           catch (Exception)
           {
               return null;
           }

       }

       public string UpdateStockStore(StockStore getStockStore, float prixAchat, int qteAchter,
           float prixVenteGros, float prixVenteDetail, float prixVenteComptoire, float totalPriceHt, string stockageId,
           string productSnumber, string productState, string stockObs, DateTime insertionDate, string refrenceNum)
       {
           try
           {
               var gestionDb = new GcdbEntities();
               var query = from t in gestionDb.StockStores
                   where t.StockStoreID == getStockStore.StockStoreID
                   select t;
               if (!query.Any()) return "Erreur";

               query.First().ProductID = getStockStore.ProductID;
               query.First().PurchasePrice = prixAchat;
               query.First().UnitsOnOrder = qteAchter;
               query.First().VentePriceGros = prixVenteGros;
               query.First().VentePriceDetail = prixVenteDetail;
               query.First().VentePriceComptoire = prixVenteComptoire;
               query.First().TotalPriceAchat = totalPriceHt;
              
               query.First().ProductState = productState;
               query.First().Serialnumber = productSnumber;
               query.First().Observation = stockObs;
               query.First().InsertionDate = insertionDate;
               query.First().RefrenceNum = refrenceNum;
               query.First().StockageID = stockageId;
               gestionDb.SaveChanges();
               return "Mise à jour avec succés";
           }
           catch (Exception)
           {
               return "Erreur";
           }

       }

       public string DeleteStockStore(StockStore getStockStore)
       {
           try
           {
               var gestionDb = new GcdbEntities();
               var query = from t in gestionDb.StockStores
                           where t.StockStoreID == getStockStore.StockStoreID
                           select t;
               if (!query.Any()) return "Erreur";
               gestionDb.StockStores.Remove(query.First());
               gestionDb.SaveChanges();
               return "Supprimé avec succés";
           }
           catch (Exception)
           {
               return "Erreur";
           }   

       }

       public Product GetProduct(StockStore getStockStore)
       {
           try
           {
               var gestionDb = new GcdbEntities();
               var query = from t in gestionDb.Products
                           where t.ProductID == getStockStore.ProductID
                           select t;
               if (!query.Any()) return null;
               return query.First();
           }
           catch (Exception)
           {
           return null;    
           }

       }
    }
}

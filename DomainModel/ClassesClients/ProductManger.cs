using System;
using System.Collections.Generic;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.ClassesClients
{
    public class ProductManger
    {
        private GcdbEntities _gestionDb;
        public List<Product> GetProducts()
        {
            try
            {
                 _gestionDb = new GcdbEntities();

                return _gestionDb.Products.Where(c => c.Status == 0).ToList();
            }
            catch (Exception)
            {
                return null;
            }

        }
        public List<StockStore> GetProductsInStock()
        {
            try
            {
                _gestionDb = new GcdbEntities();

                return _gestionDb.StockStores.Where(c => c.Product.Status == 0).ToList();
            }
            catch (Exception)
            {
                return null;
            }

        }
        public Double GetTotalProducts(Product produit)
        {
            try
            {
                _gestionDb = new GcdbEntities();

                var list = from od in _gestionDb.OrderDetails
                           where (produit.ProductID == od.ProductID)
                           select od;
                Double somme = 0;

                foreach (OrderDetail od in list) somme += (Double)od.UnitPrice * od.Quantity;


                return somme;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public String DesactivateProduct(Product produit)
        {

            _gestionDb = new GcdbEntities();

            produit.Status = 1;
            MajProduct(produit);

            return "Produit désactiver avec succes";
        }
        public Product GetProductByName(String name)
        {
            try
            {
                _gestionDb = new GcdbEntities();
               
                return _gestionDb.Products.Single(p => p.ProductName.Equals(name));
            }
            catch (Exception)
            {
                return null;
            }

        }
        public void ResetUnitsOnOrder()
        {
            try
            {
                _gestionDb = new GcdbEntities();
                var listProduit = _gestionDb.Products;
                if (listProduit.Any())
                {
                    foreach (Product produit in listProduit)
                    {
                        produit.UnitsOnOrder = 0;
                    }
                }
                _gestionDb.SaveChanges();
            }
            catch (Exception)
            {
               // 
            }

        }
       public String AddProduct(Product produit)
        {

            _gestionDb = new GcdbEntities();

            if (IsProductExist(produit))
            {
                return "Produit existe déjà ...";
            }

           _gestionDb.Products.Add(produit);
           _gestionDb.SaveChanges();

           return "Produit ajouter avec succes";
        }
        public String DelProduct(Product p) 
        {

            _gestionDb = new GcdbEntities();
            if (!IsProductExist(p)) return "Product n'existante pas";
            var objRd = _gestionDb.Products.Where(prod => prod.ProductName.Equals(p.ProductName));
            if (objRd.Any())
            {
                foreach (Product objR in objRd)
                {
                    _gestionDb.Products.Add(objR);
                }
            }
            _gestionDb.SaveChanges();


            return "Produit supprimer avec succes";
        }

        public String MajProduct(Product p)
        {

            _gestionDb = new GcdbEntities();
            
            if (!IsProductExist(p)) return "Produit n'existante pas";

            Product prod = _gestionDb.Products.Where(produit => produit.ProductName.Equals(p.ProductName)).ToList().First();
           // prod = p;
              // to updated       
            _gestionDb.SaveChanges();

            return "Produit modifier avec succes";
        }

        private bool IsProductExist(Product prod)
        {

            _gestionDb = new GcdbEntities();

        
            try
            {
                var query = _gestionDb.Products.Where(p => p.ProductName.ToLower().Equals(prod.ProductName.ToLower())).ToList();
                return query.Any();

            }
            catch (Exception )
            {
                return false;
            }
            
        
         }

        public bool IsProductExist(String prod)
        {

            _gestionDb = new GcdbEntities();

     
            try
            {
                var query = _gestionDb.Products.Where(p => p.ProductName.ToLower().Equals(prod.ToLower())).ToList();
                return query.Any();
            }
            catch (Exception )
            {
                return false;
            }

      
        }

        
        public  bool IsQuantityPermite(Product prod)
        {

            _gestionDb = new GcdbEntities();

            var query = from product in _gestionDb.Products
                        where product.ProductID.Equals(prod.ProductID)
                        select new
                        {
                            ProductQt = product.UnitsInStock
                        };


            
            foreach (var productInfo in query)
            {
                if (productInfo.ProductQt >= prod.UnitsOnOrder) return true;
            }


            return false;
        }
     


        public static void SaySomthing(string text)
        {
            Console.WriteLine(text);
        }

        public string AddNewProduct(string categoryName, string subCategoryName, string productName,
            string productMeasure, string productType, string productReferenceIntenrne, int qteMin, int qteMax,
            string productDesignation, string productRemarks, int state)
        {
            try
            {
                _gestionDb = new GcdbEntities();
                
                int getSubCategoryId = GetSubCategoryId(categoryName, subCategoryName);
                var newProduct = new Product
                { 
                    SubCategoryID = getSubCategoryId,
                  
                    ProductName = productName,
                    MeasureUnit = productMeasure,
                    ProductMaxQte = qteMax,
                    ProductMinQte = qteMin,
                    ReferenceInterne = productReferenceIntenrne,
                    Designation = productDesignation,
                    Remarks = productRemarks,
                    productType = productType,
                    Status = state
                };
                _gestionDb.Products.Add(newProduct);
                _gestionDb.SaveChanges();
                return "Ajouté avec succés";
            }
            catch (Exception)
            {
                return "Erreur";
            }
        }
         
        private int GetSubCategoryId(string categoryName, string subCategoryName)
        {

            try
            {
                _gestionDb = new GcdbEntities();
                var query = from t in _gestionDb.SubCategories
                            where t.SubCategoryName.Equals(subCategoryName) && t.Category.CategoryName.Equals(categoryName)
                    select t;
                if (query.Any())
                {
                    return query.First().SubCategoryID;

                }
                return -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public Product GetProductById(int productId)
        {

            try
            {
                _gestionDb = new GcdbEntities();
                var query = from t in _gestionDb.Products
                    where t.ProductID == productId
                    select t;
                return !query.Any() ? null : query.First();

            }
            catch (Exception)
            {
                return null;
            }
        }

        public string UpdateProduct(Product produit, string categoryName, string subCategoryName,
            string productName, string productMeasure, string productType, 
            string productReferenceIntenrne, int qteMin, int qteMax, string productDesignation, 
            string productRemarks)
        {
            try
            {
                int getCategoryId = GetSubCategoryId(categoryName, subCategoryName);
                _gestionDb = new GcdbEntities();
                var query = from t in _gestionDb.Products
                            where t.ProductID == produit.ProductID
                            select t;
                if (!query.Any()) return "Produit n'existe pas!";
           
                query.First().SubCategoryID = getCategoryId;
            
                query.First().ProductName = productName;
                query.First().MeasureUnit = productMeasure;
                query.First().ProductMaxQte = qteMax;
                query.First().ProductMinQte = qteMin;
                query.First().ReferenceInterne = productReferenceIntenrne;
                query.First().Designation = productDesignation;
                query.First().Remarks = productRemarks;
                query.First().productType = productType;
                _gestionDb.SaveChanges();
                return "Mise à jour avec succés";
            }
            catch (Exception)
            {
                return "Errur";
            }
        }

        public object GetCategoryByName(string catName)
        {
            _gestionDb = new GcdbEntities();
            var query2 = from t in _gestionDb.Categories
                         from g in _gestionDb.StockStores
                         where g.Product.SubCategory.Category.CategoryID==t.CategoryID
                         where t.CategoryName.Equals(catName)
                         select new
                         {
                             ProductName = g.Product.ProductName + " " + g.RefrenceNum,
                             g.UnitsOnOrder
                         };

            //var query = from c in _gestionDb.StockStores
            //            where c.Product.SubCategory.Category.CategoryName.Equals(catName)
                      
            //    select new
            //    {
            //       ProductName= c.Product.ProductName+" " +c.RefrenceNum,
            //        c.UnitsOnOrder
            //    };
            if (query2.Any())
            {
                object getquery = query2.ToList();
                return getquery;
            }
            return null;
        }

        public StockStore GetStockStore(int productId)
        {

            try
            {
                _gestionDb = new GcdbEntities();
                var query = from t in _gestionDb.StockStores
                            where t.ProductID == productId
                            select t;
                return !query.Any() ? null : query.First();

            }
            catch (Exception)
            {
                return null;
            }
        }
    }

}



using System;
using System.Collections.Generic;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.ClassesClients
{
    public class CategorysClient
    {
        private GcdbEntities _gestionDb;
      

        public List<string> GetCategorysNames()
        {
            try
            {
                _gestionDb = new GcdbEntities();
                var query = from t in _gestionDb.Categories
                            orderby t.CategoryName ascending
                    select t.CategoryName;

                return query.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Category> GetCategorys()
        {
            try
            {
                _gestionDb = new GcdbEntities();
                var query = from t in _gestionDb.Categories
                            orderby t.CategoryName ascending
                            select t;
                return query.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public String AddCategory(Category cat)
        {
            _gestionDb = new GcdbEntities();
            if (IsCategoryExist(cat)) return "Category existe déjà";

            _gestionDb.Categories.Add(cat);
            _gestionDb.SaveChanges();

            return "Category ajouter avec succes";
        }

     
      
        private bool IsCategoryExist(Category cat)
        {
            try
            {

                _gestionDb = new GcdbEntities();

            var requete = from t in _gestionDb.Categories
                          where t.CategoryName.Equals(cat.CategoryName) 
                          select t;

            if (requete.ToList().Count != 0) return true;
            }
            catch (Exception)
            {

                return false;
            }

            return false;
        }
      
      
        public bool IsSubCategoryExist(string subCategoryname, string Categoryname)
        {
            try
            {

                _gestionDb = new GcdbEntities();

                var query = from t in _gestionDb.SubCategories
                              where t.SubCategoryName.Equals(subCategoryname)
                              where t.Category.CategoryName.Equals(Categoryname)
                              select t;

                return (query.Any());
               
            }
            catch (Exception)
            {

                return false;
            }

       

        }

        public string AddSubCategory(string Categoryname, string subCategoryname, string description, byte[] photo)
        {

            try
            {
                _gestionDb = new GcdbEntities();
                SubCategory newSubCategory=new SubCategory
                {
                    SubCategoryName = subCategoryname
                };
               
                var query = from t in _gestionDb.Categories
                            where t.CategoryName == Categoryname
                    select t;
                if (query.Any())
                {
                    query.First().SubCategories.Add(newSubCategory);
                    _gestionDb.SaveChanges();
                    return "Sous catégorie ajouté avec succés";
                }
                else
                {
                    Category newCategory = new Category
                    {
                        CategoryName = Categoryname,
                        Photo = photo,
                        Description = description
                    };
                    SubCategory subCategory = new SubCategory
                    {
                        SubCategoryName = subCategoryname
                    };
                    newCategory.SubCategories.Add(subCategory);
                    _gestionDb.Categories.Add(newCategory);
                    _gestionDb.SaveChanges();
                    return "Ajouté avec succés";
                }
            }
            catch (Exception exe)
            {
                return "Erreur";
            }
        }

        public List<string> GetListCategoryName()
        {

            try
            {
                _gestionDb = new GcdbEntities();
                var query = from t in _gestionDb.Categories
                            orderby t.CategoryName ascending
                    select t.CategoryName;
                if(!query.Any())return new List<string>();
                return query.ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public List<string> GetSubCategorysNames(string CategoryName)
        {
            try
            {
                _gestionDb = new GcdbEntities();
                var query = from t in _gestionDb.SubCategories
                    where t.Category.CategoryName.Equals(CategoryName)
                    orderby t.SubCategoryName ascending 
                    select t.SubCategoryName;
                return !query.Any() ? new List<string>() : query.ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public List<Product> GetProducts(string CategoryName, string subCategoryName)
        {
            try
            {
                _gestionDb = new GcdbEntities();
                var query = from t in _gestionDb.Products
                    where t.SubCategory.SubCategoryName.Equals(subCategoryName)
                    where t.SubCategory.Category.CategoryName.Equals(CategoryName)
                    select t;
                return !query.Any() ? new List<Product>() : query.ToList();
            }
            catch (Exception)
            {
                return new List<Product>();
            }

        }
    }

}



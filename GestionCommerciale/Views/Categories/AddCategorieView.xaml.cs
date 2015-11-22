using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using GestionCommerciale.DomainModel.ClassesClients;
using GestionCommerciale.DomainModel.Validator;
using GestionCommerciale.Views.Products;
using Categorys = GestionCommerciale.DomainModel.Entities.Category;

namespace GestionCommerciale.Views.Categories
{
    /// <summary>
    /// Interaction logic for AddCategory.xaml
    /// </summary>
    public partial class AddCategoryView
    {
        readonly CategorysClient _categorysClient;
        private readonly AddProductView _addProductView;
        public AddCategoryView(AddProductView addProductView)
        {
            
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            _categorysClient = new CategorysClient();
            _addProductView = addProductView;

        }

        private void button1_OnClick(object sender, RoutedEventArgs e)
        {

            var cat = new Categorys();

            byte[] photo = null;

            var image = ImageEdit1.Source as BitmapImage;
            if (image != null)
            {
                var bi = image;
                var stream = bi.StreamSource as FileStream;

                if (stream != null)
                {
                    String photoPath = stream.Name;
                    photo = Validator.ConvertImageToByteArray(photoPath);
                }
            }

            var categoryname = CategoryNameCbx.Text ?? "";
            string subCategoryname = SubCategoryNameTxt.Text;
            if (String.IsNullOrEmpty(categoryname))
            {
                DXMessageBox.Show(this, "Erreur dans le Nom du catégorie (ne doit pas etre vide");
                return;
            }
            if (String.IsNullOrEmpty(subCategoryname.Trim()))
            {
                DXMessageBox.Show(this, "Erreur dans le Nom du sous catégorie (ne doit pas etre vide");
                return;
            }

            if (_categorysClient.IsSubCategoryExist(subCategoryname.Trim(), categoryname))
            {
                DXMessageBox.Show(this, "sous catégorie existe déjà");
                return;
            }

           

            String description = new TextRange(RichTextBox1.Document.ContentStart, RichTextBox1.Document.ContentEnd).Text;



            String result = _categorysClient.AddSubCategory(categoryname.Trim(), subCategoryname.Trim(), description.Trim(), photo);

            DXMessageBox.Show(this,result);
          
        }

        private void AddCategoryView_OnLoaded(object sender, RoutedEventArgs e)
        {

            try
            {
                List<string> listCategorys = _categorysClient.GetListCategoryName();
                CategoryNameCbx.ItemsSource = listCategorys;
            }
            catch (Exception)
            {
                //
            }
        }

        private void AddCategoryView_OnClosed(object sender, EventArgs e)
        {

            if (_addProductView != null)
            {
                _addProductView.LoadCategoryAndSubCategory();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using GestionCommerciale.DomainModel;
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
        readonly CategorysClient _CategorysClient;
        private AddProductView _addProductView;
        public AddCategoryView(AddProductView addProductView)
        {
            
            InitializeComponent();
            _CategorysClient = new CategorysClient();
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

            var Categoryname = CategoryNameCbx.Text ?? "";
            string subCategoryname = SubCategoryNameTxt.Text;
            if (String.IsNullOrEmpty(Categoryname))
            {
                DXMessageBox.Show(this, "Erreur dans le Nom du catégorie (ne doit pas etre vide");
                return;
            }
            if (String.IsNullOrEmpty(subCategoryname.Trim()))
            {
                DXMessageBox.Show(this, "Erreur dans le Nom du sous catégorie (ne doit pas etre vide");
                return;
            }

            if (_CategorysClient.IsSubCategoryExist(subCategoryname.Trim(), Categoryname))
            {
                DXMessageBox.Show(this, "sous catégorie existe déjà");
                return;
            }

           

            String description = new TextRange(RichTextBox1.Document.ContentStart, RichTextBox1.Document.ContentEnd).Text;



            String result = _CategorysClient.AddSubCategory(Categoryname.Trim(), subCategoryname.Trim(), description.Trim(), photo);

            DXMessageBox.Show(this,result);
          
        }

        private void AddCategoryView_OnLoaded(object sender, RoutedEventArgs e)
        {

            try
            {
                List<string> listCategorys = _CategorysClient.GetListCategoryName();
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

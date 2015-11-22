using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.ClassesClients;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.DomainModel.Validator;
using GestionCommerciale.Helpers;

namespace GestionCommerciale.Views.Products
{
    /// <summary>
    /// Interaction logic for ListSuppliersView.xaml
    /// </summary>
    public partial class ListProductsView
    {
        Product _produit;
        readonly ProductManger _productClient;
        readonly CategorysClient _categorysClient;

    
        List<Product> _productsList;


        private readonly TabHelper _tabHlp;
        private int _productId;
        public ListProductsView(string animationName, TabHelper hlp)
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            _tabHlp = hlp;
            _produit = new Product();
            _productClient = new ProductManger();
            _categorysClient = new CategorysClient();
        
            if (string.IsNullOrEmpty(animationName)) return;
            Storyboard animation = (Storyboard) Application.Current.Resources[animationName];
            LayoutRoot.BeginStoryboard(animation);
        }

      
        private void NewProductBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = _tabHlp.AddNewTab(typeof(AddProductView), "Nouveau produit ", "FadeToLeftAnim");
        }

        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            RefreshBtn_OnClick(null,null);
        }

        private void LoadProductGridControl()
        {

            var getProducts = new BackgroundWorker();
            getProducts.DoWork += GetAllProductsOnDoWork;
            getProducts.RunWorkerCompleted += GetAllProductsOnRunWorkerCompleted;
            getProducts.RunWorkerAsync(); 
            // backgroundworker soon..

            _productsList = _productClient.GetProducts();
           
                  
        }

        private void GetAllProductsOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        { 
            var productsList = _productClient.GetProducts();
            doWorkEventArgs.Result = productsList;

        }

        private void GetAllProductsOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs workerCompleted)
        {
            _productsList = workerCompleted.Result as List<Product>;
    
            if (_productsList != null)

                ProductsDataGrid.ItemsSource = (from t in _productsList
                                                select new
                                                {
                                                    ProductName = t.ProductName,
                                                    CategoryName = t.SubCategory.Category.CategoryName,
                                                    SubCategoryName = t.SubCategory.SubCategoryName,
                                                    ProductMinQte = t.ProductMinQte,
                                                    ProductMaxQte = t.ProductMaxQte,

                                                    ReferenceInterne = t.ReferenceInterne,
                                                    MeasureUnit = t.MeasureUnit,
                                                    Designation = t.Designation,
                                                    Remarks = t.Remarks,
                                                    ProductID = t.ProductID
                                                }
                                                             ).ToList();

        }


        private void ProductTableView_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (ProductsDataGrid.VisibleRowCount == 0) return;
                int rowHandle = ProductsDataGrid.View.FocusedRowHandle;
                if (rowHandle < 0) return;
                ProductManger productManger = new ProductManger();
                _productId = (int) ProductsDataGrid.GetCellValue(rowHandle, "ProductID");
                Product produit = productManger.GetProductById(_productId);

                if (produit == null)
                {
                    _productId = -1;
                    return;
                }
                LoadProductFields(produit);
            }
            catch (Exception )
            {

                //
            }

        }
     

        private void LoadProductFields(Product produit)
        {
            _productId = produit.ProductID;
            CategorysCbx.Text = produit.SubCategory.Category.CategoryName;
            SubCategoryCbx.Text = produit.SubCategory.SubCategoryName;
            ProductNameTxtBox.Text = produit.ProductName;
            MeasureCbx.Text = produit.MeasureUnit;
            ProductTypeCbx.Text = produit.productType;
            ProductReferenceTxt.Text = produit.ReferenceInterne;
            if (produit.ProductMinQte != null) ProductQteMinSpin.Value = (int) produit.ProductMinQte;
            if (produit.ProductMaxQte != null) ProductQteMaxSpin.Value = (int)produit.ProductMaxQte;
            ProductDesignationTxt.Text = produit.Designation;
            ProductRemarksTxt.Text = produit.Remarks;
        
        }

        private void EditSupplierBtn_Click(object sender, RoutedEventArgs e)
        {

            if (_productId == -1) return;

            if (DXMessageBox.Show(this,"Êtes-vous sûr de vouloir modifier ce produit?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No) return;

         

            ProductManger productManger = new ProductManger();
         
            Product produit = productManger.GetProductById(_productId);

            if (produit == null) return;

            var image = ImageEdit1.Source as BitmapImage;
            if (image != null)
            {
                BitmapImage bi = image;
                FileStream stream = bi.StreamSource as FileStream;
                if (stream != null)
                {
                    var photoPath = stream.Name;
                    produit.Photo = Validator.ConvertImageToByteArray(photoPath);
                }
            }

            string categoryName = CategorysCbx.Text;
            string subCategoryName = SubCategoryCbx.Text;
            string productName = ProductNameTxtBox.Text;
            string productMeasure = MeasureCbx.Text;
            string productType = ProductTypeCbx.Text;
            string productReference = ProductReferenceTxt.Text;
            int qteMin = (int)ProductQteMinSpin.Value;
            int qteMax = (int)ProductQteMaxSpin.Value;
            string productDesignation = ProductDesignationTxt.Text;
            string productRemarks = ProductRemarksTxt.Text;
         

            String result = productManger.UpdateProduct(produit,categoryName, subCategoryName, productName, productMeasure, productType,
                productReference, qteMin, qteMax, productDesignation, productRemarks);
            DXMessageBox.Show(this, result);
            RefreshBtn_OnClick(null, null);
        }

        private void DeleteSupplierBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.VisibleRowCount == 0) return;

            if (MessageBox.Show("Êtes-vous sûr de vouloir supprimer  ce produit?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No) return;

            int rowHandle = ProductsDataGrid.View.FocusedRowHandle;

            ProductManger pc = new ProductManger();
            Product produit = pc.GetProductByName(ProductsDataGrid.GetCellValue(rowHandle, "ProductName").ToString());

            if (produit == null) return;
            pc.DesactivateProduct(produit);
            LoadProductGridControl();
        }

      


        private void RefreshBtn_OnClick(object sender, RoutedEventArgs e)
        {
            
            LoadProductGridControl();
            ClearProductFiealds();
        }

        private void ClearProductFiealds()
        {
            _productId = -1;
            CategorysCbx.Clear();
            SubCategoryCbx.Clear();
            ProductNameTxtBox.Clear();
            MeasureCbx.Clear();
            ProductTypeCbx.Clear();
            ProductReferenceTxt.Clear();
            ProductQteMinSpin.Clear();
            ProductQteMaxSpin.Clear();
            ProductDesignationTxt.Clear();
            ProductRemarksTxt.Clear();

        }

        private void cbCategorys_GotFocus(object sender, RoutedEventArgs e)
        {

            Dispatcher.BeginInvoke(new Action(() =>
            {
                CategorysCbx.ItemsSource = _categorysClient.GetCategorysNames();

            }));
        }

        private void CategorysCbx_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {

            string CategoryName = CategorysCbx.Text;
            if (string.IsNullOrEmpty(CategoryName) || string.IsNullOrWhiteSpace(CategoryName)) return;
            SubCategoryCbx.Clear();
            ProductNameTxtBox.Clear();
            MeasureCbx.Clear();
            ProductTypeCbx.Clear();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SubCategoryCbx.ItemsSource = _categorysClient.GetSubCategorysNames(CategoryName);

            }));
        }

        private void SubCategoryCbx_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {

            ProductNameTxtBox.Clear();
            MeasureCbx.Clear();
            ProductTypeCbx.Clear();
        }

        private void MeasureCbx_OnGotFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
            var measureManager = new MeasureManager();
            var listUnits = measureManager.GetMeasureList();
            MeasureCbx.ItemsSource = listUnits;
            }));
        }
    }
}

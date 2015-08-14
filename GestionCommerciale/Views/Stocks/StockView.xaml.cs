using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using GestionCommerciale.DomainModel.Entities;
using DevExpress.Data;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using GestionCommerciale.DomainModel;
using Application = System.Windows.Application;

namespace GestionCommerciale.Views.StockFolder
{
    /// <summary>
    /// Interaction logic for StockView.xaml
    /// </summary>
    public partial class StockView
    {
      
        private DataTable _dataTable;
        private readonly CategorysClient _CategorysClient;
        private StockStore _getStockStore;
        private Product _product;
        private readonly ProductManger _productManger;
        private readonly StockManager _stockManager;

        public StockView()
        {
            InitializeComponent();
    
           

        }

        private void LoadEmptyble()
        {

            _dataTable = new DataTable();

            _dataTable.Columns.Add("Ordre", typeof(int));
            _dataTable.Columns.Add("N° Stock", typeof(int));
            _dataTable.Columns.Add("Catégorie", typeof(string));
            _dataTable.Columns.Add("Sous Catégorie", typeof(string));
            _dataTable.Columns.Add("Produit", typeof(string));
            _dataTable.Columns.Add("Référence", typeof(string));
            _dataTable.Columns.Add("Designation", typeof(string));
            _dataTable.Columns.Add("Unité", typeof(string));

            _dataTable.Columns.Add("Qte Min", typeof(int));
            _dataTable.Columns.Add("Qte Max", typeof(int));

            _dataTable.Columns.Add("Prix d'achat", typeof(float));
            _dataTable.Columns.Add("Prix de vente", typeof(float));
            _dataTable.Columns.Add("Prix Total", typeof(float));

            _dataTable.Columns.Add("Etat", typeof(string));
            _dataTable.Columns.Add("N° serie", typeof(string));
            _dataTable.Columns.Add("OBS", typeof(string));

            _dataTable.Columns.Add("Qte en stock", typeof(int));
            _dataTable.Columns.Add("Date Mise à jour", typeof(DateTime));
            _dataTable.Columns.Add("Stockage ID", typeof(string));
            StockGridControl.ItemsSource = _dataTable.DefaultView;
            StockTableView.AutoWidth = false;


#pragma warning disable 618
            DXGridDataController.DisableThreadingProblemsDetection = true;
#pragma warning restore 618

#pragma warning disable 618
            CurrencyDataController.DisableThreadingProblemsDetection = true;
#pragma warning restore 618
        }

        public StockView(string animationName)
        {
            InitializeComponent();
        
            if (!string.IsNullOrEmpty(animationName))
            {
                Storyboard animation = (Storyboard)Application.Current.Resources[animationName];
                LayoutRoot.BeginStoryboard(animation);
            }
            LoadEmptyble();
            _CategorysClient=new CategorysClient();
            _productManger=new ProductManger();
            _stockManager=new StockManager();
        }
        public void Connect(int connectionId, object target)
        {
        }

        private void StockLoadCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                
                object datatable = e.Result;
                if (datatable == null) return;
                StockGridControl.ItemsSource = datatable;
                //****************************
                StockGridControl.Columns[0].ShowInColumnChooser = true;
                StockGridControl.Columns[0].Visible = false;
                //***************************
                StockTableView.ShowAutoFilterRow = true;
                StockTableView.BestFitColumns();
                RefreshFields.IsEnabled = true;
            }
            catch (Exception)
            {
                RefreshFields.IsEnabled = true;
            }
        }

        private void StockDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var stockManager = new StockManager();
                _dataTable.Rows.Clear();
                e.Result = stockManager.GetStockDataTable(_dataTable);
            }
            catch (Exception)
            {
                //
            }
        }

        private void UserControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadCategoryCbx();
            RefreshFields_OnClick(null, null);
        }

        private void LoadCategoryCbx()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CategoryCbx.ItemsSource = _CategorysClient.GetCategorysNames();

            }));

        }

        //*********************************************

        private void AddToStockBtn_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {

                if (_product==null|| _getStockStore!=null) return;
              
                    float prixAchat = ConverToFloat(PrixAchatTxt.Text);
                    int qteAchter = ConvertToInteger(QteAchterSpin.Value);
                    float prixVenteGros = ConverToFloat(PrixVenteGrosTxt.Text);
                    float prixVenteDetail = ConverToFloat(VentePriceUnitDetailTxt.Text);
                    float prixVenteComptoire = ConverToFloat(VentePriceUnitComptoireTxt.Text);
                    float totalPriceHt = ConverToFloat(TotalPrixTxt.Text);
                    string stockageId = StockageIDtxt.Text;
                    string productSnumber = ProductSNtxt.Text;
                    string productState = ProductStateCBX.Text;
                    string stockObs = StockObStxt.Text;
                    string refrenceNum = ReferenceTxt.Text;
                    DateTime insertionDate = ProductSaleDateDte.DateTime;

                    string result = _stockManager.AddNewProductToStock(_product, prixAchat, qteAchter,
                        prixVenteGros,prixVenteDetail,prixVenteComptoire, totalPriceHt, stockageId, productSnumber, productState, stockObs, insertionDate, refrenceNum);
                    DXMessageBox.Show(this, result);
                    RefreshFields_OnClick(null, null);
           
            }
            catch (Exception)
            {
                //
            }
        }


        private void UpdateStockBtn_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_product==null||_getStockStore == null) return;

                float prixAchat = ConverToFloat(PrixAchatTxt.Text);
                int qteAchter = ConvertToInteger(QteAchterSpin.Value);
                float prixVenteGros = ConverToFloat(PrixVenteGrosTxt.Text);
                float prixVenteDetail = ConverToFloat(VentePriceUnitDetailTxt.Text);
                float prixVenteComptoire = ConverToFloat(VentePriceUnitComptoireTxt.Text);
                float totalPriceHt = ConverToFloat(TotalPrixTxt.Text);
                string stockageId = StockageIDtxt.Text;
                string productSnumber = ProductSNtxt.Text;
                string productState = ProductStateCBX.Text;
                string stockObs = StockObStxt.Text;
                string refrenceNum = ReferenceTxt.Text;
                DateTime insertionDate = ProductSaleDateDte.DateTime;

                string result = _stockManager.UpdateStockStore(_getStockStore, prixAchat, qteAchter,
                    prixVenteGros, prixVenteDetail, prixVenteComptoire, totalPriceHt, stockageId, productSnumber, productState, stockObs, insertionDate, refrenceNum);
                DXMessageBox.Show(this, result, "Resultat", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                RefreshFields_OnClick(null, null);
            }
            catch (Exception)
            {
                //
            }
        }


        private void RemoveFromStockBtn_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_getStockStore == null) return;
                if (DXMessageBox.Show(this, "Êtes-vous sûr de vouloir supprimer le stock?", "Confirmation",
                    MessageBoxButton.YesNo) == MessageBoxResult.No) return;
                string result = _stockManager.DeleteStockStore(_getStockStore);
                DXMessageBox.Show(this, result);
                RefreshFields_OnClick(null, null);
          ;
            }
            catch (Exception)
            {
                //
            }
        }

        private void RefreshFields_OnClick(object sender, RoutedEventArgs e)
        {
            RefreshFields.IsEnabled = false;
            ClearStockFields();
            ClearProductFields();
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += StockDoWork;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.RunWorkerCompleted += StockLoadCompleted;
            backgroundWorker.RunWorkerAsync();
        }

        private void ClearProductFields()
        {
            try
            {
                _product = null;
                CategoryCbx.Clear();
                SubCategoryCbx.Clear();
                ProductNameCbx.Clear();
                ReferenceTxt.Clear();
                DesignationTxt.Clear();
                MeasureTxt.Clear();
                ProductReferenceTxt.Clear();
                ProductMinValueTxt.Clear();
                ProductMaxValueTxt.Clear();
                QteInStockTxt.Clear();
            }
            catch (Exception)
            {
                //
            }
        }
       

        private void ClearStockFields()
        {
            _getStockStore = null;
            ProductSaleDateDte.Clear();
            ReferenceTxt.Clear();
            PrixAchatTxt.Clear();
            QteAchterSpin.Clear();
            PrixVenteGrosTxt.Clear();
            VentePriceUnitDetailTxt.Clear();
            VentePriceUnitComptoireTxt.Clear();
            TotalPrixTxt.Clear();
            StockageIDtxt.Clear();
            ProductSNtxt.Clear();
            ProductStateCBX.Clear();
            StockObStxt.Clear();
        }

        private static float ConverToFloat(string prixText)
        {
            try
            {
                float prix;
                bool theResult = float.TryParse(prixText, NumberStyles.Any, CultureInfo.InvariantCulture, out prix);
                if (!theResult)
                {
                    prix = 0;
                }
                return prix;
            }
            catch (Exception)
            {
                return 0;
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

        private void QteAchterSpin_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            try
            {
                float prixAchat = ConverToFloat(PrixAchatTxt.Text);
                int qteAchter = ConvertToInteger(QteAchterSpin.Value);
                TotalPrixTxt.Text = (prixAchat*qteAchter).ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                //
            }
        }

        private void PrixAchatTxt_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            QteAchterSpin_OnEditValueChanged(null, null);
        }

        private void StockTableView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (StockGridControl.VisibleRowCount == 0)
                {
                    return;
                }
                int rowHandle = StockTableView.FocusedRowHandle;
                if (rowHandle < 0) return;
                ClearStockFields();
                ClearProductFields();
                int stockStoreId = (int)StockGridControl.GetCellValue(rowHandle, "N° Stock");
                StockStore getStockStore = StockManager.GetStockStoreById(stockStoreId);
                if (getStockStore == null) return;
                _product = _stockManager.GetProduct(getStockStore);
                LoadProductFields(getStockStore);
                LoadStockFields(getStockStore);
            }
            catch (Exception)
            {
                //
            }
        }

        private void LoadProductFields(StockStore getStockStore)
        {
            try
            {
                if (getStockStore == null) return;

                CategoryCbx.Text = getStockStore.Product.SubCategory.Category.CategoryName;
                SubCategoryCbx.Text = getStockStore.Product.SubCategory.SubCategoryName;
                ProductNameCbx.Text = getStockStore.Product.ProductName;
                ReferenceTxt.Text = getStockStore.RefrenceNum;

                DesignationTxt.Text = getStockStore.Product.Designation;
                MeasureTxt.Text = getStockStore.Product.MeasureUnit;
                ProductMinValueTxt.Text = getStockStore.Product.ProductMinQte.ToString();
                ProductMaxValueTxt.Text = getStockStore.Product.ProductMaxQte.ToString();
                if (getStockStore.ProductID == null) return;
                int productId = (int) getStockStore.ProductID;
                QteInStockTxt.Text = GetQteInStock(productId);
            }
            catch (Exception)
            {
                //
            }
        }
      
        private void LoadStockFields(StockStore getStockStore)
        {
            _getStockStore = getStockStore;
            if (getStockStore == null) return;
            if (getStockStore.InsertionDate != null)
            {
                DateTime getDate = (DateTime)getStockStore.InsertionDate;
                ProductSaleDateDte.Text = getDate.ToString("d");
            }

            //***********************************
            PrixAchatTxt.Text =getStockStore.PurchasePrice.ToString();
            if (getStockStore.UnitsOnOrder != null)
                QteAchterSpin.Text = ConvertToInteger((decimal) getStockStore.UnitsOnOrder).ToString();
            PrixVenteGrosTxt.Text= getStockStore.VentePriceGros.ToString();
            VentePriceUnitDetailTxt.Text = getStockStore.VentePriceDetail.ToString();
            VentePriceUnitComptoireTxt.Text = getStockStore.VentePriceComptoire.ToString();
            TotalPrixTxt.Text = getStockStore.TotalPriceAchat.ToString();
            StockageIDtxt.Text = getStockStore.StockageID;
            ProductSNtxt.Text = getStockStore.Serialnumber;
            ProductStateCBX.Text = getStockStore.ProductState;
            StockObStxt.Text = getStockStore.Observation;
            ReferenceTxt.Text = getStockStore.RefrenceNum;
        

        }

        private string GetStingDecimalFormat(decimal? ventePriceGros)
        {
            if (ventePriceGros == null) return "0";
            decimal result = (decimal) ventePriceGros;
            return result.ToString("n2");
          
        }

        private decimal ConvertToFloat(string stringVal)
        {
            try
            {
                decimal decimalVal = Convert.ToDecimal(stringVal);
                return decimalVal;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        private void CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            StockGridControl.CopyToClipboard();
        }

     

        private void CategoryCbx_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            string CategoryName = CategoryCbx.Text;
            if (string.IsNullOrEmpty(CategoryName) || string.IsNullOrWhiteSpace(CategoryName)) return;
            SubCategoryCbx.Clear();
            ProductNameCbx.Clear();
            MeasureTxt.Clear();
            DesignationTxt.Clear();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SubCategoryCbx.ItemsSource = _CategorysClient.GetSubCategorysNames(CategoryName);

            }));

        }

        private void SubCategoryCbx_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            string subCategoryName = SubCategoryCbx.Text;
            string CategoryName = CategoryCbx.Text;
            if (string.IsNullOrEmpty(subCategoryName) || string.IsNullOrWhiteSpace(subCategoryName))
            {
                ProductNameCbx.ItemsSource=new List<string>();
                MeasureTxt.Clear();
               DesignationTxt.Clear();
                return;
            }
            if (string.IsNullOrEmpty(CategoryName) || string.IsNullOrWhiteSpace(CategoryName))
            {
                SubCategoryCbx.ItemsSource = new List<string>();
                ProductNameCbx.ItemsSource = new List<string>();
                ProductNameCbx.Clear();
                MeasureTxt.Clear();
               DesignationTxt.Clear();
                return;
            }
            ProductNameCbx.Clear();
            MeasureTxt.Clear();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ProductNameCbx.ItemsSource = _CategorysClient.GetProducts(CategoryName,subCategoryName);
                ProductNameCbx.DisplayMember = "ProductName";
                ProductNameCbx.ValueMember = "ProductID";
            }));

        }

        private void ProductNameCbx_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            try
            {
                int productId = (int) ProductNameCbx.EditValue;
                var getProduct = _productManger.GetProductById(productId);
                if(getProduct==null)return;
                LoadProductFielsd(getProduct);
                
            }
            catch (Exception)
            {
                //
            }
         
        }

        private void LoadProductFielsd(Product getProduct)
        {

            _product = getProduct;
            MeasureTxt.Text = getProduct.MeasureUnit;
            DesignationTxt.Text = getProduct.Designation;
            ProductMinValueTxt.Text = getProduct.ProductMinQte.ToString();
            ProductReferenceTxt.Text = getProduct.ReferenceExterne;
            ProductMaxValueTxt.Text = getProduct.ProductMaxQte.ToString();
            QteInStockTxt.Text = GetQteInStock(getProduct.ProductID);
        }

        private string GetQteInStock(int  getProductId)
        {

            try
            {
                decimal getQte = _stockManager.GetStockQte(getProductId);
                return getQte.ToString("###,###.00");
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
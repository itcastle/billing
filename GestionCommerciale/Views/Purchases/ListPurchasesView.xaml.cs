using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Core;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.ClassesClients;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.Helpers;
using GestionCommerciale.Views.Sales;

namespace GestionCommerciale.Views.Purchases
{
    /// <summary>
    /// Interaction logic for ListSuppliersView.xaml
    /// </summary>
    public partial class ListPurchasesView
    {
       
           
   
        private List<Purchase> _listPurchases; 
        private List<Provider> _suppliersList;
       
        private PurchaseClient _purchaseClient;

        private readonly CategorysClient _categorysClient;
        private Purchase _getPurchase;
        private readonly SuppliersManager _suppliersManager;
        private Provider _getProvider;
        private readonly TabHelper _tabHlp;
        public ListPurchasesView(string animationName, TabHelper hlp)
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            _tabHlp = hlp;
            if (!string.IsNullOrEmpty(animationName))
            {
                Storyboard animation = (Storyboard)Application.Current.Resources[animationName];
                LayoutRoot.BeginStoryboard(animation);
            }
            
            _suppliersManager = new SuppliersManager();
            _purchaseClient=new PurchaseClient();
            _categorysClient=new CategorysClient();
        }

        private void NewSaleBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var item = _tabHlp.AddNewTab(typeof(NewSaleView), "Effectuer une vente ", "FadeToLeftAnim",_tabHlp);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProviderGridControl();
            LoadCategoryAndSubCategoryCbx();
        }

        private void LoadCategoryAndSubCategoryCbx()
        {

            try
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CategoryCBX.ItemsSource = _categorysClient.GetCategorysNames();

                }));
            }
            catch (Exception)
            {
                //
            }
        }

        private void SubCategoryCBX_OnGotFocus(object sender, RoutedEventArgs e)
        {
            string CategoryName = CategoryCBX.Text;
            if (string.IsNullOrEmpty(CategoryName) || string.IsNullOrWhiteSpace(CategoryName)) return;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                SubCategoryCBX.ItemsSource = _categorysClient.GetSubCategorysNames(CategoryName);

            }));

        }
        private void LoadPurchasesOfProvider(Provider theProvider)
        {
            //*******************************************
            var getPurchases = new BackgroundWorker();
            getPurchases.DoWork += GetPurchasesOfProviderOnDoWork;
            getPurchases.RunWorkerCompleted += GetPurchasesOfProviderOnWorkerCompleted;
            getPurchases.RunWorkerAsync(theProvider);
            //*******************************************
          
        }

        private void GetPurchasesOfProviderOnDoWork(object sender, DoWorkEventArgs e)
        {
            _purchaseClient=new PurchaseClient();
            Provider receiveProvider = (Provider) e.Argument;
            _listPurchases = _purchaseClient.GetPurchasesByProvider(receiveProvider);
            e.Result = _listPurchases;

        }

        private void GetPurchasesOfProviderOnWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _listPurchases = (List<Purchase>) e.Result;
            PurchaseGridControl.ItemsSource = from t in _listPurchases

                select new
                {
                    PurchaseID = t.PurchaseID,
                    PurchaseDate = t.PurchaseDate,
                    CommandeDate = t.CommandeDate,
                    Description = t.Description,
                    ModeAchat = t.ModeAchat,
                    ModePaiement = t.ModePaiement,
                    EtatPaiement = t.EtatPaiement,
                    FactureNum = t.FactureNum,
                    FacturePrice = t.FacturePrice,
                    PurchaseMontant = t.PurchaseMontant,
                    PurchaseValidite = t.PurchaseValidite,
                    Status = t.Status,
                    Timbre = t.Timbre
                };
        }

        private void LoadProviderGridControl()
        {
            var getProviders = new BackgroundWorker();
            getProviders.DoWork += GetAllProvidersOnDoWork;
            getProviders.RunWorkerCompleted += GetAllProvidersOnRunWorkerCompleted;
            getProviders.RunWorkerAsync();
           
        }
         private void GetAllProvidersOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
         {

             var suppliersList = _suppliersManager.GetTheSuppliers();
             doWorkEventArgs.Result = suppliersList;
         }

         private void GetAllProvidersOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs workerCompleted)
         {
             _suppliersList = workerCompleted.Result as List<Provider>;
             if (_suppliersList == null) return;
             ProviderGridControl.ItemsSource = (from t in _suppliersList
                                                select new
                                                {
                                                    CompanyName = t.CompanyName,
                                                    ContactName = t.ContactName,
                                                    ContactTitle = t.ContactTitle,
                                                    Address=t.Address, 
                                                    RC = t.RC
                                                }
                 ).ToList();
         }

     

     

        private void LoadPurchaseStoreGridControl(Purchase thePurchase)
        {
            var getPurchaseStores = new BackgroundWorker();
            getPurchaseStores.DoWork += GetPurchaseStoresFromPurchaseOnDoWork;
            getPurchaseStores.RunWorkerCompleted += GetPurchaseStoresFromPurchaseOnRunWorkerCompleted;
            getPurchaseStores.RunWorkerAsync(thePurchase);
        
        }

        private void GetPurchaseStoresFromPurchaseOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var getPurchaseStores = (List<PurchaseStore>) e.Result;

            PurchaseStoreGridControl.ItemsSource = from t in getPurchaseStores

                select new
                {
              
                    ProductName = t.Product.ProductName,
                    CategoryName = t.Product.SubCategory.Category.CategoryName,
                    UnitsOnOrder = t.UnitsOnOrder, 
                    PurchasePrice = t.PurchasePrice,
                    TotalPriceAchat = t.TotalPriceAchat,
                    Discount = t.Discount,
                    TvaValue = t.TvaValue,
                    VentePriceGros = t.VentePriceGros,
                    PurchaseStoreID = t.PurchaseStoreID
                  
                }
                ;
        }

        private void GetPurchaseStoresFromPurchaseOnDoWork(object sender, DoWorkEventArgs e)
        {

            Purchase thePurchase = (Purchase) e.Argument;
            _purchaseClient = new PurchaseClient();
            var getPurchaseStores = _purchaseClient.GetPurchaseStoresList(thePurchase);
            e.Result = getPurchaseStores;
        }


     
      

       
        private void ProviderTableView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (ProviderGridControl.VisibleRowCount == 0) return;
            int rowHandle = ProviderGridControl.View.FocusedRowHandle;
            if (rowHandle < 0) return;
         SuppliersManager suppliersManager=new SuppliersManager();
            string companyName = ProviderGridControl.GetCellValue(rowHandle, "CompanyName").ToString();
            _getProvider = suppliersManager.GetProviderByName(companyName);

            if (_getProvider == null) return;
            LoadPurchasesOfProvider(_getProvider);
        }

        private void CustomersDataTable_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (PurchaseGridControl.VisibleRowCount == 0) return;
            int rowHandle = PurchaseGridControl.View.FocusedRowHandle;
            if (rowHandle < 0) return;
            _purchaseClient = new PurchaseClient();
            int purchaseId = Convert.ToInt32(PurchaseGridControl.GetCellValue(rowHandle, "PurchaseID").ToString());
            _getPurchase = _purchaseClient.GetPurchaseById(purchaseId);

            if (_getPurchase == null) return;
            LoadPurchaseStoreGridControl(_getPurchase);
           // OrderDateDateEdit.Text = order.OrderDate.ToString();
        }

        private void CustomersDataTable_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

            try
            {

                if (PurchaseGridControl.VisibleRowCount == 0) return;

                if (
                    DXMessageBox.Show(this, "Êtes-vous sûr de vouloir supprimer l'achat ?", "Confirmation",
                        MessageBoxButton.YesNo) == MessageBoxResult.No) return;
                int rowHandle = PurchaseGridControl.View.FocusedRowHandle;

                int purchaseId = (int) PurchaseGridControl.GetCellValue(rowHandle, "PurchaseID");
                string result = _purchaseClient.DelPurchase(purchaseId);
                DXMessageBox.Show(this, result);
                LoadPurchasesOfProvider(_getProvider);

            }
            catch (Exception)
            {
            
                //
            }
        }


        private void PurchaseStoreTableView_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            try
            {
                if (PurchaseStoreGridControl.VisibleRowCount == 0) return;
                int rowHandle = PurchaseStoreGridControl.View.FocusedRowHandle;
                if (rowHandle < 0) return;
                int purchaseStoreId = (int)PurchaseStoreGridControl.GetCellValue(rowHandle, "PurchaseStoreID");
                PurchaseStore thePurchaseStore = _purchaseClient.GetPurchaseStoreById(purchaseStoreId);
                if (thePurchaseStore == null)
                {
                    ClearPurchaseStoreFields();
                }
                else LoadPurchaseStoreFields(thePurchaseStore);
            }
            catch (Exception exe)
            {
               // 
            }
        }

        private void LoadPurchaseStoreFields(PurchaseStore thePurchaseStore)
        {
            CategoryCBX.Text = thePurchaseStore.Product.SubCategory.Category.CategoryName;
            SubCategoryCBX.Text = thePurchaseStore.Product.SubCategory.SubCategoryName;
            ProductCbx.Text = thePurchaseStore.Product.ProductName;
            if (thePurchaseStore.PurchasePrice != null)
            {
                decimal purchasePrice = (decimal) thePurchaseStore.PurchasePrice;
                AchatPriceTxt.Text = purchasePrice.ToString("###,###.00");
            }
            if (thePurchaseStore.UnitsOnOrder != null)
            {
                decimal unitsOnOrder = (decimal) thePurchaseStore.UnitsOnOrder;
                UnitsOnOrderTxt.Text = unitsOnOrder.ToString("###,###.00");
            }
            if (thePurchaseStore.VentePriceGros != null)
            {
                decimal ventePrice = (decimal) thePurchaseStore.VentePriceGros;
                VentePriceCbx.Text = ventePrice.ToString("###,###.00");
            }
            if (thePurchaseStore.Discount != null)
            {
                decimal discount = (decimal) thePurchaseStore.Discount;
                DiscountTxt.Text = discount.ToString("###,###.00");
            }
            if (thePurchaseStore.TotalPriceAchat != null)
            {
                decimal totalPriceAchat = (decimal) thePurchaseStore.TotalPriceAchat;
                TotalPriceAchatTxt.Text = totalPriceAchat.ToString("###,###.00");
            }
            

        }

        private void ClearPurchaseStoreFields()
        {
            

        }

       
    }
}

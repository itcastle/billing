using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using GestionCommerciale.DomainModel.Entities;
using DevExpress.Xpf.Grid;
using GestionCommerciale.Helpers;
using GestionCommerciale.DomainModel;

namespace GestionCommerciale.Views.SaleFolder
{
    /// <summary>
    /// Interaction logic for ListSuppliersView.xaml
    /// </summary>
    public partial class ListSalesView
    {
   
        readonly SuppliersManager _supplierClient;
        readonly PurchaseClient _purchaseClient;
        List<Customer> _customerList;
        List<Purchase> _purchaseList;

     
        readonly PurchaseDetailsManager _purchaseDetailsManager;
        CategorysClient cc;
        CategorysManager mc;
    
        List<PurchaseStore> _purchaseStoresList;
        private readonly TabHelper _tabHlp;

        public ListSalesView(string animationName, TabHelper hlp)
        {
            InitializeComponent();
            _tabHlp = hlp;
           
          
            _supplierClient = new SuppliersManager();
            _purchaseClient = new PurchaseClient();
            EditPurchaseBtn.Visibility = Visibility.Collapsed;
            CancelPurchaseBtn.Visibility = Visibility.Collapsed;
            image1.Visibility = Visibility.Collapsed;
            image2.Visibility = Visibility.Collapsed;
         
            _purchaseDetailsManager = new PurchaseDetailsManager();
            cc = new CategorysClient();
           
            mc = new CategorysManager();
            if (string.IsNullOrEmpty(animationName)) return;
            Storyboard animation = (Storyboard)Application.Current.Resources[animationName];
            LayoutRoot.BeginStoryboard(animation);
        }

        //private void NewPurchaseBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    var item = _tabHlp.AddNewTab(typeof(AddPurchaseView), "Effectuer un achat ", "FadeToLeftAnim",_tabHlp);
        //}
        //private void AddSupplierBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    var item = _tabHlp.AddNewTab(typeof(AddSupplierView), "Nouveau fournisseur ", "FadeToLeftAnim");
        //}

        private void PurchasesDataTable_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (SuppliersDataGrid.VisibleRowCount == 0) return;
            int rowHandle = SuppliersDataGrid.View.FocusedRowHandle;

            if (rowHandle < 0) return;
            String name = SuppliersDataGrid.GetCellValue(rowHandle, "CompanyName").ToString();

            var getSuppliers = _supplierClient.GetProviderByName(name);
            if (getSuppliers == null) return;
            LoadGridPurchases(getSuppliers);
        }


        private void LoadGridPurchases(Provider sup)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _purchaseList = _purchaseClient.GetPurchase();
                if (_purchaseList != null)
                    PurchasesDataGrid.ItemsSource = (from t in _purchaseList
                        where t.SupplierID == sup.SupplierID
                        select new
                        {
                            PurchaseID = t.PurchaseID,
                            PurchaseDate = t.PurchaseDate,
                            RequiredDate = t.CommandeDate,
                          //  Employee = t.Employee.EmployeeFirstname,
                            Description = t.Description,
                        }
                        ).ToList();
            }));

        }

        private void LoadGridSuppliers()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _customerList = _supplierClient.GetCustomers();
                if (_customerList == null) return;
                SuppliersDataGrid.ItemsSource = (from t in _customerList
                    select new
                    {
                        CompanyName = t.CompanyName,
                        ContactName = t.ContactName,
                        ContactTitle = t.ContactTitle,
                        //Phone = t.Telephones.Phone,
                        RC = t.RC
                    }
                    ).ToList();
            }));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadGridSuppliers();
        }

        private void tableView3_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
           
            if (SelectedProductsDataGrid.VisibleRowCount == 0) return;
            int rowHandle = SelectedProductsDataGrid.View.FocusedRowHandle;

            if (rowHandle < 0) return;
            PurchaseStore purchaseDetails = _purchaseStoresList.ElementAt(rowHandle);
            if (purchaseDetails == null) return;
            Afficher(purchaseDetails);
        }

        private void LoadGridProducts(Purchase purchase)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _purchaseStoresList = _purchaseDetailsManager.GetPurchaseDetails();
                if (_purchaseStoresList != null)

                    SelectedProductsDataGrid.ItemsSource = (from t in _purchaseStoresList
                        where t.PurchaseID == purchase.PurchaseID
                        select new
                        {
                            ProductName = t.Product.ProductName,
                            CategoryName = t.Product.SubCategory.Category.CategoryName,
                            UnitsInStock = t.Product.UnitsInStock,
                            PurchasePrice = t.Product.PurchasePrice,
                            SalePrice = t.Product.SalePrice,
                         //   Mesure = t.Products.Measures.MesureName,
                        }
                        ).ToList();
            }));

        }

        private void Afficher(PurchaseStore purchaseDetails)
        {
            QuantiteSaleTxtBox.Text = purchaseDetails.UnitsOnOrder.ToString();
            
        }

        private void SuppliersDataTable_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (PurchasesDataGrid.VisibleRowCount == 0) return;
            int rowHandle = PurchasesDataGrid.View.FocusedRowHandle;
            if (rowHandle < 0) return;
            PurchaseClient purchaseClient = new PurchaseClient();
            int purchaseId = (int) PurchasesDataGrid.GetCellValue(rowHandle, "PurchaseID");
            Purchase getPurchase = purchaseClient.GetPurchaseById(purchaseId);
            if (getPurchase == null) return;
            LoadGridProducts(getPurchase);
        }

        private void EditPurchaseBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelPurchaseBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}

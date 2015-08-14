using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using GestionCommerciale.Helpers;
using GestionCommerciale.Views.CustomerFolder;
using GestionCommerciale.Views.EmployeeFolder;
using GestionCommerciale.Views.InvoiceFolder;
using GestionCommerciale.Views.Options;
using GestionCommerciale.Views.ProductFolder;
using GestionCommerciale.Views.PurchaseFolder;
using GestionCommerciale.Views.SaleFolder;
using GestionCommerciale.Views.StatisticFolder;
using GestionCommerciale.Views.StockFolder;
using GestionCommerciale.Views.SupplierFolder;
using GestionCommerciale.DomainModel;
using MahApps.Metro.Controls;
using System.Windows.Controls;
namespace GestionCommerciale
{
    public partial class MainWindow:MetroWindow
    {
        private TabHelper TabHlp;
      
        public MainWindow()
        {
            InitializeComponent();
            TabHlp = new TabHelper(MainTabControl);
            TopMenu topMenu = new TopMenu(TabHlp, this,HeaderImage);
            TopMenuGrid.Children.Add(topMenu);

            TabHlp.CreateAddViews("FadeToLeftAnim", TabHlp, this, HeaderImage);

        }


        #region Nav_Btns_Events
        private void AddPurchaseNavBtn_Click(object sender, EventArgs e)
        {
            var item = TabHlp.AddNewTab(typeof(AddPurchaseView), "Effectuer un achat ", "FadeToLeftAnim",TabHlp);
        }

        private void AddSaleNavBtn_Click(object sender, EventArgs e)
        {
            var item = TabHlp.AddNewTab(typeof(AddSaleView), "Effectuer une vente ", "FadeToLeftAnim", TabHlp);
        }

        private void AddSupplierNavBtn_click(object sender, EventArgs e)
        {
            var item = TabHlp.AddNewTab(typeof(AddSupplierView), "Nouveau fournisseur ", "FadeToLeftAnim");
        }

        private void AddCustomerNavBtn_Click(object sender, EventArgs e)
        {
            var item = TabHlp.AddNewTab(typeof(AddCustomerView), "Nouveau client ", "FadeToLeftAnim");
        }

        private void AddEmployeeNavBtn_Click(object sender, EventArgs e)
        {
            var item = TabHlp.AddNewTab(typeof(AddEmployeeView), "Nouveau employée ", "FadeToLeftAnim");
        }

        private void AddProductNavBtn_Click(object sender, EventArgs e)
        {

            var item = TabHlp.AddNewTab(typeof(AddProductView), "Nouveau produit ", "FadeToLeftAnim");

        }
        private void ListSupplierNavBtn_Click(object sender, EventArgs e)
        {
            var item = TabHlp.AddNewTab(typeof(ListSuppliersView), "Mes fournisseurs ", "FadeToLeftAnim", TabHlp);
        }
        private void ListCustomersNavBtn_Click(object sender, EventArgs e)
        {
            var item = TabHlp.AddNewTab(typeof(ListCustomersView), "Mes clients ", "FadeToLeftAnim", TabHlp);
        }
        private void ListEmployeesNavBtn_Click(object sender, EventArgs e)
        {
            var item = TabHlp.AddNewTab(typeof(ListEmployeesView), "Mes employées ", "FadeToLeftAnim", TabHlp);
        }
        private void ListProductsNavBtn_Click(object sender, EventArgs e)
        {
            var item = TabHlp.AddNewTab(typeof(ListProductsView), "Mes produits ", "FadeToLeftAnim", TabHlp);
        }

        private void ListPurchasesNavBtn_Click(object sender, EventArgs e)
        {
            var item = TabHlp.AddNewTab(typeof(ListPurchasesView), "Mes achats ", "FadeToLeftAnim", TabHlp);
        }

        private void ListSalesNavBtn_Click(object sender, EventArgs e)
        {
            var item = TabHlp.AddNewTab(typeof(ListSalesView), "Mes ventes ", "FadeToLeftAnim", TabHlp);
        }
        private void StockStateNavBtn_Click(object sender, EventArgs e)
        {
            var item = TabHlp.AddNewTab(typeof(StockState), "Etat de stock ", "FadeToLeftAnim");
        }
        private void ListInvoicesNavBtn_Click(object sender, EventArgs e)
        {
            var item = TabHlp.AddNewTab(typeof(AddFactureView), "Mes factures ", "FadeToLeftAnim");
        }
        private void NewInvoiceNavBtn_Click(object sender, EventArgs e)
        {
            var item = TabHlp.AddNewTab(typeof(FactureView), "Nouvelle facture ", "FadeToLeftAnim");
        }
        private void StockInitializeNavBtn_OnClick(object sender, EventArgs e)
        {

            var item = TabHlp.AddNewTab(typeof(StockView), "Initialise Produit ", "FadeToLeftAnim");
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
                        
            try
            {
                SaveBdd.BackupBdd();
            }
            catch (Exception)
            {
            }
            var item = TabHlp.AddNewTab(typeof(AddProductView), "Nouveau produit ", "FadeToLeftAnim");
           
        }


        private void FlayoutBtn_OnClick(object sender, RoutedEventArgs e)
        {

            this.SettingsFlyout.IsOpen = !this.SettingsFlyout.IsOpen;
        }
    }

}

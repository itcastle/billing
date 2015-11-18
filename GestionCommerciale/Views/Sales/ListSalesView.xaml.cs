using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.Helpers;

namespace GestionCommerciale.Views.Sales
{
    /// <summary>
    /// Interaction logic for ListSalesView.xaml
    /// </summary>
    public partial class ListSalesView
    {
        private readonly CustomersManager _customersMng =new CustomersManager();
        private List<Customer> _customersList;
        private readonly TabHelper _tabHlp;
        private readonly TvaClient _tvaClient;
        readonly ProductManger _productManger=new ProductManger();
        List<Product> _productsList=new List<Product>();

        public ListSalesView(string animationName, TabHelper hlp)
        {
            InitializeComponent();
            _tabHlp = hlp;
            if (!string.IsNullOrEmpty(animationName))
            {
                Storyboard animation = (Storyboard)Application.Current.Resources[animationName];
                LayoutRoot.BeginStoryboard(animation);
            }
        }

        private void ThisWindows_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void LoadGridAllProducts()
        {
            var getProducts = new BackgroundWorker();
            getProducts.DoWork += GetAllProductsOnDoWork;
            getProducts.RunWorkerCompleted += GetAllProductsOnRunWorkerCompleted;
            getProducts.RunWorkerAsync();
        }

        private void GetAllProductsOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var productsList = _productManger.GetProducts();
            doWorkEventArgs.Result = productsList;

        }

        private void GetAllProductsOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs workerCompleted)
        {
            _productsList = workerCompleted.Result as List<Product>;
            SalesDataGrid.ItemsSource = _productsList;

            UpdateProductsDataGridsFromSources();
            CalculateAndDisplay();
        }

        private void CalculateAndDisplay()
        {
            

        }

        private void UpdateProductsDataGridsFromSources()
        {
            

        }


        private void LoadGridAllCustomers()
        {
            var getCustomers = new BackgroundWorker();
            getCustomers.DoWork += GetAllCustomersOnDoWork;
            getCustomers.RunWorkerCompleted += GetAllCustomersOnRunWorkerCompleted;
            getCustomers.RunWorkerAsync();

        }

        private void GetAllCustomersOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {

            var customersList = _customersMng.GetAllCustomers();
            doWorkEventArgs.Result = customersList;
        }

        private void GetAllCustomersOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs workerCompleted)
        {
            _customersList = workerCompleted.Result as List<Customer>;
            CustomersLookUpEdit.ItemsSource = _customersList;


        }



        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Invoice_Button_Click(object sender, RoutedEventArgs e)
        {

        }

       

        private void SalesDataGrid_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private void AddProductBtn_Click(object sender, RoutedEventArgs e)
        {
            

        }

        private void SelectedProductsTableView_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            

        }

        private void DiscountTxtBox_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            

        }

        private void TvaComboBox_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            

        }

        private void AddTvaBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            

        }
    }
}

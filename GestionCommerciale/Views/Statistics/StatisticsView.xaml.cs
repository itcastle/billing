using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Charts;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.Entities;
using GroupBox = DevExpress.Xpf.LayoutControl.GroupBox;

namespace GestionCommerciale.Views.Statistics
{
    /// <summary>
    /// Interaction logic for AddSupplierView.xaml
    /// </summary>
    public partial class StatisticsView
    {
        readonly PurchaseClient _purchaseClient;
        readonly OrderClient _orderClient ;
        readonly ProductManger _productClient ;
        readonly SuppliersManager _suppliersManager ;
        readonly CustomersManager _customersManager ;
        public StatisticsView()
        {
            InitializeComponent();
        }
        public StatisticsView(string animationName)
        {
            InitializeComponent();
            _purchaseClient = new PurchaseClient();
            _orderClient = new OrderClient();
            _productClient = new ProductManger();
            _suppliersManager = new SuppliersManager();
            _customersManager = new CustomersManager();
            if (string.IsNullOrEmpty(animationName)) return;
            Storyboard animation = (Storyboard)Application.Current.Resources[animationName];
            LayoutRoot.BeginStoryboard(animation);
        }

        private void GroupBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            GroupBox groupBox = (GroupBox)sender;
            groupBox.Background = Brushes.AliceBlue;
        }

        private void GroupBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            GroupBox groupBox = (GroupBox)sender;
            groupBox.Background = null;
        }

        
        private void visualiserBtn_Click(object sender, RoutedEventArgs e)
        {
            //DateTime debut = BeginDateBox.DateTime;
            //DateTime fin = EndDateBox.DateTime;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           
            var list = _purchaseClient.GetPurchase();
            if (list != null)
            {
                foreach (var p in list)
                {
                    if (p.PurchaseDate == null) continue;
                    var sp1 = new SeriesPoint((DateTime)p.PurchaseDate, _purchaseClient.GetTotalPurchase(p));
                    Purchases.Series[0].Points.Add(sp1);
                    var sp2 = new SeriesPoint((DateTime)p.PurchaseDate, _purchaseClient.GetTotalPurchase(p));
                    Purchases.Series[1].Points.Add(sp2);
                }
            }


          
            var list2 = _orderClient.GetOrder();
            if (list2 != null)
            {
                foreach (Order o in list2)
                {
                    if (o.OrderDate == null) continue;
                    var sp3 = new SeriesPoint((DateTime)o.OrderDate, _orderClient.GetTotalOrders(o));
                    Sales.Series[0].Points.Add(sp3);
                    var sp4 = new SeriesPoint((DateTime)o.OrderDate, _orderClient.GetTotalOrders(o));
                    Sales.Series[1].Points.Add(sp4);
                }
            }




          
            var list3 = _productClient.GetProducts();
            if (list3 != null)
            {
                foreach (Product produit in list3)
                {
                    var sp5 = new SeriesPoint(produit.ProductName, _productClient.GetTotalProducts(produit));
                    Products.Series[0].Points.Add(sp5);
                    var sp6 = new SeriesPoint(produit.ProductName, _productClient.GetTotalProducts(produit));
                    Products.Series[1].Points.Add(sp6);
                }
            }

         
            var list4 = _suppliersManager.GetCustomers();
            if (list4 != null)
            {
                foreach (Customer sup in list4)
                {
                    var sp7 = new SeriesPoint(sup.CompanyName, _suppliersManager.GetTotalPerCustomer(sup));
                    Suppliers.Series[0].Points.Add(sp7);
                    var sp8 = new SeriesPoint(sup.CompanyName, _suppliersManager.GetTotalPerCustomer(sup));
                    Suppliers.Series[1].Points.Add(sp8);
                }
            }

         
         
            var list5 = _customersManager.GetCustomers();
            if (list5 != null)
            {
                foreach (Customer cust in list5)
                {
                    var sp9 = new SeriesPoint(cust.CompanyName, _customersManager.GetTotalPerCustomer(cust));
                    Customers.Series[0].Points.Add(sp9);
                    var sp10 = new SeriesPoint(cust.CompanyName, _customersManager.GetTotalPerCustomer(cust));
                    Customers.Series[1].Points.Add(sp10);
                }
            }
        }
    }
}

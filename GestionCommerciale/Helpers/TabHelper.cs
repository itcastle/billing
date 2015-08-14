using System;
using System.Windows.Controls;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
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

namespace GestionCommerciale.Helpers
{
    public class TabHelper
    {
        private AddProductView _addProductView;
        AddPurchaseView _addPurchaseView;
        AddSaleView _addSaleView;
        AddSupplierView _addSupplierView;
        AddCustomerView _addCustomerView;
        AddEmployeeView _addEmployeeView;
        private Options _optionsView;
        private StatisticsView _statisticsView;
        ListSuppliersView _listSuppliersView;
        ListCustomersView _listCustomersView; 
        ListEmployeesView _listEmployeesView;  
        ListProductsView _listProductsView;
        ListSalesView _listSalesView;
        ListPurchasesView _listPurchasesView;
        StockState _liStockState;
        AddFactureView _addFactureView;

       FactureView _factureView;
        StockView _stockView;

        private DXTabControl TabControl
        {
            get;
            set;
        }

        public TabHelper()
        {
            TabControl = null;
        }
        public TabHelper(DXTabControl tab)
        {
            TabControl = tab;
        }
        public DXTabItem AddNewTab(Type type, string header, params object[] ConstructArgs)
        {
            foreach (DXTabItem tabItem in TabControl.Items)
            {
                if ((string) tabItem.Header != header) continue;
                TabControl.SelectedItem = tabItem;
                return tabItem;
            }
        
           
            UserControl cntr = null;// = (UserControl)Activator.CreateInstance(type, ConstructArgs);
           
            if (type == typeof (AddProductView))
            {
                cntr = _addProductView;
            }
            else if (type == typeof(AddSaleView))
            {
                cntr = _addSaleView;
            }
            else if (type == typeof(AddPurchaseView))
            {
                cntr = _addPurchaseView;
            }
            else if (type == typeof(AddSupplierView))
            {
                cntr = _addSupplierView;
            }
            else if (type == typeof(AddCustomerView))
            {
                cntr = _addCustomerView;
            }
            else if (type == typeof(AddEmployeeView))
            {
                cntr = _addEmployeeView;
            }
            else if (type == typeof(ListSuppliersView))
            {
                cntr = _listSuppliersView;
            }
            else if (type == typeof(ListCustomersView))
            {
                cntr = _listCustomersView;
            }
          
            else if (type == typeof(ListEmployeesView))
            {
                cntr = _listEmployeesView;
            }
            else if (type == typeof(ListProductsView))
            {
                cntr = _listProductsView;
            }
            else if (type == typeof(ListSalesView))
            {
                cntr = _listSalesView;
            }
            else if (type == typeof(ListPurchasesView))
              {
                  cntr = _listPurchasesView;
              }
            else if (type == typeof(StockState))
            {
                cntr = _liStockState;
            }
            else if (type == typeof(AddFactureView))
            {
                cntr = _addFactureView;
            }
              else if (type == typeof(FactureView))
              {
                  cntr = _factureView;
              }
            else if (type == typeof(StockView))
            {
                cntr = _stockView;
            }
            else if(type==typeof(Options))
            {
                cntr = _optionsView;

            }
            else if (type == typeof (StatisticsView))
            {
                cntr = _statisticsView;
            }

            DXTabItem tabitem = new DXTabItem { Header = header, AllowHide =DefaultBoolean.True };
            TabControl.Items.Add(tabitem);
            tabitem.Content = cntr;
            TabControl.SelectedItem = tabitem;
            return tabitem;

        }


        public void CreateAddViews(string fadetoleftanim, TabHelper tabHlp, MainWindow mainWindow, Image headerImage)
        {
             _addProductView = new AddProductView(fadetoleftanim);
             _addPurchaseView = new AddPurchaseView(fadetoleftanim, tabHlp);
             _addSaleView = new AddSaleView(fadetoleftanim, tabHlp);
              _addSupplierView = new AddSupplierView(fadetoleftanim);
              _addCustomerView = new AddCustomerView(fadetoleftanim);
              _addEmployeeView = new AddEmployeeView(fadetoleftanim);

              _listSuppliersView = new ListSuppliersView(fadetoleftanim,tabHlp);
              _listCustomersView = new ListCustomersView(fadetoleftanim,tabHlp);
              _listEmployeesView = new ListEmployeesView(fadetoleftanim, tabHlp);
              _listProductsView = new ListProductsView(fadetoleftanim, tabHlp);
              _listSalesView = new ListSalesView(fadetoleftanim, tabHlp);
              _listPurchasesView = new ListPurchasesView(fadetoleftanim, tabHlp);
              _liStockState=new StockState();
              _addFactureView = new AddFactureView(fadetoleftanim, "facture");

              _factureView = new FactureView(fadetoleftanim);
              _stockView = new StockView(fadetoleftanim);
            _optionsView=new Options(mainWindow,headerImage,"");
            _statisticsView=new StatisticsView(fadetoleftanim);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Grid;
using DevExpress.XtraRichEdit;
using GestionCommerciale.DomainModel.ClassesClients;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.Helpers;

namespace GestionCommerciale.Views.Consomation
{
    /// <summary>
    /// Interaction logic for ConsomationView.xaml
    /// </summary>
    public partial class ConsomationView
    {
    
        private readonly TabHelper _tabHlp;

        public ConsomationView()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
           Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
        }
        public ConsomationView(string animationName)
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(animationName)) return;
            var animation = (Storyboard)Application.Current.Resources[animationName];
            LayoutRoot.BeginStoryboard(animation);
          
        }

        private void ConsomationView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var gestionDb = new GcdbEntities();
            var productsInStockList = gestionDb.StockStores.ToList();
            AllProductsGridControl.BeginDataUpdate();
            AllProductsGridControl.ItemsSource = productsInStockList;
            AllProductsTableView.BestFitColumns();
            AllProductsTableView.AutoWidth = false;
            AllProductsGridControl.EndDataUpdate();
        }


        private void AllProductsTableView_OnFocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {

            try
            {
                StockStore selectedStockStore = e.NewRow as StockStore;
                if (selectedStockStore != null)
                {
                    Product getProduct = selectedStockStore.Product;
                }
            }
            catch (Exception)
            {
                //
            }
        }
    }
}

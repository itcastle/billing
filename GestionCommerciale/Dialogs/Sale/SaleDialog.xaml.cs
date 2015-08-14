using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GestionCommerciale.DomainModel;

namespace GestionCommerciale.Modals
{
    /// <summary>
    /// Interaction logic for TVADialog.xaml
    /// </summary>
    public partial class SaleDialog : Window
    {
        public SaleDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GestionCommerciale.DomainModel.OrderClient sales = new GestionCommerciale.DomainModel.OrderClient();
            dgSales.ItemsSource = sales.GetOrder();
            
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

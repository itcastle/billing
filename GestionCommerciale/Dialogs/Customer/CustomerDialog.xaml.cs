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
using GestionCommerciale.DomainModel.ClassesClients;

namespace GestionCommerciale.Modals
{
    /// <summary>
    /// Interaction logic for TVADialog.xaml
    /// </summary>
    public partial class CustomerDialog : Window
    {
        public CustomerDialog()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CustomersManager customers = new CustomersManager();
            dgCustomers.ItemsSource = customers.GetCustomers();          
        }
    }
}

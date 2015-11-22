using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.ClassesClients;

namespace GestionCommerciale.Modals.TVA
{
    /// <summary>
    /// Interaction logic for TVADialog.xaml
    /// </summary>
    public partial class TVADialog : Window
    {
        public TVADialog()
        {
            InitializeComponent();
        }

        public int NBRows
        {
            get;
            set;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TvaClient tvas = new TvaClient();
            dgTVAs.ItemsSource = tvas.GetAllTvas();
            dgTVAs.Columns.RemoveAt(3);
                        
        }

        public GestionCommerciale.DomainModel.Entities.TVA Row
        {
            get;
            set;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {

            Row = (GestionCommerciale.DomainModel.Entities.TVA)dgTVAs.GetFocusedRow();
            DialogResult = true;
            Close();
        }
    }
}

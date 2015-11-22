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
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.ClassesClients;

namespace GestionCommerciale.Modals.TVA
{
    /// <summary>
    /// Interaction logic for AddTVADialog.xaml
    /// </summary>
    public partial class AddTVADialog : Window
    {
        public AddTVADialog()
        {
            InitializeComponent();
        }

        private void btnAddMeasure_Click(object sender, RoutedEventArgs e)
        {
            TvaClient tc = new TvaClient();
            GestionCommerciale.DomainModel.Entities.TVA tva = new  GestionCommerciale.DomainModel.Entities.TVA();
            //verify value validation
            tva.TauxTVA = float.Parse(TVATxtBox.Text);
            tva.Description = DescriptionTxtBox.Text;
            tc.AddTva(tva);
            DialogResult = true;
            Close();
        }
    }
}

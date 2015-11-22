using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.ClassesClients;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.Views.TVAs
{
    /// <summary>
    /// Interaction logic for AddMesureView.xaml
    /// </summary>
    public partial class AddTvaView : Window
    {
        readonly TvaClient _tvac = new TvaClient();
        public AddTvaView()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
        }

        private void AddTvaBtn_Click(object sender, RoutedEventArgs e)
        {
            TVA tva = new TVA();
            
        

            String input = null;

            input = TvaValueTxtBox.Text;
            if (input == null) input = "";

            if (/*!Validator.IsAlphanumericValid(input) || */ String.IsNullOrEmpty(input))
            { MessageBox.Show("Erreur dans le Nom de la TVA (ne doit pas être vide)"); return; }

            if (TvaClient.IsTvaExist(Convert.ToDouble(input)))
            { MessageBox.Show("TVA existe déjà"); return; };

            
                Decimal dec = Convert.ToDecimal(input);
                tva.TauxTVA = (float)dec;

            String s =_tvac.AddTva(tva);

            MessageBox.Show(s);
            Close();
        }
    }
}

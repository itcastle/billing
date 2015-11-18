using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using GestionCommerciale.DomainModel.ClassesClients;

namespace GestionCommerciale.Views
{
    /// <summary>
    /// Interaction logic for NewServiceWindow.xaml
    /// </summary>
    public partial class NewServiceWindow
    {
     
        string ServiceName = string.Empty;
        string ServicePrice = string.Empty;
        string ServiceFamille = string.Empty;
        public NewServiceWindow(string serviceName, string servicePrice, string serviceFamille)
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            ServiceName = serviceName;
            ServiceFamille = serviceFamille;
            ServicePrice = servicePrice;

        }

        public void Connect(int connectionId, object target)
        {
            

        }

        private void SaveBtn_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var serviceManager = new ServiceManager();
                int servicePriceHT = Convert.ToInt32(ServicePriceHTtxt.Text);
                serviceManager.CreateService(ServiceNameTxt.Text, ServiceFamilleTxt.Text, servicePriceHT);
                DialogResult = true;
                Close();

            }

            catch (Exception ex)
            {
                Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }
        }

        private static float ConverToFloat(string prixText)
        {
            try
            {
                float prix;
                bool theResult = float.TryParse(prixText, NumberStyles.Any, CultureInfo.InvariantCulture, out prix);
                if (!theResult)
                {
                    prix = 0;
                }
                return prix;
            }
            catch (Exception)
            {
                return 0;
            }

        }
      
        private void CancelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                
                    Close();
                
            }
            catch (Exception ex)
            {
                Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }

        }

        private void NewServiceWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

            ServiceNameTxt.Text = ServiceName;
            ServiceFamilleTxt.Text = ServiceFamille;
            ServicePriceHTtxt.Text = ServicePrice;
            if (ServiceNameTxt.Text.Length == 0)
            {
                ServiceNameTxt.Focus();
            }
            else if (ServiceFamilleTxt.Text.Length == 0)
            {
                ServiceFamilleTxt.Focus();
            }
            else if (ServicePriceHTtxt.Text.Length == 0)
            {
                ServicePriceHTtxt.Focus();
            }
            else
            {
                SaveServiceBtn.Focus();
            }
        }
    }
}

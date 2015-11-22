using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using GestionCommerciale.DomainModel.ClassesClients;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.Views.ServiceFolder;

namespace GestionCommerciale.Views
{
    /// <summary>
    /// Interaction logic for UpdateService.xaml
    /// </summary>
    public partial class UpdateService
    {
        private readonly int _serviceID;
        private readonly ServiceView _serviceViewUc;
        private readonly ServiceManager _serviceManager = new ServiceManager();

        public UpdateService(ServiceView serviceView, int serviceID)
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            _serviceID = serviceID;
            _serviceViewUc = serviceView;
        }

        public void Connect(int connectionId, object target)
        {


        }

        private void CancelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RemoveBtn_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceManager.DeleteService(_serviceID);
                ReloadAllServiceGridControl();
                Close();
            }
            catch (Exception)
            {
                //
            }

        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceManager.UpdateService(_serviceID, ServiceNameTxt.Text, ServiceFamilleTxt.Text,
                    ServicePriceSpin.Value);
                ReloadAllServiceGridControl();
                Close();
            }
            catch (Exception)
            {
                //
            }
        }

        private void ReloadAllServiceGridControl()
        {
            _serviceViewUc.LoadAllServices();
            _serviceViewUc.FocusServicesSearchColmn();


        }

        private void UpdateService_OnLoaded(object sender, RoutedEventArgs e)
        {
       
            service getService = _serviceManager.GetServiceByID(_serviceID);
            if (getService == null) return;
            ServiceNameTxt.Text =getService.ServiceName;
            ServiceFamilleTxt.Text =getService.ServiceFamilly;

            if (getService.ServicePrice != null) ServicePriceSpin.Value =(decimal) getService.ServicePrice;
        }
    }
}

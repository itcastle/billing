using System;
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
        private readonly ServiceView _serviceViewUC;

        public UpdateService(ServiceView serviceView, int serviceID)
        {
            InitializeComponent();
            _serviceID = serviceID;
            _serviceViewUC = serviceView;
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

            }
        }

        private void ReloadAllServiceGridControl()
        {
            _serviceViewUC.LoadAllServices();
            _serviceViewUC.FocusServicesSearchColmn();


        }

        private void UpdateService_OnLoaded(object sender, RoutedEventArgs e)
        {
            var serviceManager = new ServiceManager();
            service getService = serviceManager.GetServiceByID(_serviceID);
            if (getService == null) return;
            ServiceNameTxt.Text =getService.ServiceName;
            ServiceFamilleTxt.Text =getService.ServiceFamilly;

            if (getService.ServicePrice != null) ServicePriceSpin.Value =(decimal) getService.ServicePrice;
        }
    }
}

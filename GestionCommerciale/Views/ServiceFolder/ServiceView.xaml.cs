using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;
using GestionCommerciale.DomainModel.ClassesClients;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.FactureFolder;
using GestionCommerciale.Helpers;

namespace GestionCommerciale.Views.ServiceFolder
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ServiceView 
    {
        private List<service> _allServices;

        private DataTable _selectedServices;

        private int _ordreSelectedServiceNumber;
        private readonly TabHelper _tabHlp;
        private int _serviceFactureID;

        public string ServiceModelName { get; set; }

        public bool ExitEditModel { get; set; }

        public ServiceView(string animationName, TabHelper hlp)
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            _tabHlp = hlp;
            if (!string.IsNullOrEmpty(animationName))
            {
                Storyboard animation = (Storyboard)Application.Current.Resources[animationName];
                LayoutRoot.BeginStoryboard(animation);
            }
            _selectedServices = new DataTable();
            _selectedServices.Columns.Add("Ordre", typeof(int));
            _selectedServices.Columns.Add("Name", typeof(string));
            _selectedServices.Columns.Add("Famille", typeof(string));
            _selectedServices.Columns.Add("Price HT", typeof(int));
            _selectedServices.Columns.Add("Qte", typeof(int));
            _selectedServices.Columns.Add("TVA", typeof(float));
            _selectedServices.Columns.Add("Total TTC", typeof(float));
            _selectedServices.Columns.Add("Description", typeof(string));
            SelectedServicesDataGrid.ItemsSource = _selectedServices.DefaultView;
            SelectedServicesTableView.BestFitColumns();
        }
        private void TabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.Source is TabControl)) return;
            var tabcontrol = (TabControl)sender;

            foreach (TabItem item in tabcontrol.Items)
            {
                var itemheader = (Border)item.Header;
                itemheader.Background = Brushes.White;
                itemheader.Opacity = 0.6;
                itemheader.BorderBrush = Brushes.White;
            }

            var tab = (TabItem)tabcontrol.SelectedItem;
            var header = (Border)tab.Header;
            header.Opacity = 1;
            header.Background = Brushes.YellowGreen;
            header.BorderBrush = Brushes.DarkGreen;
        }

        private void Border_OnMouseEnter(object sender, MouseEventArgs e)
        {
            var header = (Border)sender;
            header.Opacity = 1;
        }

        private void Border_OnMouseLeave(object sender, MouseEventArgs e)
        {
            var header = (Border)sender;
            // ReSharper disable once PossibleUnintendedReferenceComparison
            if (header.Background == Brushes.White)
            {
                header.Opacity = 0.6;
            }
        }

        
        private void Window1_OnLoaded(object sender, RoutedEventArgs e)
        {
            EmptyServiceFieldsBtn.IsEnabled = false;
            AddServiceBtn.IsEnabled = false;
            RemoveServiceBtn.IsEnabled = false;
            UpdateServiceBtn.IsEnabled = false;

            LoadAllServices();
            LoadServiceFactureReport();
            FocusServicesSearchColmn();
            LoadServicesModelsDataGrid();
            LoadServiceFactureGridControl();
            LoadClientList();

        }
   
        public void Connect(int connectionId, object target)
        {
            

        }
    
        private void LoadClientList()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var clientManager = new ClientManager();
                IEnumerable<string> listClient = clientManager.GetClientList("Externe");
                ClientNameCBX.ItemsSource = listClient;
                ClientNameCBX.SelectedIndex = -1;
            }));

        }

        public void FocusServicesSearchColmn()
        {
            try
            {
                var originaleMousePos = MouseOperationsHelper.GetCursorPosition();
                Point locationFromWindow = AllServicesGridControl.TranslatePoint(new Point(0, 0), this);
                var locationFromScreen = AllServicesGridControl.PointToScreen(locationFromWindow);
                var x = (int)(locationFromScreen.X - locationFromWindow.X + 30);
                var y = (int)(locationFromScreen.Y - locationFromWindow.Y + 40);
                MouseOperationsHelper.SetCursorPosition(x, y);
                MouseOperationsHelper.MouseEvent(MouseOperationsHelper.MouseEventFlags.LeftDown);
                MouseOperationsHelper.MouseEvent(MouseOperationsHelper.MouseEventFlags.LeftDown);
                MouseOperationsHelper.SetCursorPosition(originaleMousePos.X, originaleMousePos.Y);
                MouseOperationsHelper.SetCursorPosition(originaleMousePos.X, originaleMousePos.Y);
            }
            catch (Exception ex)
            {
                //  Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }
        }

        private void LoadServiceFactureReport()
        {
            client getClient;
            try
            {
                string theClient = ClientNameCBX.Text;
                string[] clientSplit = theClient.Split('-');
                int clientID = Convert.ToInt32(clientSplit[0]);
                var clientManager = new ClientManager();
                getClient = clientManager.GetClientByID(clientID);
            }
            catch (Exception exe)
            {
                getClient = null;
            }

            try
            {
                string commandNumber = CommandeNumber.Text;
                DateTime commandeDate = CommandeDateDte.DateTime.Date;
                string modePayment = ModePaymentCBX.Text;
                string etatPayment = EtatPaymentCBX.Text;
                string factureType = FactureTypeCBX.Text;
                int serviceFactureID;
                if (_serviceFactureID == -1)
                {
                    serviceFactureID = ServiceManager.GetLastServiceFactureIDPlusOne();
                }
                else serviceFactureID = _serviceFactureID;
                var reportFactureService = new FactureService
                {
                    ServicesTable = _selectedServices,
                    TheClient = getClient,
                    TheServiceDate = ServiceDate.DateTime.Date,
                    TheCommandeNumber = commandNumber,
                    ModePayment = modePayment,
                    TheServiceFactureID=serviceFactureID,
                    EtatPayment = etatPayment,
                    FactureType = factureType,
                    CommandeDate = commandeDate
                };

                var model = new XtraReportPreviewModel(reportFactureService);
                reportFactureService.CreateDocument();
                DocPreview.Model = model;
                DocPreview.Model.ZoomMode = new ZoomFitModeItem(ZoomFitMode.PageWidth);
                DocPreview.Model.IsParametersPanelVisible = false;
                DocPreview.Model.OpenCommand.CanExecute(false);
            }
            catch (Exception exe)
            {
                //  Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }
        }

        public void LoadAllServices()
        {
            try
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _allServices = ServiceManager.GetAllServices();
                    AllServicesGridControl.ItemsSource = (from entity in _allServices
                                                          select new
                                                          {
                                                              ServiceColmn = entity.ServiceName,
                                                              PrixColmn = entity.ServicePrice,
                                                              FamilleColmn = entity.ServiceFamilly
                                                          }).ToList();
                }));
            }
            catch (Exception)
            {

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
     
        //*************************************************************************

        private void SelectedServicesDataGrid_OnItemsSourceChanged(object sender, ItemsSourceChangedEventArgs e)
        {
            LoadServiceFactureReport();
        }

        private void SelectedServicesTableView_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                UpdateServiceBtn.IsEnabled = true;
                RemoveServiceBtn.IsEnabled = true;

                EmptyServiceFieldsBtn.IsEnabled = true;
                AddServiceBtn.IsEnabled = false;

                int rowHandle = SelectedServicesTableView.FocusedRowHandle;
                if (rowHandle < 0)
                {
                    _ordreSelectedServiceNumber = -1;
                    UpdateServiceBtn.IsEnabled = false;
                    RemoveServiceBtn.IsEnabled = false;
                    return;
                }
                _ordreSelectedServiceNumber = rowHandle;

                ServiceNameTxt.Text = SelectedServicesDataGrid.GetCellValue(rowHandle, "Name").ToString();
                ServiceFamilleTxt.Text = SelectedServicesDataGrid.GetCellValue(rowHandle, "Famille").ToString();
                ServicePriceTxt.Text = SelectedServicesDataGrid.GetCellValue(rowHandle, "Price HT").ToString();
                ServiceQteSpin.Text = SelectedServicesDataGrid.GetCellValue(rowHandle, "Qte").ToString();
                ServiceTvaTxt.Text = SelectedServicesDataGrid.GetCellValue(rowHandle, "TVA").ToString();
                ServiceDescriptioTxt.Text = SelectedServicesDataGrid.GetCellValue(rowHandle, "Description").ToString();
                ServiceDXTabItem.IsSelected = true;
            }
            catch (Exception exe)
            {
                _ordreSelectedServiceNumber = -1;
                UpdateServiceBtn.IsEnabled = false;
                RemoveServiceBtn.IsEnabled = false;
                AddServiceBtn.IsEnabled = false;
            }
        }

        //*************************************************************************

        private void AllServicesTableView_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // update service with newservicewindow.xaml;
            try
            {
                int rowHandle = AllServicesTableView.FocusedRowHandle;
                string serviceName = AllServicesGridControl.GetCellValue(rowHandle, "ServiceColmn").ToString();
                int servicePrice = (int)AllServicesGridControl.GetCellValue(rowHandle, "PrixColmn");
                string serviceFamille = AllServicesGridControl.GetCellValue(rowHandle, "FamilleColmn").ToString();

                int serviceID = ServiceManager.GetServiceID(serviceName, serviceFamille, servicePrice);
                var updateServiceWindow = new UpdateService(this, serviceID)
                {
                    Title = "Mise à jour du service",
                    Icon =
                        new BitmapImage(
                        new Uri(
                        @"pack://application:,,,/UserInterface;component/resources/Images/Icons/edit.png")),
                };
                updateServiceWindow.ShowDialog();
            }
            catch (Exception)
            {
            }
        }

        private void AllServicesTableView_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (AllServicesGridControl.VisibleRowCount == 0)
                {
                    return;
                }

                EmptyServiceFieldsBtn.IsEnabled = true;
                AddServiceBtn.IsEnabled = true;

                UpdateServiceBtn.IsEnabled = false;
                RemoveServiceBtn.IsEnabled = false;

                int rowHandle = AllServicesTableView.FocusedRowHandle;
                if (rowHandle < 0) return;
                ServiceDescriptioTxt.Text = "";
                ServiceNameTxt.Text = AllServicesGridControl.GetCellValue(rowHandle, "ServiceColmn").ToString();
                ServicePriceTxt.Text = AllServicesGridControl.GetCellValue(rowHandle, "PrixColmn").ToString();
                ServiceFamilleTxt.Text = AllServicesGridControl.GetCellValue(rowHandle, "FamilleColmn").ToString();
            }
            catch (Exception)
            {
                EmptyServiceFieldsBtn.IsEnabled = false;
                AddServiceBtn.IsEnabled = false;

                UpdateServiceBtn.IsEnabled = false;
                RemoveServiceBtn.IsEnabled = false;
            }
        }
        
        //*************************************************************************

        private void AddServiceBtn_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {

                string serviceName = ServiceNameTxt.Text;
                float servicePrice = ConverToFloat(ServicePriceTxt.Text);
                string serviceFamille = ServiceFamilleTxt.Text;
                string serviceDescription = ServiceDescriptioTxt.Text;
                float tva = ConverToFloat(ServiceTvaTxt.Text);
                var qte = (int)ServiceQteSpin.Value;

                var row = _selectedServices.NewRow();
                row["Ordre"] = _selectedServices.Rows.Count + 1;
                row["Name"] = serviceName;
                row["Famille"] = serviceFamille;
                row["Price HT"] = servicePrice;
                row["Qte"] = qte;
                row["TVA"] = tva;
                row["Total TTC"] = (tva + 1) * servicePrice * qte;
                row["Description"] = serviceDescription;
                _selectedServices.Rows.Add(row);

                SelectedServicesDataGrid.ItemsSource = null;
                SelectedServicesDataGrid.ItemsSource = _selectedServices.DefaultView;
                SelectedServicesTableView.BestFitColumns();
                FocusServicesSearchColmn();

            }
            catch
            {
            }
        }

        private void EmptyServiceFieldsBtn_OnClick(object sender, RoutedEventArgs e)
        {
            UpdateServiceBtn.IsEnabled = false;
            RemoveServiceBtn.IsEnabled = false;
            AddServiceBtn.IsEnabled = false;
            EmptyServiceFieldsBtn.IsEnabled = false;

            _ordreSelectedServiceNumber = -1;
            ServiceNameTxt.Clear();
            ServiceFamilleTxt.Clear();
            ServicePriceTxt.Clear();
            ServiceQteSpin.Clear();
            ServiceTvaTxt.Clear();
            ServiceDescriptioTxt.Clear();
            ServiceDXTabItem.IsSelected = true;
        }

        private void UpdateServiceBtn_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_ordreSelectedServiceNumber < 0) return;
                //*********************************


                //************************
                string theOrdre = (_ordreSelectedServiceNumber + 1).ToString(CultureInfo.InvariantCulture);

                string serviceName = ServiceNameTxt.Text;
                float servicePrice = ConverToFloat(ServicePriceTxt.Text);
                string serviceFamille = ServiceFamilleTxt.Text;
                string serviceDescription = ServiceDescriptioTxt.Text;
                float tva = ConverToFloat(ServiceTvaTxt.Text);
                var qte = (int)ServiceQteSpin.Value;

                DataRow dr = _selectedServices.Select("Ordre='" + theOrdre + "'").FirstOrDefault();

                if (dr != null)
                {
                    dr["Name"] = serviceName;
                    dr["Famille"] = serviceFamille;
                    dr["Price HT"] = servicePrice;
                    dr["Qte"] = qte;
                    dr["TVA"] = tva;
                    dr["Total TTC"] = (tva + 1) * servicePrice * qte;
                    dr["Description"] = serviceDescription;
                    _selectedServices.AcceptChanges();
                    SelectedServicesDataGrid.ItemsSource = null;
                }
                try
                {
                    SelectedServicesDataGrid.ItemsSource = _selectedServices.DefaultView;
                    SelectedServicesTableView.BestFitColumns();

                    ServiceNameTxt.Clear();
                    ServiceFamilleTxt.Clear();
                    ServicePriceTxt.Clear();
                    ServiceQteSpin.Clear();
                    //  ServiceTvaTxt.Clear();
                    ServiceDescriptioTxt.Clear();

                    UpdateServiceBtn.IsEnabled = false;
                    RemoveServiceBtn.IsEnabled = false;
                    AddServiceBtn.IsEnabled = false;
                    EmptyServiceFieldsBtn.IsEnabled = false;

                }
                catch (Exception exe)
                {
                    DXMessageBox.Show(exe.ToString());
                }
            }
            catch (Exception)
            {


            }
        }

        private void RemoveServiceBtn_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                int rowHandle = SelectedServicesTableView.FocusedRowHandle;
                _selectedServices.Rows.RemoveAt(rowHandle);
                {
                    int i = 1;
                    foreach (DataRow dr in _selectedServices.Rows) // search whole table
                    {
                        dr["Ordre"] = i++; //update the ordre

                    }
                }
                SelectedServicesDataGrid.ItemsSource = null;
                SelectedServicesDataGrid.ItemsSource = _selectedServices.DefaultView;
                SelectedServicesTableView.BestFitColumns();

                UpdateServiceBtn.IsEnabled = false;
                RemoveServiceBtn.IsEnabled = false;
                AddServiceBtn.IsEnabled = false;
                EmptyServiceFieldsBtn.IsEnabled = false;
            }
            catch (Exception)
            {

            }
        }

        //*************************************************************************

        private void SaveServiceModelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            // save facture as model;
            try
            {
                if (_selectedServices.Rows.Count > 0)
                {
                    var newProtModelNameWindow = new NewModelNameWindow(this, "", "ServiceModel")
                    {
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };
                    newProtModelNameWindow.ShowDialog();

                }
                else
                    DXMessageBox.Show("Vous n'avez choisi aucun service !", "Erreur !", MessageBoxButton.OK,
                        MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }
        }

        public void SaveServiceModel(string modelName)
        {
           for (int i = 0; i < _selectedServices.Rows.Count; i++)
            {
                string serviceName = SelectedServicesDataGrid.GetCellValue(i, "Name").ToString();
                string serviceFamille = SelectedServicesDataGrid.GetCellValue(i, "Famille").ToString();
                var servicePrice = (int)SelectedServicesDataGrid.GetCellValue(i, "Price HT");
                var serviceQte = (int)SelectedServicesDataGrid.GetCellValue(i, "Qte");
                var serviceTva = (float)SelectedServicesDataGrid.GetCellValue(i, "TVA");
                string serviceDescription = SelectedServicesDataGrid.GetCellValue(i, "Description").ToString();

                int serviceID = ServiceManager.GetServiceID(serviceName, serviceFamille, servicePrice);
                ServiceManager.AttachServiceToServiceModel(modelName, serviceID, serviceName, serviceFamille, servicePrice, serviceQte, serviceTva,
                    serviceDescription);
            }
            LoadServicesModelsDataGrid();
        }

        //*************************************************************************

        private void LoadServicesModelsDataGrid()
        {

            Dispatcher.BeginInvoke(new Action(() =>
            {
                var dt = new DataTable();
                dt.Columns.Add("Liste des modèle");
                DataTable allModelsName = ServiceManager.GetAllServiceModels(dt);
                ModelsGridControl.ItemsSource = allModelsName.DefaultView;
                ModelsTableView.BestFitColumns();
            }));
        }

        private void ServiceModelsBtn_OnClick(object sender, RoutedEventArgs e)
        {
            LoadServicesModelsDataGrid();
        }

        private void ModelsTableView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            var row = (DataRowView)ModelsGridControl.GetFocusedRow();
            if (row == null) return;
            var modelName = (string)row[0];
            if (string.IsNullOrWhiteSpace(modelName)) return;
            _selectedServices.Rows.Clear();
         //   servicemodel getServicemodel = ServiceManager.GetServiceModel(modelName);
            
            

            _selectedServices = ServiceManager.GetservicesOfModel(modelName, _selectedServices);
            SelectedServicesDataGrid.ItemsSource = null;
            SelectedServicesDataGrid.ItemsSource = _selectedServices.DefaultView;
        }

        private void ModelsTableView_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var row = (DataRowView)ModelsGridControl.GetFocusedRow();
            if (row == null) return;
            var modelName = (string)row[0];
            var response = DXMessageBox.Show("Voulez vous retirer ce model?",
                "Confirmez la passage", MessageBoxButton.YesNoCancel);
            if (response != MessageBoxResult.Yes) return;
            var serviceManager = new ServiceManager();
            string result = ServiceManager.RemoveServiceModel(modelName);
            DXMessageBox.Show(result);
            LoadServicesModelsDataGrid();
        }

        //*************************************************************************

        private void RefreshBtn_OnClick(object sender, RoutedEventArgs e)
        {


            ServiceQteSpin.Value = 0;
            ServiceDate.Text = DateTime.Now.ToShortDateString();
            ModePaymentCBX.SelectedIndex = -1;
            EtatPaymentCBX.SelectedIndex = -1;
            ServiceTvaTxt.SelectedIndex = -1;
            ClientNameCBX.SelectedIndex = -1;
            CommandeNumber.Clear();
            CommandeDateDte.Clear();
            ServiceDXTabItem.IsSelected = true;

            EmptyServiceFieldsBtn_OnClick(null, null);
            LoadServicesModelsDataGrid();
            LoadServiceFactureGridControl();
            UpdateServiceBtn.IsEnabled = false;
            RemoveServiceBtn.IsEnabled = false;
            AddServiceBtn.IsEnabled = false;
            EmptyServiceFieldsBtn.IsEnabled = false;

            _selectedServices.Rows.Clear();
            SelectedServicesDataGrid.ItemsSource = null;
            SelectedServicesDataGrid.ItemsSource = _selectedServices.DefaultView;
            _serviceFactureID = -1;

        }

        private void SaveDocBtn_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_serviceFactureID != -1)
                {
                    UpdateServiceFacture(_serviceFactureID);
                }
                else if (_selectedServices.Rows.Count > 0)
                {
                    DateTime serviceFacturedate = ServiceDate.DateTime;
                    string serviceFactureType = FactureTypeCBX.Text;
                    string serviceModePayment = ModePaymentCBX.Text;
                    string serviceEtatPayement = EtatPaymentCBX.Text;
                    float serviceTvaValue = ConverToFloat(ServiceTvaTxt.Text);
                    string serviceClientFullName = ClientNameCBX.Text;
                    string serviceCommandeNumber = CommandeNumber.Text;
                    DateTime serviceCommandeDate = CommandeDateDte.DateTime;

                    int serviceFactureID = ServiceManager.CreateFactureService(serviceFacturedate, serviceFactureType,
                        serviceModePayment, serviceEtatPayement, serviceTvaValue, serviceClientFullName, serviceCommandeNumber, serviceCommandeDate);

                    SaveFactureServicesToDataBase(serviceFactureID);


                }
                else
                    DXMessageBox.Show("Vous n'avez choisi aucun service !", "Erreur !", MessageBoxButton.OK,
                        MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }
        }

        private void UpdateServiceFacture(int serviceFactureID)
        {
            if (_selectedServices.Rows.Count <= 0) return;
            DateTime serviceFacturedate = ServiceDate.DateTime;
            string serviceFactureType = FactureTypeCBX.Text;
            string serviceModePayment = ModePaymentCBX.Text;
            string serviceEtatPayement = EtatPaymentCBX.Text;
            float serviceTvaValue = ConverToFloat(ServiceTvaTxt.Text);
            string serviceClientFullName = ClientNameCBX.Text;
            string serviceCommandeNumber = CommandeNumber.Text;
            DateTime serviceCommandeDate = CommandeDateDte.DateTime;

            int theserviceFactureID = ServiceManager.UpdateFactureService(serviceFactureID, serviceFacturedate, serviceFactureType, serviceModePayment, serviceEtatPayement,
                serviceTvaValue, serviceClientFullName, serviceCommandeNumber, serviceCommandeDate);

            UpdateServiceFactureStoreToDataBase(theserviceFactureID);
        }

        private void UpdateServiceFactureStoreToDataBase(int theserviceFactureID)
        {

            try
            {
                var serviceManager = new ServiceManager();
                for (int i = 0; i < _selectedServices.Rows.Count; i++)
                {
                    string serviceName = SelectedServicesDataGrid.GetCellValue(i, "Name").ToString();
                    string serviceFamille = SelectedServicesDataGrid.GetCellValue(i, "Famille").ToString();
                    var servicePrice = (int)SelectedServicesDataGrid.GetCellValue(i, "Price HT");
                    var serviceQte = (int)SelectedServicesDataGrid.GetCellValue(i, "Qte");
                    var serviceTva = (float)SelectedServicesDataGrid.GetCellValue(i, "TVA");
                    string serviceDescription = SelectedServicesDataGrid.GetCellValue(i, "Description").ToString();
                    DateTime serviceDate = ServiceDate.DateTime;

                    int serviceID = ServiceManager.GetServiceID(serviceName, serviceFamille, servicePrice);
                    bool existeService = ServiceManager.IsServiceExiste(theserviceFactureID, serviceID);

                    if (existeService)
                    {
                        serviceManager.UpdateServiceFactureStore(serviceDate, theserviceFactureID, serviceID, serviceName, serviceFamille, servicePrice, serviceQte, serviceTva, serviceDescription);
                       
                    }
                    else
                    {
                        serviceManager.AttachServiceToServiceFactureStore(serviceDate, theserviceFactureID, serviceID, serviceName, serviceFamille, servicePrice, serviceQte, serviceTva, serviceDescription);
                    }
                }
                RefreshBtn_OnClick(null, null);
            }
            catch (Exception)
            {

            }
        }

        private void SaveFactureServicesToDataBase(int serviceFactureID)
        {

            try
            {
                for (int i = 0; i < _selectedServices.Rows.Count; i++)
                {
                    string serviceName = SelectedServicesDataGrid.GetCellValue(i, "Name").ToString();
                    string serviceFamille = SelectedServicesDataGrid.GetCellValue(i, "Famille").ToString();
                    var servicePrice = (int)SelectedServicesDataGrid.GetCellValue(i, "Price HT");
                    var serviceQte = (int)SelectedServicesDataGrid.GetCellValue(i, "Qte");
                    var serviceTva = (float)SelectedServicesDataGrid.GetCellValue(i, "TVA");
                    string serviceDescription = SelectedServicesDataGrid.GetCellValue(i, "Description").ToString();
                    DateTime serviceDate = ServiceDate.DateTime;
                    int serviceID = ServiceManager.GetServiceID(serviceName, serviceFamille, servicePrice);

                    ServiceManager.AttachServiceToServiceFacture(serviceDate, serviceFactureID, serviceID, serviceName, serviceFamille, servicePrice, serviceQte, serviceTva, serviceDescription);

                }
                DXMessageBox.Show(" Enregstré avec succés", "Succés", MessageBoxButton.OK,
                      MessageBoxImage.None);
                
                RefreshBtn_OnClick(null, null);
         
            }
            catch (Exception)
            {
                
            }
        }

        private void LoadServiceFactureGridControl()
        {
            
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var dataTable = new DataTable();
                dataTable.Columns.Add("Ordre", typeof(int));
                dataTable.Columns.Add("Client", typeof(string));
                dataTable.Columns.Add("Date", typeof(DateTime));
                dataTable.Columns.Add("Type", typeof(string));
                dataTable.Columns.Add("N°", typeof(int));
                DataTable allServiceFacture = ServiceManager.GetAllServiceFactures(dataTable);
                ServiceFactureGridControl.ItemsSource = allServiceFacture.DefaultView;
                ServiceFactureGridControl.Columns["Ordre"].Visible = false;
                ServiceFactureGridControl.Columns["Ordre"].ShowInColumnChooser = true;
                ServiceFactureGridControl.Columns["N°"].Visible = false;
                ServiceFactureGridControl.Columns["N°"].ShowInColumnChooser = true;
                ServiceFactureTableView.AutoWidth=false;
            }));

        }

        //*************************************************************************
        private void ServiceFactureTableView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            try
            {
                if (ServiceFactureGridControl.VisibleRowCount == 0)
                {

                    DocPreview.Model = null;
                    return;
                }
                int rowHandle = ServiceFactureTableView.FocusedRowHandle;
                if (rowHandle < 0 || rowHandle >= ServiceFactureGridControl.VisibleRowCount) return;
                var serviceFactureID = (int)ServiceFactureGridControl.GetCellValue(rowHandle, "N°");
                if (serviceFactureID >= 1)
                {
                    _serviceFactureID = serviceFactureID;
                    LoadServiceFacture(serviceFactureID);
                }
                else _serviceFactureID = -1;
            }
            catch (Exception ex)
            {
            }
        }

        private void LoadServiceFacture(int serviceFactureId)
        {
            
            var serviceManager = new ServiceManager();
            servicefacture getServicefacture = serviceManager.GetServiceFacture(serviceFactureId);
            if (getServicefacture == null) return;
            _selectedServices.Rows.Clear();
            _selectedServices = serviceManager.GetServiceFactureStore(_selectedServices, serviceFactureId);

            try 
            {
                if (getServicefacture.ServiceFactureDate != null && getServicefacture.ServiceCommandeDate != null)
                {

                    ModePaymentCBX.Text = getServicefacture.ServiceModePayment;
                    EtatPaymentCBX.Text = getServicefacture.ServiceEtatPayement;
                    CommandeNumber.Text = getServicefacture.ServiceCommandeNumber;
                    CommandeDateDte.DateTime = (DateTime)getServicefacture.ServiceCommandeDate;
                    FactureTypeCBX.Text = getServicefacture.ServiceFactureType;
                    ServiceDate.DateTime = (DateTime)getServicefacture.ServiceFactureDate;
                    ClientNameCBX.Text = getServicefacture.ServiceClientFullName;

                    SelectedServicesDataGrid.ItemsSource = null;
                    SelectedServicesDataGrid.ItemsSource = _selectedServices.DefaultView;
                    SelectedServicesTableView.BestFitColumns();
                    FocusServicesSearchColmn();
                }

            }
            catch (Exception ex)
            {
            }
        }

        private client GetClient(servicefacture getServicefacture)
        {
            try
            {
                string[] clientSplit = getServicefacture.ServiceClientFullName.Split('-');
                int clientID = Convert.ToInt32(clientSplit[0]);
                var clientManager = new ClientManager();
                return clientManager.GetClientByID(clientID);
            }
            catch (Exception)
            {
                return null;
            }

        }

        private void AllServicesTableView_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.Key.Equals(Key.Return))
                {

                    if (AllServicesGridControl.VisibleRowCount > 0)
                    {
                        int rowHandle = AllServicesTableView.FocusedRowHandle;

                        AddServiceToSelectedServicesGridControl(rowHandle);


                        AllServicesGridControl.ClearColumnFilter("ServiceColmn");
                        AllServicesGridControl.ClearColumnFilter("PrixColmn");
                        AllServicesGridControl.ClearColumnFilter("FamilleColmn");
                        AllServicesGridControl.FilterString = string.Empty;
                        FocusServicesSearchColmn();

                    }
                    else
                    {
                        string serviceName =
                            Helper.GetColmnValueFromFilter(AllServicesGridControl.GetColumnFilterString("ServiceColmn"));
                        string servicePriceHT =
                            Helper.GetColmnValueFromFilter(AllServicesGridControl.GetColumnFilterString("PrixColmn"));
                        string serviceFamille =
                            Helper.GetColmnValueFromFilter(AllServicesGridControl.GetColumnFilterString("FamilleColmn"));

                        var newServiceWND = new NewServiceWindow(serviceName, servicePriceHT, serviceFamille);

                        newServiceWND.Closed += newServiceWND_OnClosed;
                        bool? result = newServiceWND.ShowDialog();


                        if (result.HasValue)
                        {
                            if (result.Value)
                            {
                                AllServicesGridControl.FilterString = string.Empty;
                                FocusServicesSearchColmn();
                                AllServicesGridControl.FilterString = "StartsWith([ServiceColmn],'" +
                                                                    _allServices.Last().ServiceName + "')";
                            }
                            else
                            {
                                FocusServicesSearchColmn();
                                AllServicesGridControl.FilterString = string.Empty;
                            }
                        }
                        else
                        {
                            FocusServicesSearchColmn();
                            AllServicesGridControl.FilterString = string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }
        }

        private void AddServiceToSelectedServicesGridControl(int rowHandle)
        {

            try
            {

                float servicePrice = ConverToFloat(ServicePriceTxt.Text);
                string serviceDescription = ServiceDescriptioTxt.Text;
                float tva = ConverToFloat(ServiceTvaTxt.Text);
                var qte = (int)ServiceQteSpin.Value;

                var row = _selectedServices.NewRow();
                row["Ordre"] = _selectedServices.Rows.Count + 1;
                row["Name"] = AllServicesGridControl.GetCellValue(rowHandle, "ServiceColmn").ToString();
                row["Famille"] = AllServicesGridControl.GetCellValue(rowHandle, "FamilleColmn").ToString();
                row["Price HT"] = AllServicesGridControl.GetCellValue(rowHandle, "PrixColmn").ToString();
                row["Qte"] = qte;
                row["TVA"] = tva;
                row["Total TTC"] = (tva + 1) * servicePrice * qte;
                row["Description"] = serviceDescription;
                _selectedServices.Rows.Add(row);

                SelectedServicesDataGrid.ItemsSource = null;

                try
                {
                    SelectedServicesDataGrid.ItemsSource = _selectedServices.DefaultView;
                    SelectedServicesTableView.BestFitColumns();
                }
                catch (Exception e)
                {
                    DXMessageBox.Show(e.ToString());
                }

            }
            catch (Exception ex)
            {
                Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }
        }

        private void newServiceWND_OnClosed(object sender, EventArgs e)
        {

            LoadAllServices();
        }

        private void SelectedServicesTableView_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (_serviceFactureID == -1) return;
                var response = DXMessageBox.Show("Voulez vous retirer ce service?",
                   "Confirmez la passage", MessageBoxButton.YesNoCancel);
                if (response != MessageBoxResult.Yes) return;
                int rowHandle = SelectedServicesTableView.FocusedRowHandle;
                string serviceName = SelectedServicesDataGrid.GetCellValue(rowHandle, "Name").ToString();
                string serviceFamille = SelectedServicesDataGrid.GetCellValue(rowHandle, "Famille").ToString();
                var servicePrice = (int)SelectedServicesDataGrid.GetCellValue(rowHandle, "Price HT");
                int serviceID = ServiceManager.GetServiceID(serviceName, serviceFamille, servicePrice);
                string result = ServiceManager.RemoveServiceStore(_serviceFactureID, serviceID);
                DXMessageBox.Show(result);
                LoadServiceFacture(_serviceFactureID);
            }
            catch (Exception)
            {
                
            }
         
        }

      
    }
}

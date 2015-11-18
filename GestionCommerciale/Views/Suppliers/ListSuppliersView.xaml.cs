using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.Helpers;

namespace GestionCommerciale.Views.Suppliers
{
    /// <summary>
    /// Interaction logic for ListSuppliersView.xaml
    /// </summary>
    public partial class ListSuppliersView
    {
        private Provider _provider;
        private readonly SuppliersManager _supplierClient;
        private List<Provider> _providerList;

        private TabHelper TabHlp;

        public ListSuppliersView(string animationName, TabHelper hlp)
        {
            InitializeComponent();
            TabHlp = hlp;
            if (!string.IsNullOrEmpty(animationName))
            {
                Storyboard animation = (Storyboard) Application.Current.Resources[animationName];
                LayoutRoot.BeginStoryboard(animation);
            }
            _provider = new Provider();
            _supplierClient = new SuppliersManager();

        }

        private void NewSupplierBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = TabHlp.AddNewTab(typeof (AddSupplierView), "Nouveau fournisseur ", "FadeToLeftAnim");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProviderGridControl();
        }

        private void ProviderTableView_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ProviderGridControl.VisibleRowCount == 0) return;
            int rowHandle = ProviderGridControl.View.FocusedRowHandle;
            if (rowHandle < 0) return;
            SuppliersManager suppliersManager = new SuppliersManager();
            Provider provider = suppliersManager.GetProviderByName(ProviderGridControl.GetCellValue(rowHandle, "CompanyName").ToString());

            if (provider == null) return;
            LoadProviderFields(provider);
        }

        private void LoadProviderFields(Provider provider)
        {
            ImageEdit1.EditValue = provider.Photo;
            CompanyTxtBox.Text = provider.CompanyName;
            TitleContactTxtBox.Text = provider.ContactTitle;
            NameContactTxtBox.Text = provider.ContactName;
            AdresseTxtBox.Text = provider.Address;
            CountryTxtBox.Text = provider.Country;
            RegionTxtBox.Text = provider.Region;
            CityTxtBox.Text = provider.City;
            ZipCodeTxtBox.Text = provider.PostalCode;
            WebSiteTxtBox.Text = provider.HomePage;
            EmailTxtBox.Text = provider.Email;
            RcTxtBox.Text = provider.RC;
            NsTxtBox.Text = provider.NF;
            NisTxtBox.Text = provider.NIS;
            AiTxtBox.Text = provider.AI;
        }

        private void LoadProviderGridControl()
        {
            var getProviders = new BackgroundWorker();
            getProviders.DoWork += GetAllProvidersOnDoWork;
            getProviders.RunWorkerCompleted += GetAllProvidersOnRunWorkerCompleted;
            getProviders.RunWorkerAsync();

        }

        private void GetAllProvidersOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {

            var supplierClient = _supplierClient.GetTheSuppliers();
            doWorkEventArgs.Result = supplierClient;
        }

        private void GetAllProvidersOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs workerCompleted)
        {
            _providerList = workerCompleted.Result as List<Provider>;
            if (_providerList == null) return;
            ProviderGridControl.ItemsSource = (from t in _providerList
                select new
                {
                    CompanyName = t.CompanyName,
                    ContactName = t.ContactName,
                    ContactTitle = t.ContactTitle,
                    City = t.City,
                    PostalCode = t.PostalCode,
                    // Phone = t.Phone,
                    RC = t.RC
                }
                ).ToList();
        }

        private void EditSupplierBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProviderGridControl.VisibleRowCount == 0) return;

            if (
                DXMessageBox.Show(this, "Êtes-vous sûr de vouloir modifier ce fournisseur?", "Confirmation",
                    MessageBoxButton.YesNo) == MessageBoxResult.No) return;

            int rowHandle = ProviderGridControl.View.FocusedRowHandle;
            var suppliersManager = new SuppliersManager();
            var provider =
                suppliersManager.GetProviderByName(ProviderGridControl.GetCellValue(rowHandle, "CompanyName").ToString());

            if (provider == null) return;

            var image = ImageEdit1.Source as BitmapImage;
            byte[] photo = ImageToByteArray(image);

            string comanyName = CompanyTxtBox.Text;
            string contactTitle = TitleContactTxtBox.Text;
            string contactName = NameContactTxtBox.Text;
            string address = AdresseTxtBox.Text;
            string country = CountryTxtBox.Text;
            string region = RegionTxtBox.Text;
            string city = CityTxtBox.Text;
            string postalCode = ZipCodeTxtBox.Text;
            string homePage = WebSiteTxtBox.Text;
            string email = EmailTxtBox.Text;
            string rC = RcTxtBox.Text;
            string nF = NsTxtBox.Text;
            string nIs = NisTxtBox.Text;
            string aI = AiTxtBox.Text;
            string phone = TelephonTxtBox.Text;
            string fax = FaxTxtBox.Text;
            const int status = 0;

            String result = suppliersManager.UpdateProvider(provider, comanyName, contactTitle, contactName, address,
                country, region, city, postalCode, phone, homePage, email, fax, rC, nF, nIs, aI, status, photo);
            DXMessageBox.Show(this, result);
            RefreshBtn_OnClick(null, null);
        }

        private static byte[] ImageToByteArray(object bitmap)
        {
            if (bitmap == null)
                return null;
            try
            {
                var bmp = (BitmapSource) bitmap;

                int stride = bmp.PixelWidth*((bmp.Format.BitsPerPixel + 7)/8);

                var binaryData = new byte[bmp.PixelHeight*stride];

                bmp.CopyPixels(binaryData, stride, 0);

                return binaryData;
            }
            catch (Exception)
            {
                return null;
            }

        }

        private void ClearSupplierFields()
        {

            ImageEdit1.Clear();
            CompanyTxtBox.Clear();
            TitleContactTxtBox.Clear();
            NameContactTxtBox.Clear();
            AdresseTxtBox.Clear();
            CountryTxtBox.Clear();
            RegionTxtBox.Clear();
            CityTxtBox.Clear();
            ZipCodeTxtBox.Clear();
            TelephonTxtBox.Clear();
            EmailTxtBox.Clear();
            FaxTxtBox.Clear();
            RcTxtBox.Clear();
            NsTxtBox.Clear();
            NisTxtBox.Clear();
            AiTxtBox.Clear();
            WebSiteTxtBox.Clear();
        }

        private void DeleteSupplierBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProviderGridControl.VisibleRowCount == 0) return;

            if (
                DXMessageBox.Show(this, "Êtes-vous sûr de vouloir supprimer ce fournisseur?", "Confirmation",
                    MessageBoxButton.YesNo) == MessageBoxResult.No) return;

            int rowHandle = ProviderGridControl.View.FocusedRowHandle;

            SuppliersManager suppliersManager = new SuppliersManager();
            Provider provider =
                suppliersManager.GetProviderByName(ProviderGridControl.GetCellValue(rowHandle, "CompanyName").ToString());

            if (provider == null) return;
            suppliersManager.DesactivateProvider(provider);
            RefreshBtn_OnClick(null, null);
        }

        private void RefreshBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ClearSupplierFields();
            LoadProviderGridControl();

        }
    }
}

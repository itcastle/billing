using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using GestionCommerciale.DomainModel.ClassesClients;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.Helpers;

namespace GestionCommerciale.Views.Customers
{
    /// <summary>
    /// Interaction logic for ListSuppliersView.xaml
    /// </summary>
    public partial class ListCustomersView
    {
     
       
        readonly CustomersManager _customerClient;
        readonly CustomersManager _customersManager ;
        List<Customer> _customersList;

        private readonly TabHelper _tabHlp;
      
        public ListCustomersView(string animationName, TabHelper hlp)
        {
            InitializeComponent();
            _tabHlp = hlp;
            _customerClient = new CustomersManager();
            _customersManager = new CustomersManager();
            if (!string.IsNullOrEmpty(animationName))
            {
                var animation = (Storyboard)Application.Current.Resources[animationName];
                LayoutRoot.BeginStoryboard(animation);
            }
        
            

        }

        private void NewCustomerBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var item = _tabHlp.AddNewTab(typeof(AddCustomerView), "Nouveau client ", "FadeToLeftAnim");
        }

        private void UserControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadCustomersGridControl();
           
        }

        private void LoadCustomersGridControl()
        {
            var getCustomers = new BackgroundWorker();
            getCustomers.DoWork += GetAllCustomersOnDoWork;
            getCustomers.RunWorkerCompleted += GetAllCustomersOnRunWorkerCompleted;
            getCustomers.RunWorkerAsync();
          
        }

        private void GetAllCustomersOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {

            var customersList = _customerClient.GetCustomers();
            doWorkEventArgs.Result = customersList;
        }

        private void GetAllCustomersOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs workerCompleted)
        {
            _customersList = workerCompleted.Result as List<Customer>;
            if (_customersList == null) return;
            CustomersGridControl.ItemsSource = (from t in _customersList
                select new
                {
                    CompanyName = t.CompanyName,
                    ContactName = t.ContactName,
                    ContactTitle = t.ContactTitle,
                    City = t.City,
                    PostalCode = t.PostalCode,
                    Phone = t.Phone,
                    RC = t.RC
                }
                ).ToList();
        }

      
        private void EditCustomerBtn_Click(object sender, RoutedEventArgs e)
        {

            if (CustomersGridControl.VisibleRowCount == 0) return;

            if (
                DXMessageBox.Show(this, "Êtes-vous sûr de vouloir modifier  ce client?", "Confirmation",
                    MessageBoxButton.YesNo) == MessageBoxResult.No) return;

            int rowHandle = CustomersGridControl.View.FocusedRowHandle;

            string companyName = CustomersGridControl.GetCellValue(rowHandle, "CompanyName").ToString();
            Customer cust =_customersManager.GetCustomerByName(companyName);


            if (cust == null) return;

            //***************************
            ImageSource image = ImageEdit1.Source ;
            
            byte[] photo =null;

            
            //****************************

            string companName = CompanyTxtBox.Text;
            string contactTitle = TitleContactTxtBox.Text;
            string contactName = NameContactTxtBox.Text;
            string address = AdresseTxtBox.Text;
            string country = CountryTxtBox.Text;
            string region = RegionTxtBox.Text;
            string city = CityTxtBox.Text;
            string postalCode = ZipCodeTxtBox.Text;
            string phone = TelephonTxtBox.Text;
            string homePage = WebSiteTxtBox.Text;
            string email = EmailTxtBox.Text;
            string fax = FaxTxtBox.Text;
            string rC = RcTxtBox.Text;
            string nF = NsTxtBox.Text;
            string nIs = NisTxtBox.Text;
            string aI = AiTxtBox.Text;
            const int status = 0;

            String result = _customersManager.UpdateCustomer(cust, companName, contactTitle, contactName,
                address, country, region, city, postalCode, phone, homePage, email, fax, rC, nF, nIs, aI, status, photo);
            DXMessageBox.Show(this, result);

            LoadCustomersGridControl();
           RefreshBtn_Click(null,null);
        }

      
        private void CustomersDataTable_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (CustomersGridControl.VisibleRowCount == 0) return;
            int rowHandle = CustomersGridControl.View.FocusedRowHandle;
            if (rowHandle < 0) return;
            string custormerName = CustomersGridControl.GetCellValue(rowHandle, "CompanyName").ToString();
            Customer getCustomers = _customersManager.GetCustomerByName(custormerName);
            if (getCustomers == null) return;
            LoadCustomerFields(getCustomers);
        }


     
        private void LoadCustomerFields(Customer cust)
        {

           // ImageSource image = ByteToImage(cust.Photo);
            ImageEdit1.Source = null;

            //**********************
            CompanyTxtBox.Text = cust.CompanyName;
            TitleContactTxtBox.Text = cust.ContactTitle;
            NameContactTxtBox.Text = cust.ContactName;
            AdresseTxtBox.Text = cust.Address;
            CountryTxtBox.Text = cust.Country;
            RegionTxtBox.Text = cust.Region;
            CityTxtBox.Text = cust.City;
            ZipCodeTxtBox.Text = cust.PostalCode;
            TelephonTxtBox.Text = cust.Phone;
            FaxTxtBox.Text = cust.Fax;
            WebSiteTxtBox.Text = cust.HomePage;
            EmailTxtBox.Text = cust.Email;
            RcTxtBox.Text = cust.RC;
            NsTxtBox.Text = cust.NF;
            NisTxtBox.Text = cust.NIS;
            AiTxtBox.Text = cust.AI;
        }

        private BitmapImage Bitmap2BitmapImage(BitmapImage bitmap)
        {
            try
            {
                var ms = new MemoryStream();
               // bitmap(ms, ImageFormat.Png);
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                return bi;
            }
            catch (Exception)
            {
                return new BitmapImage();
            }

        }

        private static BitmapImage ByteArrayToBitmap(object bytesArray)
        {
            try
            {
                if (bytesArray == null || bytesArray.GetType() != typeof(Byte[]))
                    return null;

                var binaryData = (byte[])bytesArray;

                var bmp = new BitmapImage();

                using (var stream = new MemoryStream(binaryData))
                {
                    bmp.BeginInit();
                    bmp.StreamSource = stream;
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.EndInit();
                }

                if (bmp.CanFreeze)
                    bmp.Freeze();

                return bmp;
            }
            catch (Exception e)
            {

                return null;
            }
        }
       
        private void DeleteCustomerBtn_OnClick(object sender, RoutedEventArgs e)
        {

            if (CustomersGridControl.VisibleRowCount == 0) return;

            if (DXMessageBox.Show(this,"Êtes-vous sûr de vouloir supprimer  ce client?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No) return;

            int rowHandle = CustomersGridControl.View.FocusedRowHandle;



            
            Customer getCustomers = _customersManager.GetCustomerByName(CustomersGridControl.GetCellValue(rowHandle, "CompanyName").ToString());

            if (getCustomers == null) return;
            _customersManager.DesactivateCustomer(getCustomers);
            LoadCustomersGridControl();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            CompanyTxtBox.Clear();
            TitleContactTxtBox.Clear();
            NameContactTxtBox.Clear();
            AdresseTxtBox.Clear();
            CountryTxtBox.Clear();
            RegionTxtBox.Clear();
            CityTxtBox.Clear();
            ZipCodeTxtBox.Clear();
            TelephonTxtBox.Clear();
            WebSiteTxtBox.Clear();
            EmailTxtBox.Clear();
            FaxTxtBox.Clear();
            RcTxtBox.Clear();
            NsTxtBox.Clear();
            NisTxtBox.Clear();
            AiTxtBox.Clear();
        }

       
    }
}

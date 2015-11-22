using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.LayoutControl;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.ClassesClients;

namespace GestionCommerciale.Views.Customers
{
    /// <summary>
    /// Interaction logic for AddCustomerView.xaml
    /// </summary>
    public partial class AddCustomerView
    {
        readonly CustomersManager _customersManager;
        public AddCustomerView()
        {
            InitializeComponent();
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
        }

        public AddCustomerView(string animationName)
        {
            InitializeComponent();
            _customersManager = new CustomersManager();
            if (string.IsNullOrEmpty(animationName)) return;
            var animation = (Storyboard)Application.Current.Resources[animationName];
            LayoutRoot.BeginStoryboard(animation);
           
        }


     
      
        private void GroupBox_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var groupBox = (GroupBox)sender;
            groupBox.Background = Brushes.AliceBlue;
        }

        private void GroupBox_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var groupBox = (GroupBox)sender;
            groupBox.Background = null;
        }



        private void AddTheCustomerBtn_OnClick(object sender, RoutedEventArgs e)
        {
          


            //***************************
            var image = imageEdit1.Source as BitmapImage;
            byte[] photo = ImageToByteArray(image);


            //****************************
         
          
          
            string companName = CompanyTxtBox.Text;
            string contactTitle = TitleContactTxtBox.Text;
            string contactName = NameContactTxtBox.Text;
            string address = AdresseTxtBox.Text;
            string country = CountryTxtBox.Text;
            string region = RegionTxtBox.Text;
            string city = CityTxtBox.Text;
            string postalCode= ZipCodeTxtBox.Text;
            string phone = TelephonTxtBox.Text;
            string homePage = WebSiteTxtBox.Text;
            string email = EmailTxtBox.Text;
            string fax = FaxTxtBox.Text;
            string rC = RcTxtBox.Text;
            string nF = NsTxtBox.Text;
            string nIs = NisTxtBox.Text; 
            string aI = AiTxtBox.Text;
            const int status = 0;

            String result = _customersManager.AddNewCustomer(companName, contactTitle, contactName, address,
                country, region, city, postalCode, phone, homePage, email, fax, rC, nF, nIs, aI, status,photo);
            DXMessageBox.Show(this, result);
            
            ClearFields();

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

        private void ClearFields()
        {

            imageEdit1.Clear();
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

        private void UserControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (SettingsClient.IsNotLegal()) Application.Current.Shutdown();
        }

    }
}

using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.LayoutControl;
using GestionCommerciale.DomainModel.ClassesClients;

namespace GestionCommerciale.Views.Suppliers
{
    /// <summary>
    /// Interaction logic for AddSupplierView.xaml
    /// </summary>
    public partial class AddSupplierView
    {
        
        public AddSupplierView()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
        }
        public AddSupplierView(string animationName)
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            if (string.IsNullOrEmpty(animationName)) return;
            Storyboard animation = (Storyboard)Application.Current.Resources[animationName];
            LayoutRoot.BeginStoryboard(animation);
        }
        
        private void GroupBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

           GroupBox groupBox = (GroupBox)sender;
            groupBox.Background = Brushes.AliceBlue;

            
            
        }

        private void GroupBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            try
            {
                GroupBox groupBox = (GroupBox)sender;
                groupBox.Background = null;
            }
            catch (Exception exception)
            {
                //

            }

        }
        
        private void AddTheSupplierBtn_Click(object sender, RoutedEventArgs e)
        {
         
            var image = imageEdit1.Source as BitmapImage;
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
            SuppliersManager sc = new SuppliersManager();
            String result = sc.AddNewProvider(comanyName, contactTitle, contactName, address,
                country, region, city, postalCode, phone, homePage, email, fax, rC, nF, nIs, aI, status, photo);
            DXMessageBox.Show(this,result);
            ClearSupplierFields();

            
        }

        private void ClearSupplierFields()
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

        private static byte[] ImageToByteArray(object bitmap)
        {
            if (bitmap == null)
                return null;
            try
            {
                var bmp = (BitmapSource)bitmap;

                int stride = bmp.PixelWidth * ((bmp.Format.BitsPerPixel + 7) / 8);

                var binaryData = new byte[bmp.PixelHeight * stride];

                bmp.CopyPixels(binaryData, stride, 0);

                return binaryData;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}

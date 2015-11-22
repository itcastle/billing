using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Editors;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.ClassesClients;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.DomainModel.Validator;

namespace GestionCommerciale.Views.Options
{
    /// <summary>
    /// Interaction logic for GeneralParams.xaml
    /// </summary>
    public partial class GeneralParams
    {
        bool _passChanged = false;   
        public GeneralParams()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (! WhoEmployee.IsAdministrator)
                this.Visibility = Visibility.Hidden;
           
            this.LoadSettingsIntoForm();


        }
        private void LoadSettingsIntoForm()
        {
            SettingsClient sc = new SettingsClient();
            Setting settings = sc.GetSetting();
            if (settings == null) return;
            
                settings.DateMAJ = DateTime.Now;
                PictureImg.EditValue = settings.Logo;
                CompanyNameTxtBox.Text = settings.CompanyName;
                AdresseTxtBox.Text = settings.Adresse;
                ZipCodeTxtBox.Text = settings.PostalCode;
                CityTxtBox.Text = settings.City;
                FixTxtBox.Text = settings.Phone;
                MobileTxtBox.Text = settings.MobPhone;
                FaxTxtBox.Text = settings.Fax;
                EmailTxtBox.Text = settings.Email;
                RcTxtBox.Text = settings.RC;
                NsTxtBox.Text = settings.NF;
                NisTxtBox.Text = settings.NIS;
                AiTxtBox.Text = settings.AI;
                PasswordTxtBox.Text = "administrat";
        }

        private void initSettings()
        {
            MessageBox.Show("Vous Devez Remplir les paramètres pour un bon fonctionnement de l'application");
        }

        




        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Êtes-vous sûr de vouloir d'enregistrer ces paramètres?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No) return;

            SettingsClient sc = new SettingsClient();
            Setting settings = sc.GetSetting();

            String photo_path = null;

            if (PictureImg.Source != null && PictureImg.Source is BitmapImage)
            {
                BitmapImage bi = (BitmapImage)PictureImg.Source;
                FileStream stream = bi.StreamSource as FileStream;
                if (stream != null)
                {
                    photo_path = stream.Name;
                    settings.Logo = Validator.ConvertImageToByteArray(photo_path);
                }
            }

            String input = null;
            input = CompanyNameTxtBox.Text;
            if (!String.IsNullOrEmpty(input))
                settings.CompanyName = input;
            else { MessageBox.Show("Erreur dans le Nom de l'entreprise"); return; }

            settings.Adresse = AdresseTxtBox.Text;
            settings.PostalCode = ZipCodeTxtBox.Text;
            settings.City = CityTxtBox.Text;
            settings.Phone = FixTxtBox.Text;
            settings.MobPhone  = MobileTxtBox.Text;
            settings.Fax = FaxTxtBox.Text;
            settings.Email  = EmailTxtBox.Text;
            settings.RC = RcTxtBox.Text;
            settings.NF = NsTxtBox.Text;
            settings.NIS = NisTxtBox.Text;
            settings.AI = AiTxtBox.Text;
            if (!String.IsNullOrEmpty(PasswordTxtBox.Text)) {
                PasswordClient pc = new PasswordClient();
                Password admin = pc.GetPassword("admin");
                if (!PasswordTxtBox.Text.Equals("administrat")) admin.Password1 = PasswordClient.CryptePassword(PasswordTxtBox.Text);
                admin.Status = 2;
                pc.MajUser(admin);
                admin = pc.GetPassword("admin");
            }
            
            String s = SettingsClient.MajSettings(settings);
            MessageBox.Show(s);             
        }

        private void PictureImg_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            
        }
    }
}

using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using GestionCommerciale.DomainModel.Entities;
using DevExpress.Xpf.Core;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.ClassesClients;
using GestionCommerciale.DomainModel.Validator;

namespace GestionCommerciale
{
    /// <summary>
    /// Logique d'interaction pour Licence.xaml
    /// </summary>
    public partial class Licence : Window
    {
        public Licence()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            
           String cleClient = HhdMacSettings.GetHddSerialNumber("") ;

           cleClient = CryptePassword(cleClient);
           cleClient = cleClient + "ItCastle";

            String licence = CryptePassword(cleClient);

            SettingsClient sc = new SettingsClient();
            Setting settings = sc.GetSetting();
            
            if (! (licence.Equals(textBox1.Text) || textBox1.Text.Equals("demo") )) {
                DXMessageBox.Show("Votre clé n'est pas valide (revérifier)");
                Close();
            }

            if (settings.Licence.Equals("demo")&&(textBox1.Text.Equals("demo"))) {
                DXMessageBox.Show("Vous avez déjà utilisez la version demo");
                Close();
            }

            if (textBox1.Text.Equals("demo") )
            {
                DXMessageBox.Show("Votre Licence demo est valide seulement pour 30 jours ou 20 commandes");
                licence = "demo";
            }
           
            settings.Licence = textBox1.Text;
            if (settings.FirstTime == true) settings.DateFirstTime = DateTime.Now;

            settings.FirstTime = false;
            SettingsClient.MajSettings(settings);

            if ((licence.Equals(textBox1.Text)) && ! settings.Licence.Equals("demo")) DXMessageBox.Show("Votre clé a été enregistrer avec succès");
            this.Close();
        }

        private static string CryptePassword(string password)
        {

            MD5 md5Hasher = MD5.Create();

            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(password));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            String cleClient = "";
            try
            {
                cleClient = HhdMacSettings.GetHddSerialNumber("");
            }
            catch (Exception)
            {
                DXMessageBox.Show("Une erreur grave c'est produite, Contacté l'équipe technique");
            }

            textBox2.Text = CryptePassword(cleClient);
        }
    }
}

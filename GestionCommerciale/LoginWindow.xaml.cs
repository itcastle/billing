using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.Validator;
using System.Security.Cryptography;
using GestionCommerciale.DomainModel.ClassesClients;


namespace GestionCommerciale
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow
    {
      

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginBtn_OnClick(object sender, RoutedEventArgs e)
        {
           // PasswordClient pc = new PasswordClient();
           // Password password = new Password();

           // String username = UserNameTxtBox.Text;
            //if (Validator.IsLoginValid(username) && !String.IsNullOrEmpty(username))
            //    password.Login = username;
            //else { MessageBox.Show("Erreur dans le Login ou le password"); return; }

           // String pass = PasswordPswdBox.Password;
            //if (!String.IsNullOrEmpty(pass))   //ajouter Validator.IsPassWordValid(pass) && pour plus de regles apropos des mots de passe
            //    password.PassWord1 = PasswordClient.CryptePassword(pass);
            //else { MessageBox.Show("Erreur dans le Login ou le password"); return; }

         
            //bool existe = pc.IsPassWordValid(password);
            //if (existe)
            //{
            //    EmployeesClient empc = new EmployeesClient();
               // WhoEmployee.employee = empc.getEmployeeByUserID(password);

                //if (pc.getStatus(password) == 2)
                //{
                    WhoEmployee.IsAdministrator = true;
                    var mw = new MainWindow();
                    mw.Show();
                    Close();
            //}

                //else
                //{
                //    WhoEmployee.isAdministrator = false;
                //    MainWindow mw = new MainWindow();
                //    mw.AddEmployeeNavBtn.IsEnabled = false;
                //    mw.ListEmployeesNavBtn.IsEnabled = false;
                //    mw.Show();
                //    Close();
                //}
           // }

            //if (tentative == 3)
            //{
            //    MessageBox.Show("user name ou password incorrect cété ta dernierer tentative vérifier bien votre user & mot de passe la prochaine fois");
            //    Close();
            //}

            //if (tentative < 3)
            //{
            //    MessageBox.Show(String.Format("user name ou password incorrect il vous reste {0} tentative", 3 - tentative));
            //    UserNameTxtBox.Clear();
            //    PasswordPswdBox.Clear();
            //}
        }


        private void CloseBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void window_OnLoaded(object sender, RoutedEventArgs e)
        {
            //var sc = new SettingsClient();
            //Settings s = sc.getSetting();
            //if (s == null) SettingsClient.initSettings();
            
            //String cleClient = HHD_MAC_Settings.GetHDDSerialNumber("");

            //cleClient = CryptePassword(cleClient) + "ItCastle";

            //String Licence = CryptePassword(cleClient);

            //if (IsDemo())
            //{
            //    String ss = "Vous utilisez une version demo de 30 jours et max 20 commandes";
            //    ss = ss + "\n  il vous reste " + calculeTemps() + " jours";
            //    ss = ss + " ou " + calculeOrdre() + " commandes";
            //    MessageBox.Show(ss);
            //}
            //else if (!Licence.Equals(s.Licence) && ! IsDemo())
            //{
            //    MessageBox.Show("Vous devez avoir une clé valide pour utiliser ce logiciel");
            //    var licence = new Licence();
            //    licence.Show();
            //    this.Close();
            //    return;
            //}

            UserNameTxtBox.Focus();
        }

        private bool IsDemo()
        {
            try
            {
                var sc = new SettingsClient();
                var s = sc.GetSetting();

                if (!s.Licence.Equals("demo")) return false;
                if (calculeOrdre() < 0) return false;
                if (calculeTemps() < 0 || calculeTemps() > 30) return false;
                return true;
            }
            catch (Exception e)
            {

                return true;
            }
        }

        private int calculeTemps()
        {
            var sc = new SettingsClient();
            Setting s = sc.GetSetting();
            var debut = (DateTime)s.DateFirstTime;
            DateTime fin = DateTime.Now;
            TimeSpan difference = fin - debut;
            return 30 - difference.Days;
        }

        private int calculeOrdre()
        {
            var oc = new OrderClient();
            return 20 - oc.GetCountOrder();
        }

        private void RootLayout_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginBtn_OnClick(null, null);
            }
        }

        private string CryptePassword(string password)
        {

            MD5 md5Hasher = MD5.Create();

            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(password));

            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}

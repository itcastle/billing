using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.ClassesClients;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.DomainModel.Validator;

namespace GestionCommerciale.Views.Employees
{
    /// <summary>
    /// Interaction logic for AddEmployee.xaml
    /// </summary>
    public partial class AddEmployeeView
    {
        public AddEmployeeView()
        {
            InitializeComponent();
        }
        public AddEmployeeView(string animationName)
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(animationName))
            {
                Storyboard animation = (Storyboard)Application.Current.Resources[animationName];
                LayoutRoot.BeginStoryboard(animation);
            }
        }

        

        #region GroupBox_focus_event

        private void GroupBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            DevExpress.Xpf.LayoutControl.GroupBox groupBox = (DevExpress.Xpf.LayoutControl.GroupBox)sender;
            groupBox.Background = Brushes.AliceBlue;
        }

        private void GroupBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            DevExpress.Xpf.LayoutControl.GroupBox groupBox = (DevExpress.Xpf.LayoutControl.GroupBox)sender;
            groupBox.Background = null;
        }
        #endregion

        private void AddEmployeeBtn_Click(object sender, RoutedEventArgs e)
        {
            Employee emp = new Employee();
            EmployeesManager empc = new EmployeesManager();

            String photoPath = null;
            if (imageEdit1.Source != null && imageEdit1.Source is BitmapImage)
            {
                BitmapImage bi = (BitmapImage)imageEdit1.Source;
                FileStream stream = bi.StreamSource as FileStream;
               
                photoPath = stream.Name;
                emp.Photo = Validator.ConvertImageToByteArray(photoPath);
               
            }
            emp.PhotoPath = photoPath;

            String input = null;
            input = LastNameTxtBox.Text;
            if ( !String.IsNullOrEmpty(input))
                emp.EmployeeLastname = input;
            else { MessageBox.Show("Erreur dans le Nom de l'employée"); return; }


            input = FirstNameTxtBox.Text;
            if ( !String.IsNullOrEmpty(input))
                emp.EmployeeFirstname = input;
            else { MessageBox.Show("Erreur dans le Prenom de l'employée"); return; }


            input = TitleTxtBox.Text;
            if ( !String.IsNullOrEmpty(input))
                emp.Title = input;
            else { MessageBox.Show("Erreur dans le Titre de l'employée"); return; }


            DateTime date = BirthDayTxtBox.DateTime.Date;
            if (!String.IsNullOrEmpty(date.ToString()))
                emp.BirthDate = date;
            else { MessageBox.Show("Erreur dans la Date de naissance"); return; }

            // a savoir si un employée est déjà engagé il ya longtemps ????       
            emp.HireDate = DateTime.Now;

            emp.Address = AdresseTxtBox.Text;
            emp.Country = CountryTxtBox.Text;
            emp.City = CityTxtBox.Text;
            emp.Region = RegionTxtBox.Text;


            input = ZipCodeTxtBox.Text;
            if (Validator.IsPostalCodeValid(input) || String.IsNullOrEmpty(input))
                emp.PostalCode = input;
            else { MessageBox.Show("Erreur dans le code postal"); return; }

            input = MobileTxtBox.Text;
         //   if (Validator.IsMobPhoneValid(input) || String.IsNullOrEmpty(input))
                emp.Phone1 = input;
         //   else { MessageBox.Show("Erreur dans le Telephone mobile (10 chiffres)"); return; }


            input = FixTxtBox.Text;
            if (Validator.IsFixPhoneValid(input) || String.IsNullOrEmpty(input))
                emp.HomePhone = input;
            else { MessageBox.Show("Erreur dans le Telephone Fix (09 chiffres)"); return; }

            input = EmailTxtBox.Text;
            if (Validator.IsEmailValid(input) || String.IsNullOrEmpty(input))
                emp.EmailAdress = input;
            else { MessageBox.Show("Erreur dans l'Email "); return; }
            

            ////      emp.ReportsTo   =   ;

            var textRange = new TextRange(NotesRichTxtArea.Document.ContentStart, NotesRichTxtArea.Document.ContentEnd);
            var note = textRange.Text;
            emp.Notes = note;

            emp.HireDate = dateEdit2.DateTime;
            
            Password pass = new Password();
            
            input = textEdit1.Text;
            if (!String.IsNullOrEmpty(input))
                pass.Login = input;
            else { MessageBox.Show("Erreur dans le login "); return; }

            input = textEdit2.Text;
            if (!String.IsNullOrEmpty(input))
                pass.Password1 = PasswordClient.CryptePassword(input);
            else { MessageBox.Show("Erreur dans le mot de passe "); return; }

            PasswordClient pc = new PasswordClient();
            pc.AddUser(pass);
            pc.ActivatUser(pass);

            emp.Password = pass;

            emp.Status = 0;
            String s = empc.AddEmployee(emp);
            MessageBox.Show(s);

            imageEdit1.Clear();
            LastNameTxtBox.Clear();
            FirstNameTxtBox.Clear();
            TitleTxtBox.Clear();
            BirthDayTxtBox.Clear();
            AdresseTxtBox.Clear();
            CountryTxtBox.Clear();
            CityTxtBox.Clear();
            RegionTxtBox.Clear();
            ZipCodeTxtBox.Clear();
            MobileTxtBox.Clear();
            FixTxtBox.Clear();
            EmailTxtBox.Clear();
            dateEdit2.Clear();
            NotesRichTxtArea.Document.Blocks.Clear();
            textEdit1.Clear();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (SettingsClient.IsNotLegal()) App.Current.Shutdown();
        }

        private void imageEdit1_Validate(object sender, DevExpress.Xpf.Editors.ValidationEventArgs e)
        {
            if (!(e.Value is BitmapImage)) return;
            BitmapImage img = (BitmapImage)e.Value;
            FileStream str = (FileStream)img.StreamSource;
            MessageBox.Show(str.Name);
        }

    

    }
}

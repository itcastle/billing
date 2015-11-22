using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.ClassesClients;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.DomainModel.Validator;
using GestionCommerciale.Helpers;

namespace GestionCommerciale.Views.Employees
{
    /// <summary>
    /// Interaction logic for ListSuppliersView.xaml
    /// </summary>
    public partial class ListEmployeesView
    {
        Employee employee;
        EmployeesManager employeeClient;
        List<Employee> employeeList;

        private TabHelper TabHlp;
        public ListEmployeesView(string animationName, TabHelper hlp)
        {
            InitializeComponent();
            TabHlp = hlp;
            employee = new Employee();
            employeeClient = new EmployeesManager();
            if (!string.IsNullOrEmpty(animationName))
            {
                Storyboard animation = (Storyboard)Application.Current.Resources[animationName];
                LayoutRoot.BeginStoryboard(animation);
            }
           
        }

        private void NewEmployeeBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = TabHlp.AddNewTab(typeof(AddEmployeeView), "Nouveau employée ", "FadeToLeftAnim");

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loadGrid();
        }

        private void loadGrid()
        {
            employeeList = employeeClient.GetEmployees();
            if (employeeList!=null)
            EmployeesDataGrid.ItemsSource = (from t in employeeList
                                             select new
                                             {
                                                 employee_firstname = t.EmployeeFirstname,
                                                 employee_lastname = t.EmployeeLastname,
                                                 BirthDate = t.BirthDate,
                                                 City = t.City,
                                                 PostalCode = t.PostalCode,
                                                 employee_phone = t.Phone1
                                             }
                                             ).ToList();
        }

        private void EditEmployeeBtn_Click(object sender, RoutedEventArgs e)
        {
            employeeClient = new EmployeesManager();

            if (EmployeesDataGrid.VisibleRowCount == 0) return;

            if (MessageBox.Show("Êtes-vous sûr de vouloir modifier  cet employer?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No) return;

            int rowHandle = EmployeesDataGrid.View.FocusedRowHandle;

            EmployeesManager empc = new EmployeesManager();
            Employee emp = empc.GetEmployeeByName(EmployeesDataGrid.GetCellValue(rowHandle, "employee_firstname").ToString());

            if (emp == null) return;

            String photo_path = null;

            if (imageEdit1.Source != null && imageEdit1.Source is BitmapImage)
            {
                BitmapImage bi = (BitmapImage)imageEdit1.Source;
                FileStream stream = bi.StreamSource as FileStream;
                if (stream != null)
                {
                    photo_path = stream.Name;
                    emp.Photo = Validator.ConvertImageToByteArray(photo_path);
                }
            }
            emp.PhotoPath = photo_path;

            String input = null;
            input = LastNameTxtBox.Text;
            if (input == null) input = "";
            input = input.Trim();
            if (Validator.IsAlphabeticValid(input) && !String.IsNullOrEmpty(input))
                emp.EmployeeLastname = input;
            else { MessageBox.Show("Erreur dans le Nom de l'employée"); return; }


            input = FirstNameTxtBox.Text;
            if (input == null) input = "";
            input = input.Trim();
            if (Validator.IsAlphabeticValid(input) && !String.IsNullOrEmpty(input))
                emp.EmployeeFirstname = input;
            else { MessageBox.Show("Erreur dans le Prenom de l'employée"); return; }


            input = TitleTxtBox.Text;
            if (input == null) input = "";
            input = input.Trim();
            if (Validator.IsAlphabeticValid(input) || String.IsNullOrEmpty(input))
                emp.Title = input;
            else { MessageBox.Show("Erreur dans le Titre de l'employée"); return; }

            if (BirthDayTxtBox!=null)
            emp.BirthDate = BirthDayTxtBox.DateTime.Date;

            
            // a savoir si un employée est déjà engagé il ya longtemps ????       
            emp.HireDate = DateTime.Now;

            emp.Address = AdresseTxtBox.Text;
            emp.Country = CountryTxtBox.Text;
            emp.City = CityTxtBox.Text;
            emp.Region = RegionTxtBox.Text;


            input = ZipCodeTxtBox.Text;
            if (input == null) input = "";
            input = input.Trim();
            if (Validator.IsPostalCodeValid(input) || String.IsNullOrEmpty(input))
                emp.PostalCode = input;
            else { MessageBox.Show("Erreur dans le code postal"); return; }

            input = MobileTxtBox.Text;
            if (input == null) input = "";
            input = input.Trim();
        //    if (Validator.IsMobPhoneValid(input) || String.IsNullOrEmpty(input))
                emp.Phone1 = input;
        //    else { MessageBox.Show("Erreur dans le Telephone mobile (10 chiffres)"); return; }


            input = FixTxtBox.Text;
            if (input == null) input = "";
            input = input.Trim();
            if (Validator.IsFixPhoneValid(input) || String.IsNullOrEmpty(input))
                emp.HomePhone = input;
            else { MessageBox.Show("Erreur dans le Telephone Fix (09 chiffres)"); return; }

            input = EmailTxtBox.Text;
            if (input == null) input = "";
            input = input.Trim();
            if (Validator.IsEmailValid(input) || String.IsNullOrEmpty(input))
                emp.EmailAdress = input;
            else { MessageBox.Show("Erreur dans l'Email "); return; }


            ////      emp.ReportsTo   =   ;

            var textRange = new TextRange(NotesRichTxtArea.Document.ContentStart, NotesRichTxtArea.Document.ContentEnd);
            var note = textRange.Text;
            emp.Notes = note;

            emp.HireDate = dateEdit2.DateTime;

            input = textEdit2.Text;
            if (!input.Equals("Password"))
                emp.Password.Password1 = PasswordClient.CryptePassword(input);
        
            emp.Status = 0;
             
            String s = employeeClient.MajEmployee(emp);
            MessageBox.Show(s);

            loadGrid();

            EmployeesDataGrid.View.FocusedRowHandle = rowHandle;

        }

        private void EmployeesDataTable_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        {
            if (EmployeesDataGrid.VisibleRowCount == 0) return;
            int rowHandle = EmployeesDataGrid.View.FocusedRowHandle;
            if (rowHandle < 0) return;
            EmployeesManager empc = new EmployeesManager();
            Employee emp = empc.GetEmployeeByName(EmployeesDataGrid.GetCellValue(rowHandle, "employee_firstname").ToString());


            if (emp == null) return;
            Afficher(emp);
        }

        private void Afficher(Employee emp)
        {
            imageEdit1.EditValue = emp.Photo;
            LastNameTxtBox.Text = emp.EmployeeLastname;
            FirstNameTxtBox.Text = emp.EmployeeFirstname;
            TitleTxtBox.Text = emp.Title;
            try
            {
                BirthDayTxtBox.DateTime = emp.BirthDate.Value;
                dateEdit2.DateTime = emp.HireDate.Value;
            }
            catch (Exception)
            {
              
            }
            
            AdresseTxtBox.Text = emp.Address;
            CountryTxtBox.Text = emp.Country;
            CityTxtBox.Text = emp.City;
            RegionTxtBox.Text = emp.Region;
            ZipCodeTxtBox.Text = emp.PostalCode;

            MobileTxtBox.Text = emp.Phone1;
            FixTxtBox.Text = emp.HomePhone;
            EmailTxtBox.Text = emp.EmailAdress;
            textEdit1.Text = emp.Password.Login;
            textEdit2.Text = "Password";
         //   textEdit16.Text = emp.Employees2Reference.Value.employee_firstname;


            NotesRichTxtArea.Document.Blocks.Clear();
            NotesRichTxtArea.AppendText(emp.Notes);
                     
        }

        private void DeleteEmployeeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesDataGrid.VisibleRowCount == 0) return;

            if (MessageBox.Show("Êtes-vous sûr de vouloir supprimer  cet employer?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            int rowHandle = EmployeesDataGrid.View.FocusedRowHandle;
            
            EmployeesManager empc = new EmployeesManager();
            Employee emp = empc.GetEmployeeByName(EmployeesDataGrid.GetCellValue(rowHandle, "employee_firstname").ToString());

            if (emp == null) return;
            empc.DesactivateEmployee(emp);
            loadGrid();
        }

        
    }
}

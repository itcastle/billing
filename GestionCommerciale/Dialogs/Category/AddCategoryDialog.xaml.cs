using System.Windows;
using DevExpress.Xpf.Core;
using GestionCommerciale.DomainModel;

namespace GestionCommerciale.Dialogs.Category
{
    /// <summary>
    /// Interaction logic for AddCategoryModal.xaml
    /// </summary>
    public partial class AddCategoryDialog : Window
    {
        public AddCategoryDialog()
        {
            InitializeComponent();
            ThemeManager.SetTheme(this, HelpFunctions.AppTheme);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddCategory_Click(object sender, RoutedEventArgs e)
        {
             CategorysClient cc=new CategorysClient();
             GestionCommerciale.DomainModel.Entities.Category cat=new GestionCommerciale.DomainModel.Entities.Category {CategoryName=CategoryTxtBox.Text,Description=DescriptionTxtBox.Text};
            cc.AddCategory(cat);
            DialogResult=true;
            Close();
        }
    }
}

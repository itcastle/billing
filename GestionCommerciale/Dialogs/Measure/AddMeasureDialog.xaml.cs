using System.Windows;
using GestionCommerciale.DomainModel.Entities;
using DevExpress.Xpf.Core;
using GestionCommerciale.DomainModel;
using GestionCommerciale.Helpers;

namespace GestionCommerciale.Dialogs.Measure
{
    /// <summary>
    /// Interaction logic for AddMeasureDialog.xaml
    /// </summary>
    public partial class AddMeasureDialog : Window
    {
        public AddMeasureDialog()
        {
            InitializeComponent();
            ThemeManager.SetTheme(this, HelpFunctions.AppTheme);   
        }
        private void btnAddMeasure_Click(object sender, RoutedEventArgs e)
        {
            CategorysManager mc=new  CategorysManager();
            ProductMeasure m = new ProductMeasure
            {
                ProMeasureType = MeasureTxtBox.Text,
                ProMeasureUnit = DescriptionTxtBox.Text
            };
            mc.AddMeasure(m);
           // CategorysManager.NewestMeasure = m;
            DialogResult = true;
            Close();
        }   
    }
}

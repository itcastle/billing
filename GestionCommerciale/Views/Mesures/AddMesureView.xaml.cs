using System;
using System.Windows;
using System.Windows.Documents;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.Entities;
using DevExpress.Xpf.Core;

namespace GestionCommerciale.Views.MesureFolder
{
    /// <summary>
    /// Interaction logic for AddMesureView.xaml
    /// </summary>
    public partial class AddMesureView
    {
        public AddMesureView()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            ProductMeasure measures = new ProductMeasure();

            CategorysManager CategorysManager = new CategorysManager();

            String input = null;

            input = TextEdit1.Text ?? "";

            if ( /*!Validator.IsAlphanumericValid(input) || */ String.IsNullOrEmpty(input))
            {
                DXMessageBox.Show(this,"Erreur dans le Nom de la Mesure (ne doit pas être vide)");
                return;
            }

            if (CategorysManager.IsMeasureExist(input))
            {
                DXMessageBox.Show(this, "Mesure existe déjà");
                return;
            }
            

            measures.ProMeasureType = input;

            String description =
                new TextRange(RichTextBox1.Document.ContentStart, RichTextBox1.Document.ContentEnd).Text;

            measures.ProMeasureUnit = description;

            String result = CategorysManager.AddMeasure(measures);

            DXMessageBox.Show(result);
            Close();
        }
    }
}

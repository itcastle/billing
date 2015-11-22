using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Grid;
using DevExpress.XtraRichEdit;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.Helpers;

namespace GestionCommerciale.Views.Purchases
{
    /// <summary>
    /// Interaction logic for ListCommandsView.xaml
    /// </summary>
    public partial class ListCommandsView
    {
        private readonly TabHelper _tabHlp;

        public ListCommandsView(string animationName, TabHelper hlp)
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(animationName)) return;
            var animation = (Storyboard)Application.Current.Resources[animationName];
            LayoutRoot.BeginStoryboard(animation);
            _tabHlp = hlp;

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GcdbEntities db = new GcdbEntities();
            TheDataGrid.ItemsSource = db.Commands.ToList();
            Command cmd = new Command();
        }

        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            DocRichEdit.biFilePrint.PerformClick();
        }

        private void TheTableView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (e.NewRow != null)
            {
                Command selectedCommand = e.NewRow as Command;
                if (selectedCommand == null) return;
                if (selectedCommand.Document != null)
                {
                    byte[] decompressed = Helper.Decompress(selectedCommand.Document.DocFile);
                    Stream stream = new MemoryStream(decompressed);
                    DocRichEdit.DocRichEdit.LoadDocument(stream, DocumentFormat.OpenXml);
                }
                else DocRichEdit.DocRichEdit.CreateNewDocument();
            }
            else DocRichEdit.DocRichEdit.CreateNewDocument();

        }

        private void PurchaseBtn_Click(object sender, RoutedEventArgs e)
        {
            Command selectedCommand = TheDataGrid.SelectedItem as Command;
            if (selectedCommand != null)
            {
                var item = _tabHlp.AddNewTab(typeof(AddPurchaseView), "Effectuer un achat ", "FadeToLeftAnim", _tabHlp,selectedCommand.Purchase);
            }
        }
    }
}

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GestionCommerciale.DomainModel.Entities;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.LayoutControl;
using DevExpress.Xpf.Printing;
using GestionCommerciale.Modals;
using GestionCommerciale.Modals.TVA;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.Validator;

namespace GestionCommerciale.Views.InvoiceFolder
{

    /// <summary>
    /// Interaction logic for AddCustomerView.xaml
    /// </summary>    
    public partial class AddFactureView
    {

        Decimal _total;
        Decimal _ttc;
        Decimal _discount;
        Decimal _tva;
        Decimal _timbre;

        readonly TvaClient _tvaClient = new TvaClient();
        readonly OrderClient _orderClient = new OrderClient();

        public AddFactureView(string typeFacture)
        {
            InitializeComponent();
        }

        public AddFactureView(string animationName, string typeFacture)
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(animationName)) return;
            var animation = (Storyboard)Application.Current.Resources[animationName];
            LayoutRoot.BeginStoryboard(animation);
        }


     

        private void GroupBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            GroupBox groupBox = (GroupBox)sender;
            groupBox.Background = Brushes.AliceBlue;
        }

        private void GroupBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            GroupBox groupBox = (GroupBox)sender;
            groupBox.Background = null;
        }

     

        private void TVABtn_Click(object sender, RoutedEventArgs e)
        {
            var tvaDialog = new TVADialog();
            tvaDialog.ShowDialog();
            if (tvaDialog.DialogResult == true)
            { }
        }

        private void CustomersBtn_Click(object sender, RoutedEventArgs e)
        {
            var customerDialog = new CustomerDialog();
            customerDialog.ShowDialog();
        }

        private void SaleSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            var saleDialog = new SaleDialog();
            saleDialog.ShowDialog();
        }

        private void AddFactureBtn_Click(object sender, RoutedEventArgs e)
        {
            // a remplir benamar
        }
        private Decimal GetTva()
        {
            try
            {
                return Convert.ToDecimal(CbTva.Text);
            }
            catch (Exception)
            {
                //
            }
            return Convert.ToDecimal(0.17);//s
        }

        private void FactureControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (SettingsClient.IsNotLegal()) Application.Current.Shutdown();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CbTva.ItemsSource = _tvaClient.GetTvaValues();
        
                if (CbTva.SelectedIndex < 0) CbTva.SelectedIndex = 0;
                if (CbPayment.SelectedIndex < 0) CbPayment.SelectedIndex = 0;

                LoadGridFact();
            }));
        }

        private void LoadGridFact()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var listeOrdersNonFactured = _orderClient.GetOrderFactured();
                if (listeOrdersNonFactured != null)
                    DgFactures.ItemsSource = (from t in listeOrdersNonFactured

                                              select new
                                              {
                                                  OrderID = t.OrderID,
                                                  OrderDate = t.OrderDate,
                                                  RequiredDate = t.CommandeDate,
                                                  CompanyName = t.Customer.CompanyName,
                                                  Description = t.Description,
                                              }
                                                                 ).ToList();
            }));
        
           

        }

        private void Afficher()
        {
            TotalHtTxtBox.Text = _total.ToString("###,###.00");
            //TotalAfterDiscountTxtBox.Text = discount.ToString("###,###.00");
            TimbreTxtBox.Text = _timbre.ToString("###,###.00");
            TotalTxtBox.Text = _ttc.ToString("###,###.00");
        }

        private void Calculate(Order ordre)
        {
            _total = 0;
            _ttc = 0;
            _discount = 0;
            _tva = 0;
            var orderDetailsClient = new OrderDetailsClient();
            var lisstOrderDetailses = orderDetailsClient.GetOrderDetailsByOrder(ordre);
            foreach (OrderDetail entity in lisstOrderDetailses)
            {
                _total += entity.UnitPrice * entity.Quantity;// a refaire en cas de comande distribuée
            }
            _tva = GetTva();
            _discount = getDiscount() * _total;

            _timbre = _total / 100;
            if (_timbre > 2500) _timbre = 2500;
            if (!CbPayment.Text.Equals("Espèce")) _timbre = 0;

            _ttc = _total - _discount + (_total * _tva) + _timbre;
        }

        private decimal getDiscount()
        {
            // if (!String.IsNullOrEmpty(DiscountTxtBox.Text)) return Convert.ToDecimal(DiscountTxtBox.Text) / 100;
            return 0;
        }

        private void cbSecurityNum_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {

        }
       

        private void cbCustomerAddress_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {

        }

       

        
        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {

            if (DgFactures.VisibleRowCount == 0) return;
            int rowHandle = DgFactures.View.FocusedRowHandle;
            if (rowHandle < 0) return;
            OrderClient orderClient = new OrderClient();
            Order ordre = orderClient.GetOrderById(Convert.ToInt32(DgFactures.GetCellValue(rowHandle, "OrderID")));

              

            /* la fcture dans la base de données */
                
            FacturesClient factureClient = new FacturesClient();
            Facture facture = factureClient.GetFactureByOrdre(ordre);


           
            SettingsClient sc = new SettingsClient();
            Setting settings = sc.GetSetting();

            FactureCase factureCase = new FactureCase
            {
                Order = ordre,
                TotalHt = _total,
                Ttc = _ttc,
                Tva = _tva,
                Timbre = _timbre,
                Numbre = facture.FactureNum.ToString(),
                Facture = facture
            };

            ////InvoiceReport report = new InvoiceReport(factureCase);


            //if (settings.Logo != null) report.Logo = Validator.ConvertByteArrayToImage(settings.Logo);
               
                        
            //report.PayMode = facture.TypePayment;
            //report.SetSettings();
            //PrintHelper.ShowPrintPreview(null, report).WindowState = WindowState.Maximized;

            /* fin de l'impression de la facture */

            /* debut de modifiction de l'après impression de la facture */

               
            LoadGridFact();

            /* fin de modifiction de l'après impression de la facture */
        }

        private void TableView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            FacturesClient fc = new FacturesClient();
            if (DgFactures.VisibleRowCount == 0) return;
            int rowHandle = DgFactures.View.FocusedRowHandle;
            if (rowHandle >= 0)
            {
         
                OrderClient oc = new OrderClient();
                Order ordre = oc.GetOrderById(Convert.ToInt32(DgFactures.GetCellValue(rowHandle, "OrderID")));
                FacturesClient factureClient = new FacturesClient();
                Facture facture = factureClient.GetFactureByOrdre(ordre);
                if (ordre.Status == 10) {PrintBtn.IsEnabled = false;  PrintProBtn.IsEnabled = true;}
                if (ordre.Status == 0) {PrintProBtn.IsEnabled = false; PrintBtn.IsEnabled = true;}
                
                //////////////////////
       
                if (ordre.OrderDate != null) DtFactureDate.DateTime = (DateTime)ordre.OrderDate;
                CbCustomerAddress.Text = ordre.Customer.Address;
                CbSecurityNum.Text = ordre.Customer.RC;       // a revoir
                CbRs.Text = ordre.Customer.CompanyName;
                NumTxtBox.Text = facture.FactureNum.ToString();
                Calculate(ordre);
                Afficher();
            }
        }

        private void FactureControl_GotFocus(object sender, RoutedEventArgs e)
        {
            CbTva.ItemsSource = _tvaClient.GetTva();
            if (CbTva.SelectedIndex < 0) CbTva.SelectedIndex = 0;
            CbTva.DisplayMember = "TauxTVA";

        }

        private void PrintProBtn_Click(object sender, RoutedEventArgs e)
        {

            if (DgFactures.VisibleRowCount == 0) return;
            int rowHandle = DgFactures.View.FocusedRowHandle;
            if (rowHandle >= 0)
            {
                OrderClient oc = new OrderClient();
                Order ordre = oc.GetOrderById(Convert.ToInt32(DgFactures.GetCellValue(rowHandle, "OrderID")));



                /* la fcture dans la base de données */

                var factureClient = new FacturesClient();
                var facture = factureClient.GetFactureByOrdre(ordre);



                var settingsClient = new SettingsClient();
                var settings = settingsClient.GetSetting();

                var factureCase = new FactureCase
                {
                    Order = ordre,
                    TotalHt = _total,
                    Ttc = _ttc,
                    Tva = _tva,
                    Timbre = _timbre,
                    Numbre = facture.FactureNum.ToString(),
                    Facture = facture
                };

                //var report = new InvoiceReport(factureCase);


                //if (settings.Logo != null) report.Logo = Validator.ConvertByteArrayToImage(settings.Logo);

                //report.PayMode = facture.TypePayment;
                //report.SetSettings();
                //PrintHelper.ShowPrintPreview(null, report).WindowState = WindowState.Maximized;

                /* fin de l'impression de la facture */

                /* debut de modifiction de l'après impression de la facture */


                LoadGridFact();

                /* fin de modifiction de l'après impression de la facture */
            }
        }

        private void cbPayment_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            String s = CbPayment.Text;
            if (CbPayment.Text.Equals("Espèce"))
            {
                TimbreTxtBox.Visibility = Visibility.Visible;
                //label1.Visibility = Visibility.Visible;
                _timbre = _total / 100;
                if (_timbre > 2500) _timbre = 2500;
                _ttc = _total - _discount + (_total * _tva) + _timbre;
                TimbreTxtBox.Text = _timbre.ToString("###,###.00");
                TotalTxtBox.Text = _ttc.ToString("###,###.00");
            }
            else
            {
                TimbreTxtBox.Visibility = Visibility.Hidden;
               // label1.Visibility = Visibility.Hidden;
                _ttc = _total - _discount + (_total * _tva);
                TimbreTxtBox.Text = _timbre.ToString("###,###.00");
                TotalTxtBox.Text = _ttc.ToString("###,###.00");
            }
        }

        private void cbTVA_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            _tva = GetTva();
            _discount = getDiscount() * _total;

            _timbre = _total / 100;
            if (_timbre > 2500) _timbre = 2500;
            if (!CbPayment.Text.Equals("Espèce")) _timbre = 0;

            _ttc = _total - _discount + (_total * _tva) + _timbre;
            TotalTxtBox.Text = _ttc.ToString("###,###.00");
            
        }


}
}

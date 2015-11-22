using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.LayoutControl;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.ClassesClients;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.DomainModel.Invoice;
using GestionCommerciale.DomainModel.UseCases;
using GestionCommerciale.Modals;
using GestionCommerciale.Modals.TVA;

namespace GestionCommerciale.Views.Invoices
{

    /// <summary>
    /// Interaction logic for AddCustomerView.xaml
    /// </summary>    
    public partial class FactureView
    {

        Decimal _total;
        Decimal _ttc;
        Decimal _discount;
        Decimal _tva;
        Decimal _timbre;
        private const String TypeFacture = "Facture";

        readonly TvaClient _tvaClient = new TvaClient();
        FacturesClient _factureclient = null;
        public FactureView()
        {
            InitializeComponent();
        }

        public FactureView(string animationName)
        {
            InitializeComponent();
            _factureclient = new FacturesClient();
            if (string.IsNullOrEmpty(animationName)) return;
            Storyboard animation = (Storyboard) Application.Current.Resources[animationName];
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
            TVADialog tvaDialog = new TVADialog();
            tvaDialog.ShowDialog();
            if (tvaDialog.DialogResult == true)
            { }
        }

        private void CustomersBtn_Click(object sender, RoutedEventArgs e)
        {
            CustomerDialog customerDialog = new CustomerDialog();
            customerDialog.ShowDialog();
        }

        private void SaleSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            SaleDialog saleDialog = new SaleDialog();
            saleDialog.ShowDialog();
        }

        private void AddFactureBtn_Click(object sender, RoutedEventArgs e)
        {
            
            //facture adding
            //Verify that the invoice doesn't exist
    
            CustomerOrder order = (CustomerOrder)DgFactures.GetFocusedRow();
            // Get selected TVA           



            TVA tva = (TVA)CbTva.EditValue;

            var facturesClient = new FacturesClient();
            Facture factures = new Facture
            {
                FactureDate = DtFactureDate.DateTime,
                TypePayment = CbPayment.Text,
                TVA = tva,
                Order = order.Order,
                FactureNum = facturesClient.RetrieveFactureNumber(NumTxtBox.Text)
            };

            //  f.Total = factures.GetSaleTotal(order,tva);
            facturesClient.AddFacture(factures);
            DXMessageBox.Show("Facture ajoutée avec success.");
          
            PrintBtn.IsEnabled = true;
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
            CbTva.ItemsSource = _tvaClient.GetAllTvas();
            if (CbTva.SelectedIndex < 0) CbTva.SelectedIndex = 0;
            CbTva.DisplayMember = "TauxTVA";

            if (CbPayment.SelectedIndex < 0) CbPayment.SelectedIndex = 0;

            LoadGridFact();
        }

        private void LoadGridFact()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {


                var oc = new OrderClient();
                var listeOrdersNonFactured = oc.GetOrderNotFactured();
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
            OrderDetailsClient orderDetailsClient = new OrderDetailsClient();
            List<OrderDetail> listeOd = orderDetailsClient.GetOrderDetailsByOrder(ordre);
            foreach (OrderDetail od in listeOd)
            {
                _total += od.UnitPrice * od.Quantity;// a refaire en cas de comande distribuée
            }
            _tva = GetTva();
            _discount = GetDiscount() * _total;

            _timbre = _total / 100;
            if (_timbre > 2500) _timbre = 2500;
            if (!CbPayment.Text.Equals("Espèce")) _timbre = 0;

            _ttc = _total - _discount + (_total * _tva) + _timbre;
        }

        private static decimal GetDiscount()
        {
            // if (!String.IsNullOrEmpty(DiscountTxtBox.Text)) return Convert.ToDecimal(DiscountTxtBox.Text) / 100;
            return 0;
        }

        private void cbSecurityNum_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {

        }
        private void dXTabControl1_SelectionChanged(object sender, TabControlSelectionChangedEventArgs e)
        {
            if (e.NewSelectedIndex == 0) return;
          
        }
        
        private void tblSale_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            Order order = (Order)e.NewRow;
            if (order == null) return;
            Calculate(order);
            Afficher();

        }

        private void cbCustomerAddress_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {

        }

        private void btnAddTva_Click_1(object sender, RoutedEventArgs e)
        {
            AddTVADialog addTvaDialog = new AddTVADialog();
            addTvaDialog.ShowDialog();
        }

        private void cbTVA_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
       
        }
        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {

            if (DgFactures.VisibleRowCount == 0) return;
            int rowHandle = DgFactures.View.FocusedRowHandle;
            if (rowHandle < 0) return;
            var orderClient = new OrderClient();
            var ordre = orderClient.GetOrderById(Convert.ToInt32(DgFactures.GetCellValue(rowHandle, "OrderID")));

            if (ordre == null) return;

            /* la fcture dans la base de données */
              
            var factureClient = new FacturesClient();
            var facture = new Facture
            {
                FactureDate = DateTime.Now,
                Order = ordre,
                FactureNum = Convert.ToInt32(NumTxtBox.Text),
                TVA = _tvaClient.GetTvaByName(_tva.ToString()),
                TypePayment = CbPayment.Text,
                Status = 0,
                Type = TypeFacture
            };
            //ou facture proforma
            factureClient.AddFacture(facture);

            /* fin de l'ajout de la facture a la base de données */

            /* debut de l'impression de la facture */
            var settingsClient = new SettingsClient();
            Setting settings = settingsClient.GetSetting();

            FactureCase factureCase = new FactureCase
            {
                Order = ordre,
                TotalHt = _total,
                Ttc = _ttc,
                Tva = _tva,
                Timbre = _timbre,
                Numbre = NumTxtBox.Text,
                Facture = facture
            };

            //var report = new InvoiceReport(factureCase);


            //if (settings.Logo!= null)  report.Logo = Validator.ConvertByteArrayToImage(settings.Logo);
            ////report.HeaderContent = "ITCastle Company   Boumerdes";            
            //report.PayMode = CbPayment.Text;
            //report.SetSettings();
            //PrintHelper.ShowPrintPreview(null, report).WindowState = WindowState.Maximized;

            /* fin de l'impression de la facture */

            /* debut de modifiction de l'après impression de la facture */

            settings.FactureNumber = (Convert.ToInt32(settings.FactureNumber) + 1).ToString();
            SettingsClient.MajSettings(settings);
            LoadGridFact();

            /* fin de modifiction de l'après impression de la facture */
        }

        private void TableView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            var facturesClient = new FacturesClient();
            if (DgFactures.VisibleRowCount == 0) return;
            int rowHandle = DgFactures.View.FocusedRowHandle;
            if (rowHandle < 0) return;
            var orderClient = new OrderClient();
            var ordre = orderClient.GetOrderById(Convert.ToInt32(DgFactures.GetCellValue(rowHandle, "OrderID")));

            if (ordre == null) return;
            if (ordre.OrderDate != null) DtFactureDate.DateTime = (DateTime)ordre.OrderDate;
            CbCustomerAddress.Text = ordre.Customer.Address;
            CbSecurityNum.Text = ordre.Customer.RC;       // a revoir
            NumTxtBox.Text = facturesClient.GetFactureNumber().ToString();
            CbRs.Text = ordre.Customer.CompanyName;
            Calculate(ordre);
            Afficher();
        }

        private void FactureControl_GotFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CbTva.ItemsSource = _tvaClient.GetAllTvas();
                if (CbTva.SelectedIndex < 0) CbTva.SelectedIndex = 0;
                CbTva.DisplayMember = "TauxTVA";
            }));
        }

        private void cbPayment_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
           // String s = CbPayment.Text;
            if (CbPayment.Text.Equals("Espèce"))
            {
                TimbreTxtBox.Visibility = Visibility.Visible;
                Label1.Visibility = Visibility.Visible;
                _timbre = _total / 100;
                if (_timbre > 2500) _timbre = 2500;
                _ttc = _total - _discount + (_total * _tva) + _timbre;
                TimbreTxtBox.Text = _timbre.ToString("###,###.00");
                TotalTxtBox.Text = _ttc.ToString("###,###.00");
            }
            else {
                TimbreTxtBox.Visibility = Visibility.Hidden;
                Label1.Visibility = Visibility.Hidden;
                _ttc = _total - _discount + (_total * _tva) ;
                TimbreTxtBox.Text = _timbre.ToString("###,###.00");
                TotalTxtBox.Text = _ttc.ToString("###,###.00");
            }
        }

        private void cbTVA_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            _tva = GetTva();
            _discount = GetDiscount() * _total;

            _timbre = _total / 100;
            if (_timbre > 2500) _timbre = 2500;
            if (!CbPayment.Text.Equals("Espèce")) _timbre = 0;

            _ttc = _total - _discount + (_total * _tva) + _timbre;
            TotalTxtBox.Text = _ttc.ToString("###,###.00");
            
        }
    }
}

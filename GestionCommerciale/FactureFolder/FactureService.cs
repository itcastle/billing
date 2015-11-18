using System;
using System.Data;
using System.Drawing.Printing;
using System.Globalization;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.FactureFolder
{
    public partial class FactureService : XtraReport
    {
        public DataTable ServicesTable { set; get; }
        public client TheClient { set; get; }
        public DateTime TheServiceDate { set; get; }
        public string TheCommandeNumber { set; get; }
        public string ModePayment { get; set; }
        public string EtatPayment { get; set; }
        public string FactureType { get; set; }
        public DateTime CommandeDate { get; set; }
        public int TheServiceFactureID { get; set; }

        private int _counter;
        public FactureService()
        {
            InitializeComponent();
        }

        private void FactureService_OnBeforePrint(object sender, PrintEventArgs e)
        {
            _counter = ServicesTable.Rows.Count;
            
            servicesDataTable1.DataTable1.FillOrdonnanceTable(ServicesTable); 
        }
        


        private void xrTableCell1_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            if (_counter == 1)
            {
                xrTableCell1.Borders = BorderSide.All;
            }
            else if (_counter > 1)
            {
                
                xrTableCell1.Borders = BorderSide.Left | BorderSide.Right;
               
            }
            
        }

        private void xrTableCell2_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            if (_counter == 1)
            {
                xrTableCell2.Borders = BorderSide.All;
            }
            else if (_counter > 1)
            {

                xrTableCell2.Borders = BorderSide.Left | BorderSide.Right;

            }
          
        }

        private void xrTableCell3_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            if (_counter == 1)
            {
                xrTableCell3.Borders = BorderSide.All;
            }
            else if (_counter > 1)
            {

                xrTableCell3.Borders = BorderSide.Left | BorderSide.Right;

            }
           
        }

        private void xrTableCell4_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            if (_counter == 1)
            {
                xrTableCell4.Borders = BorderSide.All;
            }
            else if (_counter > 1)
            {

                xrTableCell4.Borders = BorderSide.Left | BorderSide.Right;

            }
           
        }

        private void xrTableCell5_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            if (_counter == 1)
            {
                xrTableCell5.Borders = BorderSide.All;
            }
            else if (_counter > 1)
            {

                xrTableCell5.Borders = BorderSide.Left | BorderSide.Right;

            }
          
        }

        private void xrLabel2_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            //var configurationManager=new ConfigurationManager();
            //string raisonsocial=configurationManager.GetRaisonSocial();
            //xrLabel2.Text = raisonsocial;
        }

        private void xrLabel20_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            // Load client Name;
            if (TheClient != null)
            {
                xrLabel20.Text = TheClient.ClientRS;
            }
        }

        private void xrLabel18_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            // Load CLient address
            if (TheClient != null)
            {
                xrLabel18.Text = TheClient.ClientAddress;
            }
        }

        private void xrLabel19_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            // Load Client RC;
            if (TheClient != null)
            {
                xrLabel19.Text = TheClient.ClientRC;
            }
        }

        private void xrLabel21_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            // Load Client fiscale;
            if (TheClient != null)
            {
                xrLabel21.Text = TheClient.ClientNIF;
            }
        }

        private void xrLabel22_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            // Load client AI;
            if (TheClient != null)
            {
                xrLabel22.Text = TheClient.ClientAI;
            }
        }

        private void xrLabel14_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            // Load facture Date;
            xrLabel14.Text = TheServiceDate.ToShortDateString();
        }

        private void xrLabel15_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            // Load bon de commande number
            xrLabel15.Text = TheCommandeNumber;
        }

        private void xrLabel16_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            // load mode de payment;
            xrLabel16.Text = ModePayment;
        }

        private void xrLabel17_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            // load etat de payment;
            xrLabel17.Text = EtatPayment;
        }

        private void xrLabel13_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            // load type Facture Type;
            xrLabel13.Text = FactureType;
        }

        private void xrLabel1_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
           // var configurationManager = new ConfigurationManager();
          //  string infoFooter = configurationManager.GetAdressFooter();
         //   xrLabel1.Text = infoFooter;
        }

        private void xrLabel25_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            // load commande date;
            xrLabel25.Text = CommandeDate.ToShortDateString();
        }

        private void xrLabel23_OnPrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            xrLabel23.Text = TheServiceFactureID.ToString(CultureInfo.InvariantCulture);
        }

    }
}

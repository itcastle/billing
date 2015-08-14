using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using GestionCommerciale.DomainModel.Entities;
using DevExpress.XtraReports.UI;
using GestionCommerciale.DomainModel;

namespace GestionCommerciale.Views.Prints
{
    public partial class InvoiceReport : XtraReport
    {
        public List<InvoiceDetail> Details;
        public DataTable InvoiceTable { set; get; }

        public string CompanyName
        {            
            set { txtCompanyName.Text = value; }
        }
        public Image Logo
        {
            set { pbLogo.Image = value; }
        }
        public string CSN
        {
            set { txtCSN.Text = value; }
        }
        public string CPhone
        {
            set { txtCNIS.Text = value; }
        }
        public string CFax
        {
            set { txtCNF.Text = value; }
        }
        public string Date
        {
            set { txtDate.Text = value; }
        }
        public string PayMode
        {
            set { txtPayMode.Text = value; }
        }
        public string Company
        {
            set { txtCompany.Text = value; }
        }
        public string SN
        {
            set { txtSN.Text = value; }
        }
        public string Address
        {
            set { txtAddress.Text = value; }
        }
        FactureCase TheFactureCase { get; set; }
        public InvoiceReport(FactureCase factureCase)
        {            
            InitializeComponent();      
      
         
            TheFactureCase = factureCase;     

            Facture invoice = factureCase.Facture;          
            Customer customer = invoice.Order.Customer;
            Parameters["InvoiceNum"].Value = factureCase.Numbre;
           
            if (factureCase.Order.Status==0)
            xrLabel5.Text = "Facture N° : ";
            if (factureCase.Order.Status == 10)
                xrLabel5.Text = "Facture Proforma N° : ";



            

            txtCompany.Text = factureCase.Order.Customer.CompanyName;
            txtSN.Text = factureCase.Order.Customer.RC;
            txtAddress.Text = factureCase.Order.Customer.Address;

                    
        }

        private void lblTotal_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTotal.Text)) return;
            lblWord.Text = NumberToWords.CurrencyNumberToFrenchWords(double.Parse(txtTotal.Text));
        }


        public void SetSettings()
        {
            SettingsClient sc=new SettingsClient();
            Setting settings = sc.GetSetting();
            txtCompanyName.Text = settings.CompanyName;
            txtCSN.Text = settings.RC;
            txtCNIS.Text = settings.NIS;
            txtCNF.Text = settings.NF;
            txtCPhone.Text = settings.Phone;
            txtCFax.Text = settings.Fax;
            txtTown.Text = settings.City;
            txtCompany.Text = settings.CompanyName;
            txtCAddress.Text = settings.Adresse;
            txtAI.Text = settings.AI;
            txtEmail.Text = settings.Email;
        }

        private void InvoiceReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

            //****************
            examenTable1.DataTable1.FillTable(InvoiceTable);
            //****************


            if (TheFactureCase == null) return;
            FacturesClient invoices = new FacturesClient();
            Details = invoices.GetFactureDetails(TheFactureCase);
     
            //Test
            //txtItem.DataBindings.Add("Text", Details, "ProductName");
            //txtItem.Summary = new XRSummary(SummaryRunning.Report, SummaryFunc.RecordNumber, "{0}");
            //txtProductName.DataBindings.Add("Text", Details, "ProductName");
            //txtQuantity.DataBindings.Add("Text", Details, "Quantity");
            //txtUnitPrice.DataBindings.Add("Text", Details, "UnitPrice");
            //txtUnitPrice.DataBindings[0].FormatString = "{0:n2}";
            // Amount.Expression = "[Quantity]*[UnitPrice]";
            //txtAmount.DataBindings.Add("Text", Details, "Amount");
            //txtAmount.DataBindings[0].FormatString = "{0:n2}";

            //txtUnit.DataBindings.Add("Text", Details, "Unite");

            //txtPTva.DataBindings.Add("Text", Details, "PTVA");
            //txtTotalNoTax.DataBindings.Add("Text", Details, "TotalHT");
            //txtTimbre.DataBindings.Add("Text", Details, "Timbre");
            //txtTotal.DataBindings.Add("Text", Details, "Total");
            //txtDate.DataBindings.Add("Text", Details, "InvoiceDate");
            //txtDate.DataBindings[0].FormatString = "{0:dd-MMM-yyyy}";
            //txtPayMode.DataBindings.Add("Text", Details, "PayMode");

            //txtTotalNoTax.DataBindings[0].FormatString = "{0:n2}";
            //txtPTva.DataBindings[0].FormatString = "{0:n2}";
            //txtTotal.DataBindings[0].FormatString = "{0:n2}";
        }

        private void txtAI_PrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            //if (theclint == null) return;
            //txtAI.Text = theclient.RS;
        }
       
    }
}

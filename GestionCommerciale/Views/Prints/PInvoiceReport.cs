using System;
using System.Collections.Generic;
using System.Drawing;
using GestionCommerciale.DomainModel.Entities;
using DevExpress.XtraReports.UI;
using GestionCommerciale.DomainModel;

namespace GestionCommerciale.Views.Prints
{
    public partial class PInvoiceReport : XtraReport
    {
        private List<InvoiceDetail> _details = null;


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
        
        public PInvoiceReport(FactureCase factureCase)
        {            
            InitializeComponent();            
            FacturesClient invoices = new FacturesClient();
                        

            Facture invoice = factureCase.Facture;          
            Customer customer = invoice.Order.Customer;
            Parameters["InvoiceNum"].Value = factureCase.Numbre;
            _details = invoices.GetFactureDetails(factureCase);           
            DataSource = _details;                      
            //Test
            txtItem.DataBindings.Add("Text", _details, "ProductName");
            txtItem.Summary = new XRSummary(SummaryRunning.Report, SummaryFunc.RecordNumber, "{0}");
            txtProductName.DataBindings.Add("Text", _details, "ProductName");
            txtQuantity.DataBindings.Add("Text", _details, "Quantity");
            txtUnitPrice.DataBindings.Add("Text", _details, "UnitPrice");            
            txtUnitPrice.DataBindings[0].FormatString = "{0:n2}";
           // Amount.Expression = "[Quantity]*[UnitPrice]";
            txtAmount.DataBindings.Add("Text", _details, "Amount");
            txtAmount.DataBindings[0].FormatString = "{0:n2}";

            txtUnit.DataBindings.Add("Text", _details, "Unite");

            txtPTva.DataBindings.Add("Text", _details, "PTVA");
            txtTotalNoTax.DataBindings.Add("Text", _details, "TotalHT");
            txtTimbre.DataBindings.Add("Text", _details, "Timbre");
            txtTotal.DataBindings.Add("Text", _details, "Total");
            txtDate.DataBindings.Add("Text", _details, "InvoiceDate");
            txtDate.DataBindings[0].FormatString = "{0:dd-MMM-yyyy}";
            txtPayMode.DataBindings.Add("Text", _details, "PayMode");
            if (factureCase.Order.Status==0)
            xrLabel5.Text = "Facture N° : ";
            if (factureCase.Order.Status == 10)
                xrLabel5.Text = "Facture Proforma N° : ";



            txtTotalNoTax.DataBindings[0].FormatString = "{0:n2}";
            txtPTva.DataBindings[0].FormatString = "{0:n2}";
            txtTotal.DataBindings[0].FormatString = "{0:n2}";

                    
        }

        private void lblTotal_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTotal.Text)) return;
            lblWord.Text = NumberToWords.CurrencyNumberToFrenchWords(double.Parse(txtTotal.Text));
        }


        public void SetSettings()
        {
            SettingsClient settingsClient = new SettingsClient();
            Setting settings = settingsClient.GetSetting();
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
       
    }
}

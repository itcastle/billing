using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using DevExpress.XtraRichEdit;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.ClassesClients;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.Helpers;

namespace GestionCommerciale.Views.Invoices
{
    /// <summary>
    /// Interaction logic for NewInvoiceView.xaml
    /// </summary>
    public partial class NewInvoiceView : Window
    {

        private readonly Facture _invoice;
        private readonly FacturesClient _factureManager = new FacturesClient();
        private readonly DocModelsManager _docModelManager = new DocModelsManager();
        private readonly SettingsClient _settingManager = new SettingsClient();
        private List<DocModel> _listModels = new List<DocModel>();

        public NewInvoiceView(Facture theFacture)
        {
          
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            _invoice = _factureManager.GetFactureById(theFacture.FactureID);
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayAllDocModels();
        }


        private void DisplayAllDocModels()
        {
            try
            {
                _listModels = _docModelManager.GetDocModelsByType("facture");
                if(_listModels==null)return;
                Models_ComboBox.ItemsSource = _listModels;
                if (_listModels.Count > 0)
                {
                    Models_ComboBox.SelectedIndex = 0;
                }

            }
            catch (Exception ex)
            {
                Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }
        }

        private void ModelsComboBoxSelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Models_ComboBox.SelectedIndex < 0||_listModels == null) return;
                if (_listModels.Count < Models_ComboBox.SelectedIndex) return;
                string dateOfNow = DateTime.Now.ToShortDateString();
                var companyInfos = _settingManager.GetSetting();
                var companyName = companyInfos.CompanyName ?? "";
                string adress = companyInfos.Adresse ?? "";
                string phone = companyInfos.Phone ?? "";
                string rC = companyInfos.RC ?? "";
                string nis = companyInfos.NIS ?? "";
                LoadDocsModel(_listModels[Models_ComboBox.SelectedIndex], dateOfNow, companyName, adress, phone, rC, nis);
            }
            catch (Exception ex)
            {
                Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }

        }

        private void LoadDocsModel(DocModel model, string dateOfNow, string companyName, string adress, string phone,
            string rC, string nis)
        {
            try
            {
                byte[] decompressed = Helper.Decompress(model.DocFile);
                Stream stream = new MemoryStream(decompressed);

                var date = _invoice.FactureDate != null ? _invoice.FactureDate.Value.Date.ToShortDateString() : "";
                string telephone = "";
                var firstOrDefault = _invoice.Order.Customer.Telephones.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    telephone = firstOrDefault.TELEPHONENUMBER ?? "";
                }
                string cientName = _invoice.Order.Customer.CompanyName ?? "";
                string factureNum = _invoice.FactureNum.ToString() ?? 0.ToString();
                string clientAddress = _invoice.Order.Customer.Address ?? "";

                DocRichEdit.DocRichEdit.LoadDocument(stream, DocumentFormat.OpenXml);

                DocRichEdit.DocRichEdit.Document.Variables.Add("Date", dateOfNow);
                DocRichEdit.DocRichEdit.Document.Variables.Add("CompanyName", companyName);
                DocRichEdit.DocRichEdit.Document.Variables.Add("CompanyAdresse", adress);
                DocRichEdit.DocRichEdit.Document.Variables.Add("CompanyTel", phone);
                DocRichEdit.DocRichEdit.Document.Variables.Add("CompanyRC", rC);
                DocRichEdit.DocRichEdit.Document.Variables.Add("CompanyNIF", nis);
                DocRichEdit.DocRichEdit.Document.Variables.Add("DATE", date);
                DocRichEdit.DocRichEdit.Document.Variables.Add("FN", factureNum);
                DocRichEdit.DocRichEdit.Document.Variables.Add("ClientName", cientName);
                DocRichEdit.DocRichEdit.Document.Variables.Add("ClientAdresse", clientAddress);
                DocRichEdit.DocRichEdit.Document.Variables.Add("ClientTel", telephone);
                DocRichEdit.DocRichEdit.Document.Fields.Update();
                AddProductsToInvoice();
            }

            catch (Exception ex)
            {
                DocRichEdit.DocRichEdit.CreateNewDocument();
            }
        }

        private void AddProductsToInvoice()
        {
            try
            {

                decimal totalHT = 0;
                int i = 0;
                foreach (var productOrd in _invoice.Order.OrderDetails)
                {
                    var row = DocRichEdit.DocRichEdit.Document.Tables[2].Rows.InsertAfter(i);
                    row.Height = 100;
                    DocRichEdit.DocRichEdit.Document.InsertText(row.Cells[0].Range.Start, productOrd.Product.ProductName);
                    DocRichEdit.DocRichEdit.Document.InsertText(row.Cells[1].Range.Start, productOrd.UnitPrice.ToString("###,###.00"));
                    DocRichEdit.DocRichEdit.Document.InsertText(row.Cells[2].Range.Start, productOrd.Quantity.ToString("0.0", CultureInfo.InvariantCulture));
                    DocRichEdit.DocRichEdit.Document.InsertText(row.Cells[3].Range.Start, productOrd.TotalPrice.ToString("###,###.00"));
                    totalHT += productOrd.TotalPrice;
                    i++;
                }

                decimal totalTtc = totalHT;
                if (_invoice.Order.TvaValue.HasValue)
                {
                    totalTtc = totalHT * (1 + _invoice.Order.TvaValue.Value / 100);
                }
                DocRichEdit.DocRichEdit.Document.Variables.Add("TotalHT", totalHT.ToString("###,###.00"));
                DocRichEdit.DocRichEdit.Document.Variables.Add("TotalTTC", totalTtc.ToString("###,###.00"));
                string totalLettres = Helper.ConvertChifreToLettre(totalTtc);
                DocRichEdit.DocRichEdit.Document.Variables.Add("SommeLettres", totalLettres);
                DocRichEdit.DocRichEdit.Document.Fields.Update();




            }
            catch (Exception ex)
            {
                Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }
        }




        private void CancelDocBtn_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                
                    if (Helper.ShowQuestionMessageBox("Confirmation", "Voulez vous vraiment annuler ?") == MessageBoxResult.Yes)
                    {
                        Close();
                    }
                
            }
            catch (Exception ex)
            {
                Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }


        }

        private void SaveDocBtn_OnClick(object sender, RoutedEventArgs e)
        {

            try
            {
                if (SaveDoc_btn.IsEnabled)
                {
                    MemoryStream stream = new MemoryStream();
                    DocRichEdit.DocRichEdit.SaveDocument(stream, DocumentFormat.OpenXml);
                    byte[] compressed = Helper.Compress(stream.ToArray());

                    _factureManager.AddDocToFacture(_invoice, compressed);
                    Close();

                }

            }
            catch (Exception ex)
            {
                Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }

        }

        private void SavePrintDocBtn_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SavePrintDoc_btn.IsEnabled)
                {
                    MemoryStream stream = new MemoryStream();
                    DocRichEdit.DocRichEdit.SaveDocument(stream, DocumentFormat.OpenXml);
                    byte[] compressed = Helper.Compress(stream.ToArray());
                    _factureManager.AddDocToFacture(_invoice, compressed);


                    DocRichEdit.biFilePrint.PerformClick();

                }
            }
            catch (Exception ex)
            {
                Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }
        }

        
    }
}

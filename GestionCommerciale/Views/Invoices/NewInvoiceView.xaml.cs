using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

        private Facture _Invoice;
        private FacturesClient _factureManager = new FacturesClient();
        private DocModelsManager _docModelMNG = new DocModelsManager();
        private SettingsClient _settingMNG = new SettingsClient();
        private List<DocModel> _models = new List<DocModel>();

        public NewInvoiceView(Facture TheFacture)
        {
            _Invoice = _factureManager.GetFactureById(TheFacture.FactureID);
            InitializeComponent();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayAllDocModels();
        }


        private void DisplayAllDocModels()
        {
            try
            {
                _models = _docModelMNG.getDocModelsByType("facture");
                Models_ComboBox.ItemsSource = Helper.GetModelsNamesList(_models);
                if (_models.Count > 0)
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
                if (Models_ComboBox.SelectedIndex >= 0)
                {
                    if (_models != null)
                    {
                        if (_models.Count > Models_ComboBox.SelectedIndex)
                        {
                            LoadDocsModel(_models[Models_ComboBox.SelectedIndex]);
                        }
                    }
                }
            }


            catch (Exception ex)
            {
                Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }

        }
        private void LoadDocsModel(DocModel model)
        {
            try
            {

                byte[] decompressed = Helper.Decompress(model.DocFile);
                Stream stream = new MemoryStream(decompressed);


                DocRichEdit.DocRichEdit.LoadDocument(stream, DocumentFormat.OpenXml);


                var CompanyInfos = _settingMNG.GetSetting();

                DocRichEdit.DocRichEdit.Document.Variables.Add("Date", DateTime.Now.ToShortDateString());
                if (CompanyInfos.CompanyName != null)
                {
                    DocRichEdit.DocRichEdit.Document.Variables.Add("CompanyName", CompanyInfos.CompanyName);
                }

                if (CompanyInfos.Adresse != null)
                {
                    DocRichEdit.DocRichEdit.Document.Variables.Add("CompanyAdresse", CompanyInfos.Adresse);
                }
                if (CompanyInfos.Phone != null)
                {
                    DocRichEdit.DocRichEdit.Document.Variables.Add("CompanyTel", CompanyInfos.Phone);
                }
                if (CompanyInfos.RC != null)
                {
                    DocRichEdit.DocRichEdit.Document.Variables.Add("CompanyRC", CompanyInfos.RC);
                }

                if (CompanyInfos.NIS != null)
                {
                    DocRichEdit.DocRichEdit.Document.Variables.Add("CompanyNIF", CompanyInfos.NIS);
                }
                //--------------------------------------------------------------------
                if (_Invoice.FactureDate != null)
                {
                    DocRichEdit.DocRichEdit.Document.Variables.Add("DATE", _Invoice.FactureDate);
                }
                if (_Invoice.FactureNum != null)
                {
                    DocRichEdit.DocRichEdit.Document.Variables.Add("FN", _Invoice.FactureNum);
                }

                //-------------------------------------------------------------------
                if (_Invoice.Order.Customer.CompanyName != null)
                {

                    DocRichEdit.DocRichEdit.Document.Variables.Add("ClientName", _Invoice.Order.Customer.CompanyName);
                }

                if (_Invoice.Order.Customer.Address != null)
                {

                    DocRichEdit.DocRichEdit.Document.Variables.Add("ClientAdresse", _Invoice.Order.Customer.Address);
                }

                if (_Invoice.Order.Customer.Telephones != null && _Invoice.Order.Customer.Telephones.Count > 0)
                {

                    DocRichEdit.DocRichEdit.Document.Variables.Add("ClientTel", _Invoice.Order.Customer.Telephones.FirstOrDefault().TELEPHONENUMBER);
                }

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

                decimal TotalHT = 0;
                int i = 0;
                foreach (var ProductOrd in _Invoice.Order.OrderDetails)
                {
                    var row = DocRichEdit.DocRichEdit.Document.Tables[2].Rows.InsertAfter(i);
                    row.Height = 100;
                    DocRichEdit.DocRichEdit.Document.InsertText(row.Cells[0].Range.Start, ProductOrd.Product.ProductName);
                    DocRichEdit.DocRichEdit.Document.InsertText(row.Cells[1].Range.Start, ProductOrd.UnitPrice.ToString("###,###.00"));
                    DocRichEdit.DocRichEdit.Document.InsertText(row.Cells[2].Range.Start, ProductOrd.Quantity.ToString("0.0", CultureInfo.InvariantCulture));
                    DocRichEdit.DocRichEdit.Document.InsertText(row.Cells[3].Range.Start, ProductOrd.TotalPrice.ToString("###,###.00"));
                    TotalHT += ProductOrd.TotalPrice;
                    i++;
                }

                decimal TotalTTC = TotalHT;
                if (_Invoice.Order.TvaValue.HasValue)
                {
                    TotalTTC = TotalHT * (1 + _Invoice.Order.TvaValue.Value / 100);
                }
                DocRichEdit.DocRichEdit.Document.Variables.Add("TotalHT", TotalHT.ToString("###,###.00"));
                DocRichEdit.DocRichEdit.Document.Variables.Add("TotalTTC", TotalTTC.ToString("###,###.00"));
                string TotalLettres = Helper.ConvertChifreToLettre(TotalTTC);
                DocRichEdit.DocRichEdit.Document.Variables.Add("SommeLettres", TotalLettres);
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
                if (CancelDoc_btn.IsEnabled)
                {
                    if (Helper.ShowQuestionMessageBox("Confirmation", "Voulez vous vraiment annuler ?") == MessageBoxResult.Yes)
                    {
                        Close();
                    }
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

                    _factureManager.AddDocToFacture(_Invoice, compressed);
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
                    _factureManager.AddDocToFacture(_Invoice, compressed);


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

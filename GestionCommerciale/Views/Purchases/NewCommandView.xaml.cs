using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using DevExpress.XtraRichEdit;
using GestionCommerciale.DomainModel.ClassesClients;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.Views.Purchases
{
    /// <summary>
    /// Interaction logic for NewCommandView.xaml
    /// </summary>
    public partial class NewCommandView : Window
    {
        private readonly Command _command;
        private readonly CommandsManager _commandsManager = new CommandsManager();
        private readonly DocModelsManager _docModelManager = new DocModelsManager();
        private readonly SettingsClient _settingManager = new SettingsClient();
        private  List<DocModel> _listModels = new List<DocModel>();


        public NewCommandView(Command theCommand)
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            _command = _commandsManager.GetCommandByID(theCommand.CommandID);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayAllDocModels();
        }
        private void DisplayAllDocModels()
        {
            try
            {
                _listModels = _docModelManager.GetDocModelsByType("command");
                if (_listModels == null) return;
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
                var ee = Models_ComboBox.SelectedItemValue; 
                if (Models_ComboBox.SelectedIndex < 0||_listModels == null) return;
             
                if (_listModels.Count > Models_ComboBox.SelectedIndex)
                {
                    LoadDocsModel(_listModels[Models_ComboBox.SelectedIndex]);
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

                string dateOfNow = DateTime.Now.ToShortDateString(); 
                var companyInfos = _settingManager.GetSetting();
                var companyName = companyInfos.CompanyName ?? "";
                string adress = companyInfos.Adresse ?? "";
                string phone = companyInfos.Phone ?? "";
                string rC = companyInfos.RC ?? "";
                string nis = companyInfos.NIS ?? "";
                var date = _command.Date != null ? _command.Date.Value.Date.ToShortDateString() : "";
                string telephone = "";
                var firstOrDefault = _command.Purchase.Provider.Telephones.FirstOrDefault();
                if (firstOrDefault != null)
                {
                     telephone = firstOrDefault.TELEPHONENUMBER ?? "";
                }
                string providerName = _command.Purchase.Provider.CompanyName ?? "";
                string commandNum = _command.Num ?? "";
                string providerAddress = _command.Purchase.Provider.Address ?? "";
                //-------------------------------------------------------------------------
                DocRichEdit.DocRichEdit.Document.Variables.Add("Date", dateOfNow);
                DocRichEdit.DocRichEdit.Document.Variables.Add("CompanyName", companyName);
                DocRichEdit.DocRichEdit.Document.Variables.Add("CompanyAdresse", adress);
                DocRichEdit.DocRichEdit.Document.Variables.Add("CompanyTel", phone);
                DocRichEdit.DocRichEdit.Document.Variables.Add("CompanyRC", rC);
                DocRichEdit.DocRichEdit.Document.Variables.Add("CompanyNIF",nis);
                DocRichEdit.DocRichEdit.Document.Variables.Add("DATE", date);
                DocRichEdit.DocRichEdit.Document.Variables.Add("FN", commandNum);
                DocRichEdit.DocRichEdit.Document.Variables.Add("ProviderName", providerName);
                DocRichEdit.DocRichEdit.Document.Variables.Add("ProviderAdresse", providerAddress);
                DocRichEdit.DocRichEdit.Document.Variables.Add("ProviderTel", telephone);
                DocRichEdit.DocRichEdit.Document.Fields.Update();
                AddProductsToCommand();
            }

            catch (Exception ex)
            {
                DocRichEdit.DocRichEdit.CreateNewDocument();
            }
        }
        private void AddProductsToCommand()
        {
            try
            {

                foreach (var productOrd in _command.Purchase.PurchaseStores)
                {
                    var row = DocRichEdit.DocRichEdit.Document.Tables[2].Rows.Append();
                    row.Height = 100;
                    DocRichEdit.DocRichEdit.Document.InsertText(row.Cells[0].Range.Start, productOrd.Product.ProductName);
                    DocRichEdit.DocRichEdit.Document.InsertText(row.Cells[1].Range.Start, productOrd.UnitsOnOrder.Value.ToString("0.0", CultureInfo.InvariantCulture));
                }
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
                MemoryStream stream = new MemoryStream();
                DocRichEdit.DocRichEdit.SaveDocument(stream, DocumentFormat.OpenXml);
                byte[] compressed = Helper.Compress(stream.ToArray());
                _commandsManager.AddDocToCommand(_command, compressed);
                Close();
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

                MemoryStream stream = new MemoryStream();
                DocRichEdit.DocRichEdit.SaveDocument(stream, DocumentFormat.OpenXml);
                byte[] compressed = Helper.Compress(stream.ToArray());
                _commandsManager.AddDocToCommand(_command, compressed);
                DocRichEdit.biFilePrint.PerformClick();
            }
            catch (Exception ex)
            {
                Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }
        }

    }
}

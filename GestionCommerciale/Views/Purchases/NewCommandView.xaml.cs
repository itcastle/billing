using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using DevExpress.XtraRichEdit;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.Helpers;

namespace GestionCommerciale.Views.Purchases
{
    /// <summary>
    /// Interaction logic for NewCommandView.xaml
    /// </summary>
    public partial class NewCommandView : Window
    {
        private Command _Command;
        private CommandsManager _commandsMNG = new CommandsManager();
        private DocModelsManager _docModelMNG = new DocModelsManager();
        private SettingsClient _settingMNG = new SettingsClient();
        private List<DocModel> _models = new List<DocModel>();


        public NewCommandView(Command TheCommand)
        {
            InitializeComponent();
            _Command = _commandsMNG.GetCommandByID(TheCommand.CommandID);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayAllDocModels();
        }
        private void DisplayAllDocModels()
        {
            try
            {
                _models = _docModelMNG.getDocModelsByType("command");
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
                if (_Command.Date != null)
                {
                    DocRichEdit.DocRichEdit.Document.Variables.Add("DATE", _Command.Date);
                }
                if (_Command.Num != null)
                {
                    DocRichEdit.DocRichEdit.Document.Variables.Add("FN", _Command.Num);
                }

                //-------------------------------------------------------------------
                if (_Command.Purchase.Provider.CompanyName != null)
                {

                    DocRichEdit.DocRichEdit.Document.Variables.Add("ProviderName", _Command.Purchase.Provider.CompanyName);
                }

                if (_Command.Purchase.Provider.Address != null)
                {

                    DocRichEdit.DocRichEdit.Document.Variables.Add("ProviderAdresse", _Command.Purchase.Provider.Address);
                }

                if (_Command.Purchase.Provider.Telephones != null && _Command.Purchase.Provider.Telephones.Count > 0)
                {

                    DocRichEdit.DocRichEdit.Document.Variables.Add("ProviderTel", _Command.Purchase.Provider.Telephones.FirstOrDefault().TELEPHONENUMBER);
                }

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

                foreach (var ProductOrd in _Command.Purchase.PurchaseStores)
                {
                    var row = DocRichEdit.DocRichEdit.Document.Tables[2].Rows.Append();
                    row.Height = 100;
                    DocRichEdit.DocRichEdit.Document.InsertText(row.Cells[0].Range.Start, ProductOrd.Product.ProductName);
                    DocRichEdit.DocRichEdit.Document.InsertText(row.Cells[1].Range.Start, ProductOrd.UnitsOnOrder.Value.ToString("0.0", CultureInfo.InvariantCulture));

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

                    _commandsMNG.AddDocToCommand(_Command, compressed);
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
                    _commandsMNG.AddDocToCommand(_Command, compressed);

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

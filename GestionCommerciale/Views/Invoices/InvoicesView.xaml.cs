﻿using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Grid;
using DevExpress.XtraRichEdit;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.Helpers;

namespace GestionCommerciale.Views.Invoices
{
    /// <summary>
    /// Interaction logic for InvoicesView.xaml
    /// </summary>
    public partial class InvoicesView
    {
        public InvoicesView(string animationName)
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            if (string.IsNullOrEmpty(animationName)) return;
            var animation = (Storyboard)Application.Current.Resources[animationName];
            LayoutRoot.BeginStoryboard(animation);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GcdbEntities db = new GcdbEntities();

            TheDataGrid.ItemsSource = db.Factures.ToList();
            
        }

        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            DocRichEdit.biFilePrint.PerformClick();
        }

        private void TheTableView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
          
            if(e.NewRow != null)
            {
                Facture selectedFacture = e.NewRow as Facture;
                if(selectedFacture != null)
                {
                    if (selectedFacture.Document != null)
                    {
                        byte[] decompressed = Helper.Decompress(selectedFacture.Document.DocFile);
                        Stream stream = new MemoryStream(decompressed);
                        

                        DocRichEdit.DocRichEdit.LoadDocument(stream, DocumentFormat.OpenXml);
                    }
                    else DocRichEdit.DocRichEdit.CreateNewDocument();
                }

            }
            else DocRichEdit.DocRichEdit.CreateNewDocument();

        }
    }
}

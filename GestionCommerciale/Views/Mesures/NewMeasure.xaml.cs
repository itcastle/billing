using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using GestionCommerciale.DomainModel.ClassesClients;
using GestionCommerciale.Views.Products;

namespace GestionCommerciale.Views.Mesures
{
    /// <summary>
    /// Interaction logic for NewProductMeasure.xaml
    /// </summary>
    public partial class NewMeasure
    {
        private readonly AddProductView _addProductView;
        private int _measureId;
        private string _measureName;

        public NewMeasure(AddProductView newProductView)
        {
            InitializeComponent();
            _addProductView = newProductView; 
        }

        public void Connect(int connectionId, object target)
        {
        }


        private void AddMeasureBtn_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var measureManager = new MeasureManager();
            string name = MeasureNameTxt.Text;
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrEmpty(name)) return;
            measureManager.AddMeasure(name);
            LoadMeasureGridControl();
        }

        private void UpdateMeasureBtn_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                string name = MeasureNameTxt.Text;
                if (_measureId >= 0 && !string.IsNullOrWhiteSpace(name) && !string.IsNullOrEmpty(name))
                {
                    var measureManager = new MeasureManager();
                    measureManager.UpdateMeasure(_measureId, name);
                    LoadMeasureGridControl();
                }
            }
            catch (Exception)
            {
                //
            }
        }

        private void RefreshBtn_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            LoadMeasureGridControl();
        }

        private void DeleteMeasureBtn_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                if (_measureId < 0) return;
                var measureManager = new MeasureManager();
                measureManager.RemoveMeasure(_measureId);
                LoadMeasureGridControl();
            }
            catch (Exception)
            {
                //
            }
        }


        private void MetroWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadMeasureGridControl();
        }

        private void LoadMeasureGridControl()
        {
            var measureManager = new MeasureManager();
            DataTable measureTable = measureManager.GetMeasureDataTable();
            MeasureGridControl.ItemsSource = measureTable.DefaultView;
            _measureId = -1;
            _measureName = null;
            MeasureNameTxt.Clear();
        }

        private void MeasureTableView_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var selectedRow = (DataRowView) MeasureGridControl.GetFocusedRow();
                if (selectedRow != null)
                {
                    _measureId = (int) selectedRow[0];
                    _measureName = (string) selectedRow[1];

                    MeasureNameTxt.Text = _measureName;
                }
            }
            catch
            {
            }
        }

        private void MetroWindow_OnClosed(object sender, EventArgs e)
        {
            _addProductView.LoadMeasureItemSource();
        }
    }
}
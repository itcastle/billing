using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.LayoutControl;
using GestionCommerciale.Views.CategoryFolder;
using GestionCommerciale.DomainModel;
using Categorys = GestionCommerciale.DomainModel.Entities.Category;

namespace GestionCommerciale.Views.ProductFolder
{
    /// <summary>
    /// Interaction logic for AddCustomerView.xaml
    /// </summary>
    public partial class AddProductView
    {
        private readonly CategorysClient _CategorysClient;
     
        private readonly ProductManger _productManger;
        private readonly MeasureManager _measureManager;
        public AddProductView()
        {
            InitializeComponent();
          
        }

        public AddProductView(string animationName)
        {
            InitializeComponent();
            _CategorysClient = new CategorysClient();
            _productManger = new ProductManger();
             _measureManager = new MeasureManager();
            if (string.IsNullOrEmpty(animationName)) return;
            var animation = (Storyboard) Application.Current.Resources[animationName];
            LayoutRoot.BeginStoryboard(animation);
        }

        private void GroupBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var groupBox = (GroupBox) sender;
            groupBox.Background = Brushes.AliceBlue;
        }

        private void GroupBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var groupBox = (GroupBox) sender;
            groupBox.Background = null;
        }
        
        private void AddCategoryBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var addCat = new AddCategoryView(this);
            addCat.ShowDialog();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (SettingsClient.IsNotLegal()) Application.Current.Shutdown();

            LoadCategoryAndSubCategory();
            LoadMeasureItemSource();
        }

      

        private void CategorysCbx_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            try
            {
                //Categorys Category = (Categorys) e.NewValue;
                string CategoryName = CategorysCbx.Text;
                if (string.IsNullOrEmpty(CategoryName) || string.IsNullOrWhiteSpace(CategoryName)) return;
                SubCategoryCbx.Clear();
                ProductNameTxtBox.Clear();
                MeasureCbx.Clear();
                ProductTypeCbx.Clear();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    SubCategoryCbx.ItemsSource = _CategorysClient.GetSubCategorysNames(CategoryName);

                }));
            }
            catch (Exception exception)
            {
                
                //
            }


        }
        private void SubCategoryCbx_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            ProductNameTxtBox.Clear();
            MeasureCbx.Clear();
            ProductTypeCbx.Clear();

        }
        private void AddProductBtn_OnClick(object sender, RoutedEventArgs e)
        {
        
          //  var image = ImageEdit1.Source as BitmapImage;

            string categoryName = CategorysCbx.Text;
            string subCategoryName = SubCategoryCbx.Text;
            string productName = ProductNameTxtBox.Text;
            string productMeasure = MeasureCbx.Text;
            string productType = ProductTypeCbx.Text;
            string productReference = ProductReferenceTxt.Text;
            int qteMin = (int) ProductQteMinSpin.Value;
            int qteMax = (int) ProductQteMaxSpin.Value;
            string productDesignation = ProductDesignationTxt.Text;
            string productRemarks = ProductRemarksTxt.Text;
            int state = 0;

            String result = _productManger.AddNewProduct(categoryName, subCategoryName, productName, productMeasure, productType,
                productReference, qteMin, qteMax, productDesignation, productRemarks, state);
            DXMessageBox.Show(this, result);
            ClearProductFields();


        }

        private void ClearProductFields()
        {

           CategorysCbx.Clear();
           SubCategoryCbx.Clear();
           ProductNameTxtBox.Clear();
           MeasureCbx.Clear();
           ProductTypeCbx.Clear();
           ProductReferenceTxt.Clear();
           ProductQteMinSpin.Clear();
           ProductQteMaxSpin.Clear();
           ProductDesignationTxt.Clear();
           ProductRemarksTxt.Clear();
        }

      
      
        
        //*************Here I start

        private void AddMeasureBTn_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
          
            NewMeasure newMeasure=new NewMeasure(this);
            newMeasure.ShowDialog();
        }


        public void LoadMeasureItemSource()
        {
          Dispatcher.BeginInvoke(new Action(() =>
            {
         
            var listUnits = _measureManager.GetMeasureList();
            MeasureCbx.ItemsSource = listUnits;
            }));

        }

        public void LoadCategoryAndSubCategory()
        {
            SubCategoryCbx.Clear();
            ProductNameTxtBox.Clear();
            MeasureCbx.Clear();
            ProductTypeCbx.Clear();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CategorysCbx.ItemsSource = _CategorysClient.GetCategorys();

            }));

        }
    }
}

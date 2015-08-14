using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GestionCommerciale.DomainModel.Entities;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Editors;
using GestionCommerciale.DomainModel;

namespace GestionCommerciale.Views.StockFolder
{
    /// <summary>
    /// Interaction logic for AddEmployee.xaml
    /// </summary>
    public partial class StockState
    {

        object _datasource;
        readonly CategorysClient _CategorysClient;
        readonly ProductManger _productManger;

       
        public StockState()
        {
            InitializeComponent();
            _CategorysClient = new CategorysClient();
            _productManger=new ProductManger();
            cbCategorys.ItemsSource = _CategorysClient.GetCategorysNames();
            cbCategorys_SelectedIndexChanged(null, null);
           
        }
        public StockState(string animationName)
        {
            InitializeComponent();
            _CategorysClient = new CategorysClient();
            _productManger = new ProductManger();
            GetCategorys();
            if (string.IsNullOrEmpty(animationName)) return;
            Storyboard animation = (Storyboard)Application.Current.Resources[animationName];
            LayoutRoot.BeginStoryboard(animation);
        }


        private void GetCategorys()
        {
           
          
            cbCategorys.ItemsSource = _CategorysClient.GetCategorysNames();
        }



        #region GroupBox_focus_event

        private void GroupBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            DevExpress.Xpf.LayoutControl.GroupBox groupBox = (DevExpress.Xpf.LayoutControl.GroupBox)sender;
            groupBox.Background = Brushes.AliceBlue;
        }

        private void GroupBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            DevExpress.Xpf.LayoutControl.GroupBox groupBox = (DevExpress.Xpf.LayoutControl.GroupBox)sender;
            groupBox.Background = null;
        }
        #endregion

        private void cbCategorys_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
           string catName = cbCategorys.Text;
           
            object ds = _productManger.GetCategoryByName(catName);//.GetProductsByCategory((int)cat.CategoryID);
            Draw(ds, "Stock");
        }
        private void Draw(object dataSource, string title)
        {

            _datasource = dataSource;
            Series.DataSource = _datasource;
            Series.ArgumentDataMember = "ProductName";
            Series.ValueDataMember = "UnitsOnOrder";
            Series.DisplayName = title;
        }
        private void stockChart_CustomDrawSeriesPoint(object sender, CustomDrawSeriesPointEventArgs e)
        {
            if (e.SeriesPoint.Value < 5)
                e.DrawOptions.Color = Colors.Red;
            else if (e.SeriesPoint.Value >= 4 && e.SeriesPoint.Value <= 30)
                e.DrawOptions.Color = Colors.Yellow;
            else
                e.DrawOptions.Color = Colors.Green;
        }

     

        private void cbCategorys_PopupOpening(object sender, OpenPopupEventArgs e)
        {
            GetCategorys();
        }

   

    
        
    }
}

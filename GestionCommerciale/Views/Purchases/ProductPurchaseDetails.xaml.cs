using System.Globalization;
using System.Threading;
using System.Windows;
using DevExpress.Xpf.Editors;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.Views.Purchases
{
    /// <summary>
    /// Interaction logic for ProductPurchaseDetails.xaml
    /// </summary>
    public partial class ProductPurchaseDetails : Window
    {
       
        public PurchaseStore ProductPurchase
        {
            get { return (PurchaseStore)GetValue(ProductPurchaseProperty); }
            set { SetValue(ProductPurchaseProperty, value); }
        }

        public static readonly DependencyProperty ProductPurchaseProperty =
            DependencyProperty.Register("ProductPurchase", typeof(PurchaseStore), typeof(ProductPurchaseDetails), null);




        public string ValidateBtnText
        {
            get { return (string)GetValue(ValidateBtnTextProperty); }
            set { SetValue(ValidateBtnTextProperty, value); }
        }

        public static readonly DependencyProperty ValidateBtnTextProperty =
            DependencyProperty.Register("ValidateBtnText", typeof(string), typeof(ProductPurchaseDetails), null);

        private bool _isDiscountPercentage = false;

        public ProductPurchaseDetails()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            this.DataContext = ProductPurchase;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AchatPriceUnit_SpinEdit.Focus();
        }

        

        private void AchatInputs_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            CalculateAndDisplay();
        }

        private void CalculateAndDisplay()
        {

            decimal UnitePrice = AchatPriceUnit_SpinEdit.Value;
            decimal Qte = Quantite_SpinEdit.Value;
            decimal TotalBeforDiscount = UnitePrice * Qte;
            AchatTotalBeforeDiscount_SpinEdit.Value = TotalBeforDiscount;


            decimal Discount = Discount_SpinEdit.Value;
            AchatTotal_SpinEdit.Value = TotalBeforDiscount - Discount;
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;

        }
        

        private void DiscountPercentage_SpinEdit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (DiscountPercentage_SpinEdit.IsKeyboardFocusWithin)
            {
                decimal TotalBeforDiscount = AchatTotalBeforeDiscount_SpinEdit.Value;
                decimal DiscountPercentage = DiscountPercentage_SpinEdit.Value;
                Discount_SpinEdit.Value = (TotalBeforDiscount * DiscountPercentage) / 100;
                CalculateAndDisplay();
                _isDiscountPercentage = true;
            }
            
        }

        private void Discount_SpinEdit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (Discount_SpinEdit.IsKeyboardFocusWithin)
            {
                decimal TotalBeforDiscount = AchatTotalBeforeDiscount_SpinEdit.Value;
                decimal Discount = Discount_SpinEdit.Value;
                DiscountPercentage_SpinEdit.Value = (Discount * 100) / TotalBeforDiscount;
                CalculateAndDisplay();
                _isDiscountPercentage = false;

            }
        }
    }
}

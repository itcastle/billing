using System.Windows;
using DevExpress.Xpf.Editors;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.Views.Sales
{
    /// <summary>
    /// Interaction logic for ProductPurchaseDetails.xaml
    /// </summary>
    public partial class ProductSaleDetails : Window
    {
       
        public OrderDetail ProductSale
        {
            get { return (OrderDetail)GetValue(ProductSaleProperty); }
            set { SetValue(ProductSaleProperty, value); }
        }

        public static readonly DependencyProperty ProductSaleProperty =
            DependencyProperty.Register("ProductSale", typeof(OrderDetail), typeof(ProductSaleDetails), null);




        public string ValidateBtnText
        {
            get { return (string)GetValue(ValidateBtnTextProperty); }
            set { SetValue(ValidateBtnTextProperty, value); }
        }

        public static readonly DependencyProperty ValidateBtnTextProperty =
            DependencyProperty.Register("ValidateBtnText", typeof(string), typeof(ProductSaleDetails), null);

        private bool _isDiscountPercentage = false;

        public ProductSaleDetails()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PriceUnit_SpinEdit.Focus();
        }

        

        private void AchatInputs_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            CalculateAndDisplay();
        }

        private void CalculateAndDisplay()
        {

            decimal UnitePrice = PriceUnit_SpinEdit.Value;
            decimal Qte = Quantite_SpinEdit.Value;
            decimal TotalBeforDiscount = UnitePrice * Qte;
            TotalBeforeDiscount_SpinEdit.Value = TotalBeforDiscount;


            decimal Discount = Discount_SpinEdit.Value;
            Total_SpinEdit.Value = TotalBeforDiscount - Discount;
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;

        }
        

        private void DiscountPercentage_SpinEdit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (DiscountPercentage_SpinEdit.IsKeyboardFocusWithin)
            {
                decimal TotalBeforDiscount = TotalBeforeDiscount_SpinEdit.Value;
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
                decimal TotalBeforDiscount = TotalBeforeDiscount_SpinEdit.Value;
                decimal Discount = Discount_SpinEdit.Value;
                DiscountPercentage_SpinEdit.Value = (Discount * 100) / TotalBeforDiscount;
                CalculateAndDisplay();
                _isDiscountPercentage = false;

            }
        }
    }
}

using System.Globalization;
using System.Threading;
using System.Windows;
using DevExpress.Xpf.Editors;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.Views.Sales
{
    /// <summary>
    /// Interaction logic for ProductPurchaseDetails.xaml
    /// </summary>
    public partial class ProductSaleDetails
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
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
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

            decimal unitePrice = PriceUnit_SpinEdit.Value;
            decimal qte = Quantite_SpinEdit.Value;
            decimal totalBeforDiscount = unitePrice * qte;
            TotalBeforeDiscount_SpinEdit.Value = totalBeforDiscount;


            decimal discount = Discount_SpinEdit.Value;
            Total_SpinEdit.Value = totalBeforDiscount - discount;
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

        }
        

        private void DiscountPercentage_SpinEdit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (!DiscountPercentage_SpinEdit.IsKeyboardFocusWithin) return;
            decimal totalBeforDiscount = TotalBeforeDiscount_SpinEdit.Value;
            decimal discountPercentage = DiscountPercentage_SpinEdit.Value;
            Discount_SpinEdit.Value = (totalBeforDiscount * discountPercentage) / 100;
            CalculateAndDisplay();
            _isDiscountPercentage = true;
        }

        private void Discount_SpinEdit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (!Discount_SpinEdit.IsKeyboardFocusWithin) return;
            decimal totalBeforDiscount = TotalBeforeDiscount_SpinEdit.Value;
            decimal discount = Discount_SpinEdit.Value;
            DiscountPercentage_SpinEdit.Value = (discount * 100) / totalBeforDiscount;
            CalculateAndDisplay();
            _isDiscountPercentage = false;
        }
    }
}

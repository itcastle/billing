using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using GestionCommerciale.DomainModel.ClassesClients;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.Helpers;
using GestionCommerciale.Views.Invoices;
using GestionCommerciale.Views.Products;
using GestionCommerciale.Views.Suppliers;
using GestionCommerciale.Views.TVAs;

namespace GestionCommerciale.Views.Sales
{
    /// <summary>
    /// Interaction logic for AddPurchaseView.xaml
    /// </summary>
    public partial class NewSaleView
    {
        public Order TheSale
        {
            get { return (Order)GetValue(TheSaleProperty); }
            set { SetValue(TheSaleProperty, value); }
        }

        public static readonly DependencyProperty TheSaleProperty =
            DependencyProperty.Register("TheSale", typeof(Order), typeof(NewSaleView), null);


        private readonly TabHelper _tabHlp;


        private readonly TvaClient _tvaClient;
        private CustomersManager _customersMNG;
        private readonly ProductManger _productManger;

        private List<Product> _productsList;
        List<OrderDetail> ProductsSale;
        private List<Customer> _customersList;


        private float _total;
        private float _ttc;
        private float _discount;
        private float _tva;
        private float _timbre;



        private Customer _customer;



        public NewSaleView(string animationName, TabHelper hlp)
        {
         
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            _tabHlp = hlp;
            ProductsSale = new List<OrderDetail>();
            _productManger = new ProductManger();

            _tvaClient = new TvaClient();
            TvaComboBox.ItemsSource = _tvaClient.GetTvaValues();

            _customersMNG = new CustomersManager();
            _productManger.ResetUnitsOnOrder();
            if (string.IsNullOrEmpty(animationName)) return;
            var animation = (Storyboard)Application.Current.Resources[animationName];
            LayoutRoot.BeginStoryboard(animation);
            this.TheSale = new Order();
        }
        public NewSaleView(string animationName, TabHelper hlp, Order sale)
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            _tabHlp = hlp;
            ProductsSale = new List<OrderDetail>();
            _productManger = new ProductManger();

            _tvaClient = new TvaClient();
            TvaComboBox.ItemsSource = _tvaClient.GetTvaValues();

            _customersMNG = new CustomersManager();
            _productManger.ResetUnitsOnOrder();
            if (string.IsNullOrEmpty(animationName)) return;
            var animation = (Storyboard)Application.Current.Resources[animationName];
            LayoutRoot.BeginStoryboard(animation);

            this.TheSale = sale;
            this.ProductsSale = sale.OrderDetails.ToList();
           
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (SettingsClient.IsNotLegal()) Application.Current.Shutdown();
            OrderDateDateEdit.DateTime = DateTime.Now;
            LoadGridAllCustomers();
            LoadGridAllProducts();
            TvaComboBox.ItemsSource = _tvaClient.GetAllTvas();
        }



        private void LoadGridAllProducts()
        {
            var getProducts = new BackgroundWorker();
            getProducts.DoWork += GetAllProductsOnDoWork;
            getProducts.RunWorkerCompleted += GetAllProductsOnRunWorkerCompleted;
            getProducts.RunWorkerAsync();
        }

        private void GetAllProductsOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var productsList = _productManger.GetProducts();
            doWorkEventArgs.Result = productsList;

        }

        private void GetAllProductsOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs workerCompleted)
        {
            _productsList = workerCompleted.Result as List<Product>;
            AllProductsDataGrid.ItemsSource = _productsList;
            UpdateProductsDataGridsFromSources();
            CalculateAndDisplay();
        }


        private void LoadGridAllCustomers()
        {
            var getProviders = new BackgroundWorker();
            getProviders.DoWork += GetAllCustomersOnDoWork;
            getProviders.RunWorkerCompleted += GetAllCustomersOnRunWorkerCompleted;
            getProviders.RunWorkerAsync();

        }

        private void GetAllCustomersOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {

            var customersList = _customersMNG.GetAllCustomers();
            doWorkEventArgs.Result = customersList;
        }

        private void GetAllCustomersOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs workerCompleted)
        {
            _customersList = workerCompleted.Result as List<Customer>;
            CustomersLookUpEdit.ItemsSource = _customersList;



        }


        //******************************************************************************************************
        private void DiscountTxtBox_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            CalculateAndDisplay();
        }

        private void TvaComboBox_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            CalculateAndDisplay();
        }

        private void AchatTypeCbx_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {

            CalculateAndDisplay();
        }



        private void AddTvaBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var addTva = new AddTvaView();
            addTva.ShowDialog();
        }

        private void CalculateAndDisplay()
        {



            _total = 0;
            _ttc = 0;
            _discount = 0;
            _tva = 0;

            foreach (var ps in ProductsSale)
            {
                _total += (float)ps.TotalPrice;
                _discount += ps.Discount;
            }

            var SelectedTva = TvaComboBox.SelectedItem as TVA;
            if(SelectedTva != null)
            {
                _tva = SelectedTva.TauxTVA / 100;
            }
           
            

            _timbre = _total / 100;
            if (_timbre > 2500) _timbre = 2500;

            _ttc = _total + (_total * _tva) + _timbre - _discount;

            TotalBeforDiscountTxtBox.Value = (decimal)_total;
            DiscountTxtBox.Value = (decimal)_discount;
            TotalAfterDiscountTxtBox.Value = (decimal)_total - (decimal)_discount;
            TotalPaymentTxtBox.Value = (decimal)_ttc;
            TimbreTxtBox.Value = (decimal)_timbre;

        }



        //******************************************************************************************************
        

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                Order newOrder = Enregistrer();


                ResetAll();
                ShowSuccessAlert("Votre Vente a été enregistrée avec succès");

            }
            catch (Exception ex)
            {
                ShowErrorAlert(ex.Message);
            }
        }


        private void PInvoiceBtn_Click(object sender, RoutedEventArgs e)
        {
            //GcdbEntities gestionDb = new GcdbEntities();

            //if (theOrder == null) return;
            
            //Order getOrder = gestionDb.Orders.FirstOrDefault(c => c.OrderID == theOrder.OrderID);
            //var facture = new Facture
            //{
            //    FactureDate = DateTime.Now,
            //    Order = getOrder,
            //    TVA = _tvaClient.GetTvaByName(_tvaValue.ToString(CultureInfo.InvariantCulture)),
            //    TypePayment = "Espèce",
            //    Status = 10,
            //    Type = "Facture Proforma"
            //};
            //gestionDb.Factures.Add(facture);
            //gestionDb.SaveChanges();


            


            //FactureCase factureCase = new FactureCase
            //{
            //    Order = theOrder,
            //    TotalHt = _total,
            //    Ttc = _ttc,
            //    Tva = _tvaValue,
            //    Timbre = _timbre,
            //    Numbre = facture.FactureNum.ToString(),
            //    Facture = facture
            //};

            

        }

        private void InvoiceBtn_Click(object sender, RoutedEventArgs e)
        {
            GcdbEntities gestionDb = new GcdbEntities();

            Order newOrder = Enregistrer();
            if (newOrder == null) return;

            var SelectedTva = TvaComboBox.SelectedItem as TVA;
            

            var facture = new Facture
            {
                FactureDate = DateTime.Now,
                OrdresID = newOrder.OrderID,
                // FactureNum = Convert.ToInt32(settings.FactureNumber),
                TVAID = SelectedTva.TVAID,
                TypePayment = "Espèce",
                Status = 0,
                Type = "Facture"
            };
            gestionDb.Factures.Add(facture);
            gestionDb.SaveChanges();
            

            NewInvoiceView newInvoiceWnd = new NewInvoiceView(facture);
            newInvoiceWnd.Show();
            ResetAll();
       

        }

        private Order Enregistrer()
        {
            _customer = CustomersLookUpEdit.SelectedItem as Customer;
            if (_customer == null)
            {
                ShowErrorAlert("Veillez choisir un client");
                return null;
            }
            if (ProductsSale.Count == 0)
            {
                ShowErrorAlert("Veillez choisir au moin un produit");
                return null;
            }
            
            String remarque = RemarqueRichTxtBox.Text;
            decimal tvaValue = 0;
            bool IsTvaValidFloat = decimal.TryParse(TvaComboBox.Text, out tvaValue);
            tvaValue = tvaValue / 100;
            
            return CreateVente(ProductsSale, remarque, _customer, OrderDateDateEdit.DateTime, tvaValue);

            
        }

        private Order CreateVente(List<OrderDetail> ProductForSale, string remarque, Customer customer, DateTime dateTime, decimal tvaValue)
        {
            try
            {
                GcdbEntities gestionDb = new GcdbEntities();

                Customer getCustomer = gestionDb.Customers.FirstOrDefault(c => c.CustomerID == customer.CustomerID);
                if (getCustomer == null) return null;

                Order newOrder = new Order
                {
                    OrderDate = dateTime,
                    CommandeDate = dateTime,
                    CustomerID = getCustomer.CustomerID,
                    Customer = getCustomer,
                    TvaValue = tvaValue,
                    Description = remarque,
                    Status = 0,
                };
                foreach (var ps in ProductForSale)
                {
                    OrderDetail newOrderDetail = new OrderDetail
                    {

                        ProductID = ps.Product.ProductID,
                        Quantity = ps.Quantity,
                        UnitPrice = ps.UnitPrice,
                        TotalPrice = ps.TotalPrice,
                    };
                    newOrder.OrderDetails.Add(newOrderDetail);
                }
                gestionDb.Orders.Add(newOrder);
                gestionDb.SaveChanges();
                return newOrder;


            }
            catch (Exception ex)
            {
                ShowErrorAlert(ex.ToString());
                return null;
            }

        }







        //******************************************************************************************************
        private void AddProductToSaleBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                Product TheProduct = AllProductsDataGrid.SelectedItem as Product;

                if (TheProduct != null)
                {
                    ProductSaleDetails ProductToSaleWnd = new ProductSaleDetails();
                    ProductToSaleWnd.ValidateBtnText = "Ajouter";

                    OrderDetail ProductSale = new OrderDetail() { Product = TheProduct };
                    ProductToSaleWnd.ProductSale = ProductSale;
                    if (ProductToSaleWnd.ShowDialog() == true)
                    {

                        int quantite = Convert.ToInt16(ProductToSaleWnd.Quantite_SpinEdit.Value);
                        decimal UnitPrice = ProductToSaleWnd.PriceUnit_SpinEdit.Value;
                        decimal TotalPrice = ProductToSaleWnd.TotalBeforeDiscount_SpinEdit.Value;

                        float discount = (float)ProductToSaleWnd.Discount_SpinEdit.Value;

                        var NewProductSale = new OrderDetail
                        {
                            Product = TheProduct,
                            Quantity = quantite,
                            UnitPrice = UnitPrice,
                          
                            Discount = discount,
                            TotalPrice = TotalPrice
                        };


                        ProductsSale.Add(NewProductSale);
                        UpdateProductsDataGridsFromSources();
                        CalculateAndDisplay();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorAlert(e.ToString());
            }

        }

        private void RemoveProductFromSaleBtn_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                OrderDetail TheProductSale = SelectedProductsDataGrid.SelectedItem as OrderDetail;
                if (TheProductSale != null)
                {

                    var index = ProductsSale.IndexOf(TheProductSale);
                    ProductsSale.RemoveAt(index);
                    UpdateProductsDataGridsFromSources();


                    CalculateAndDisplay();
                }
            }
            catch (Exception ex)
            {
                ShowErrorAlert(ex.ToString());
            }


        }

        private void SelectedProductsTableView_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            try
            {
                OrderDetail TheProductSale = SelectedProductsDataGrid.SelectedItem as OrderDetail;

                if (TheProductSale != null)
                {

                    ProductSaleDetails ProductToSaleWnd = new ProductSaleDetails();
                    ProductToSaleWnd.ValidateBtnText = "Modifier";
                    ProductToSaleWnd.ProductSale = TheProductSale;
                    if (ProductToSaleWnd.ShowDialog() == true)
                    {

                        var index = ProductsSale.IndexOf(TheProductSale);
                        ProductsSale[index] = ProductToSaleWnd.ProductSale;
                        UpdateProductsDataGridsFromSources();
                        CalculateAndDisplay();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorAlert(ex.Message);
            }
        }

        void UpdateProductsDataGridsFromSources()
        {
            AllProductsDataGrid.ItemsSource = _productsList.Where(p => !IsProductsSaleContainProduct(p));
            AllProductsDataGrid.RefreshData();
            SelectedProductsDataGrid.ItemsSource = this.ProductsSale;
            SelectedProductsDataGrid.RefreshData();
        }

        bool IsProductsSaleContainProduct(Product TheProduct)
        {
            var result = this.ProductsSale.FirstOrDefault(ps => ps.Product.ProductID == TheProduct.ProductID);
            return (result != null) ? true : false;
        }
        void ResetAll()
        {
            this.ProductsSale.Clear();
            SelectedProductsDataGrid.ItemsSource = this.ProductsSale;
            SelectedProductsDataGrid.RefreshData();
            LoadGridAllProducts();

            CustomersLookUpEdit.SelectedItem = null;
            OrderDateDateEdit.DateTime = DateTime.Now;
            RemarqueRichTxtBox.Clear();

            CalculateAndDisplay();
        }

        //******************************************************************************************************
        private void AllProductsTableView_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            AddProductToSaleBtn_Click(null, null);
        }

        private void AllProductsDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddProductToSaleBtn_Click(null, null);
            }
        }

        private void SelectedProductsDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SelectedProductsTableView_RowDoubleClick(null, null);
            }
        }

        private void AddSupplierBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = _tabHlp.AddNewTab(typeof(AddSupplierView), "Nouveau fournisseur ", "FadeToLeftAnim");
        }

        private void AddProductBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = _tabHlp.AddNewTab(typeof(AddProductView), "Nouveau produit ", "FadeToLeftAnim");
        }
        void ShowSuccessAlert(string Message)
        {
            msgbar.SetSuccessAlert(Message, 3);
        }
        void ShowErrorAlert(string Message)
        {
            msgbar.SetDangerAlert(Message.ToString());
        }
    }
}

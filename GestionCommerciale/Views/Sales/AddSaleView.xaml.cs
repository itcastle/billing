using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GestionCommerciale.DomainModel.Entities;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.LayoutControl;
using DevExpress.Xpf.Printing;
using GestionCommerciale.Helpers;
using GestionCommerciale.Views.CustomerFolder;
using GestionCommerciale.Views.EmployeeFolder;
using GestionCommerciale.Views.ProductFolder;
using GestionCommerciale.Views.TVAFolder;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.Validator;

namespace GestionCommerciale.Views.SaleFolder
{
    /// <summary>
    /// Interaction logic for AddSale.xaml
    /// </summary>
    /// 


    public partial class AddSaleView
    {
        struct ProduitsSel
        {
            public String ProductName;
            public Decimal UnitsOnOrder;
            public Decimal TotalPrice;
            public Decimal UnitePrice;
            public Product TheProduct;
        }

        GcdbEntities gestionDb = new GcdbEntities();
        readonly TvaClient _tvaClient = new TvaClient();
        readonly ProductManger _productManger = new ProductManger();
        readonly CustomersManager _customersManager = new CustomersManager();
        private Order theOrder;

        List<StockStore> _productsInStockList;
        List<ProduitsSel> _productSelSelected;
        List<Product> _productSelected;
        List<Customer> _customersList;


        Decimal _total;
        Decimal _ttc;
        Decimal _discount;
        Decimal _tvaValue;
        Decimal _timbre;
  

        Customer _customer;


        private readonly TabHelper _tabHlp;
        public AddSaleView()
        {
         
            InitializeComponent();
         
        }
      
        public AddSaleView(string animationName, TabHelper hlp)
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            if (!string.IsNullOrEmpty(animationName))
            {
                Storyboard animation = (Storyboard)Application.Current.Resources[animationName];
                LayoutRoot.BeginStoryboard(animation);
            }
          

            _productSelSelected = new List<ProduitsSel>();
          
    
            TvaComboBox.ItemsSource = _tvaClient.GetTva();
            if (TvaComboBox.SelectedIndex < 0) TvaComboBox.SelectedIndex = 0;
            TvaComboBox.DisplayMember = "TauxTVA";

            _productManger.ResetUnitsOnOrder();
            _productSelected = new List<Product>();
            _tabHlp = hlp;
        

        }

        //*****************************************************************************************
        private void GroupBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

            var groupBox = (GroupBox)sender;
            groupBox.Background = Brushes.AliceBlue;



        }

        private void GroupBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var groupBox = (GroupBox)sender;
            groupBox.Background = null;

        }

  
        //*****************************************************************************************
        

        

        private void AddCustomerBtn_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var item = _tabHlp.AddNewTab(typeof(AddEmployeeView), "Nouveau Employée ", "FadeToLeftAnim");

        }

      

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (SettingsClient.IsNotLegal()) Application.Current.Shutdown();

            OrderDateDateEdit.DateTime = DateTime.Now;
            LoadGridAllCustomers();
            LoadGridAllProducts();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                TvaComboBox.ItemsSource = _tvaClient.GetTva();
                if (TvaComboBox.SelectedIndex < 0) TvaComboBox.SelectedIndex = 0;
                TvaComboBox.DisplayMember = "TauxTVA";
            }));
        }

        //*****************************************************************************************

        private void LoadGridAllProducts()
        {

            try
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _productsInStockList = _productManger.GetProductsInStock();

                    if (_productsInStockList != null)
                        AllProductsDataGrid.ItemsSource = (from t in _productsInStockList
                            select new
                            {
                                ProductName = t.Product.ProductName,
                                VentePriceComptoire = t.VentePriceComptoire,
                                VentePriceDetail = t.VentePriceDetail,
                                VentePriceGros = t.VentePriceGros,
                                UnitsOnOrder = t.UnitsOnOrder,
                                ProductID = t.ProductID
                            }
                            ).ToList();
                }));
            }
            catch (Exception e)
            {
                

            }
        }

        private void LoadGridAllCustomers()
        {

            try
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _customersList = _customersManager.GetCustomers();

                    if (_customersList != null)
                        CustomerDataGrid.ItemsSource = (from t in _customersList
                            select new
                            {
                                CompanyName = t.CompanyName,
                                ContactName = t.ContactName,
                                ContactTitle = t.ContactTitle,
                                CustomerID=t.CustomerID
                            }
                            ).ToList();
                }));
            }
            catch (Exception e)
            {
                

            }
        }

        private void LoadGridProductsSelected()
        {
            try
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (_productSelSelected!=null)
                        SelectedProductsDataGrid.ItemsSource = (from t in _productSelSelected
                            select new
                            {
                                ProductName = t.ProductName,
                                TotalPrice = t.TotalPrice,
                                UnitsOnOrder = t.UnitsOnOrder
                            }
                            ).ToList();
                }));
            }
            catch (Exception e)
            {
                

            }

        }

        //*****************************************************************************************

  

        private void AddProductToSaleBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {

            try
            {
               
                if (AllProductsDataGrid.VisibleRowCount == 0) return;
                int rowHandle = AllProductsDataGrid.View.FocusedRowHandle;
                int productId = (int) AllProductsDataGrid.GetCellValue(rowHandle, "ProductID");
               // LoadGridAllProducts();
                StockStore stockStore = _productManger.GetStockStore(productId);
                if (stockStore == null) return;
               
           
                if (String.IsNullOrEmpty(QuantiteSaleTxtBox.Text) || !Validator.IsNumberValid(QuantiteSaleTxtBox.Text))
                {
                    DXMessageBox.Show(this, "Vous devez saisir une quantité valide");
                    QuantiteSaleTxtBox.Text = "";
                    return;
                }
              

                int qteSalePrice = Convert.ToInt16(QuantiteSaleTxtBox.Text);
                Decimal salePrice = ConvertToDecimal(VentePriceTxt.Text);
               
                ProduitsSel prodSel = new ProduitsSel
                {
                    ProductName = stockStore.Product.ProductName,
                    UnitsOnOrder = qteSalePrice,
                    TotalPrice = salePrice * qteSalePrice,
                    TheProduct = stockStore.Product,
                    UnitePrice = salePrice,
                };
                //produit.UnitsOnOrder += qteSalePrice;
                decimal discount =ConvertToDecimal(DiscountTxtBox.Text);
                //12;
               // int index = _productSelected.IndexOf(produit);
                var exist = _productSelected.Exists(item => item.ProductID == stockStore.Product.ProductID);
                if (!exist)
                {
                    _productSelected.Add(stockStore.Product);
                    _productSelSelected.Add(prodSel);
                }
                else
                {
                    UpdateFunction(stockStore, qteSalePrice, discount, salePrice);
                }
                ClearAfterSave();
               
            }
            catch (Exception exception)
            {
                //
            }
        }

        private void UpdateFunction(StockStore stockStore, int unitsOnorder, decimal discount, decimal salePrice) 
        {

            try
            {
                bool exist = _productSelected.Exists(item => item.ProductID == stockStore.Product.ProductID);
                if (!exist) return;
                var returnProduct = _productSelSelected.Find(item => item.ProductName == stockStore.Product.ProductName);
                var prodSel = new ProduitsSel
                {
                    //ProductId = getProduit.ProductID, 
                    ProductName = stockStore.Product.ProductName,
                    UnitsOnOrder = unitsOnorder + returnProduct.UnitsOnOrder,
                    TotalPrice = salePrice*unitsOnorder + returnProduct.TotalPrice,
                    TheProduct = returnProduct.TheProduct,
                    UnitePrice = salePrice,
                    // PurchasePrice = purchasePrice + returnProduct.PurchasePrice,
                    //Discount = discount,
                    //VentePriceGros = returnProduct.VentePriceGros,
                    //VentePriceDetail = returnProduct.VentePriceDetail,
                    //VentePriceComptoire = returnProduct.VentePriceComptoire,
                    //TotalPriceVente = returnProduct.VentePriceGros * (float)(unitsOnorder + returnProduct.UnitsOnOrder),
                    //TotalPriceAchat = purchasePrice * (float)(unitsOnorder + returnProduct.UnitsOnOrder)
                };

                var index = _productSelSelected.IndexOf(returnProduct);
                _productSelSelected.RemoveAt(index);
                _productSelSelected.Add(prodSel);
            }
            catch (Exception)
            {
                //
            }
        }

        private void ClearAfterSave()
        {

            try
            {
                QuantiteSaleTxtBox.Text = "";
                Calculate();
                Afficher();
                LoadGridProductsSelected();
            }
            catch (Exception)
            {
                
            }
        }

        private void Afficher()
        {
            TotalBeforDiscountTxtBox.Text = _total.ToString("###,###.00");
            TotalAfterDiscountTxtBox.Text = _discount.ToString("###,###.00");
            TotalPaymentTxtBox.Text = _ttc.ToString("###,###.00");
            TimbreTxtBox.Text = _timbre.ToString("###,###.00");
            
        }

        private void Calculate()
        {
            _total = 0;
            _ttc = 0;
            _discount = 0;
            _tvaValue = 0;

            foreach (var entity in _productSelSelected)
            {
                _total += entity.TotalPrice;// a refaire en cas de comande distribuée
            }
            _tvaValue = GetTva();
            _discount = GetDiscount() * _total;
            
            _timbre = _total / 100;
            if (_timbre > 2500) _timbre = 2500;

            _ttc = _total - _discount + (_total * _tvaValue)+_timbre;
        }

        private decimal ConvertToDecimal(string text)
        {
            decimal value = 0;
            try
            {
                if (!String.IsNullOrEmpty(text))
                {
                    value = Convert.ToDecimal(text);
                }
                return value;
            }
            catch (Exception)
            {
                return value;
            }
        }

        private decimal GetDiscount()
        {
            decimal returnDiscount = 0;
            try
            {
               

                if (!String.IsNullOrEmpty(DiscountTxtBox.Text))
                {
                    returnDiscount = Convert.ToDecimal(DiscountTxtBox.Text) / 100;
                }
               
                return returnDiscount;
            }
            catch (Exception)
            {
                return returnDiscount;  
            }
           
        }

        private Decimal GetTva()
        {
          
            decimal tvaValue = (decimal) 0.17;
            try
            {
                if (!String.IsNullOrEmpty(TvaComboBox.Text))
                {
                    tvaValue = Convert.ToDecimal(TvaComboBox.Text);
                }
                return tvaValue;
            }
            catch (Exception)
            {
                return tvaValue;
            }
           
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            DXMessageBox.Show(this,Enregistrer());
            Initialiser();
        }

        private String Enregistrer()
        {
            
            Employee newEmployee = WhoEmployee.Employee;
            _customer = GetCustomer();
            if (_customer == null)
            {
                DXMessageBox.Show(this,"Vous devez choisir un Client");
                return "Commande non Enregistrer";
            }
            if (_productSelSelected.Count == 0)
            {
                DXMessageBox.Show(this,"Vous devez choisir au moin un produit");
                return "Commande non Enregistrer";
            }
            String remarque = RemarqueRichTxtBox.Text;
            decimal tvaValue = ConvertToDecimal(TvaComboBox.Text);
            CreateVente( _productSelSelected, remarque, _customer, OrderDateDateEdit.DateTime, tvaValue, newEmployee);
           
        

            
            return "Votre commande a été enregistrer avec succes";
        }

        private void CreateVente(List<ProduitsSel> productSelSelected, string remarque, Customer customer, DateTime dateTime, decimal tvaValue, Employee newEmployee)
        {
            try
            {

                Customer getCustomer = gestionDb.Customers.FirstOrDefault(c => c.CustomerID==customer.CustomerID);
                if(getCustomer!=null)
                {
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
                    foreach (var entity in productSelSelected)
                    {
                        OrderDetail newOrderDetail = new OrderDetail
                        {
                       
                            ProductID = entity.TheProduct.ProductID,
                            Quantity = (int) entity.UnitsOnOrder,
                            UnitPrice = entity.UnitePrice,
                            TotalPrice = entity.TotalPrice,
                        };
                        newOrder.OrderDetails.Add(newOrderDetail);
                    }
                    gestionDb.Orders.Add(newOrder);
                    gestionDb.SaveChanges();
                    theOrder= newOrder;
                }
                else theOrder = null;
            }
            catch (Exception exe)
            {
                MessageBox.Show(exe.ToString());
                theOrder= null;
            }

        }

       

        private void Initialiser()
        {
            _productSelected = new List<Product>();
            _productSelSelected = new List<ProduitsSel>();
            LoadGridAllProducts();
            LoadGridProductsSelected();
            QuantiteSaleTxtBox.Text = "";
            RemarqueRichTxtBox.Clear();
            OrderDateDateEdit.Clear();
            Calculate();
            Afficher();
        }

        private Customer GetCustomer()
        {
        
            if (CustomerDataGrid.VisibleRowCount == 0) return null;
            int rowHandle = CustomerDataGrid.View.FocusedRowHandle;
            int custorId = (int) CustomerDataGrid.GetCellValue(rowHandle, "CustomerID");
            _customer = _customersManager.GetCustomerById(custorId);

            return _customer;

        }

     

        private void DiscountTxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Calculate();
            Afficher();
        }

        private void RemoveProductFromSaleBtn_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (SelectedProductsDataGrid.VisibleRowCount == 0) return;
                int rowHandle = SelectedProductsDataGrid.View.FocusedRowHandle;
                if(rowHandle<0)return;
               int productId= (int) SelectedProductsDataGrid.GetCellValue(rowHandle, "ProductID");
                var prodSel = new ProduitsSel
                {
                    ProductName = SelectedProductsDataGrid.GetCellValue(rowHandle, "ProductName").ToString(),
                    UnitsOnOrder =
                        Convert.ToDecimal(SelectedProductsDataGrid.GetCellValue(rowHandle, "UnitsOnOrder").ToString()),
                    TotalPrice =
                        Convert.ToDecimal(SelectedProductsDataGrid.GetCellValue(rowHandle, "TotalPrice").ToString())
                };

                var getProducts = _productManger.GetProductById(productId);
                if (getProducts == null) return;

                int index = _productSelSelected.IndexOf(prodSel);
                _productSelSelected.RemoveAt(index);
                
                index = _productSelected.IndexOf(getProducts);
                _productSelected.ElementAt(index).UnitsOnOrder -= (short)prodSel.UnitsOnOrder;
                if (getProducts.UnitsOnOrder == 0)
                {
                    _productSelected.RemoveAt(index);
                }


                QuantiteSaleTxtBox.Text = "";
                Calculate();
                Afficher();
                LoadGridProductsSelected();
            }
            catch (Exception exception)
            {
                
                //
            }
        }

        private void AddTvaBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var addTva = new AddTvaView();
            addTva.ShowDialog();
        }

      

        private void AddCustomerBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = _tabHlp.AddNewTab(typeof(AddCustomerView), "Nouveau client ", "FadeToLeftAnim");
        }

        private void AddProductBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = _tabHlp.AddNewTab(typeof(AddProductView), "Nouveau produit ", "FadeToLeftAnim");
        }

        private void PInvoiceBtn_Click(object sender, RoutedEventArgs e)
        {
       
            if(theOrder==null)return;

            /* Ajout de la facture dans la base de données */

           
     
           // var settingsClient = new SettingsClient();
          //  Setting settings = settingsClient.GetSetting();
            Order getOrder = gestionDb.Orders.FirstOrDefault(c => c.OrderID == theOrder.OrderID);
            var facture = new Facture
            {
                FactureDate = DateTime.Now,
                Order = getOrder,
                TVA = _tvaClient.GetTvaByName(_tvaValue.ToString(CultureInfo.InvariantCulture)),
                TypePayment = "Espèce",
                Status = 10,
                Type = "Facture Proforma"
            };
            gestionDb.Factures.Add(facture);
            gestionDb.SaveChanges();
        


            /* fin de l'ajout de la facture a la base de données */

            /* debut de l'impression de la facture */


            FactureCase factureCase = new FactureCase
            {
                Order = theOrder,
                TotalHt = _total,
                Ttc = _ttc,
                Tva = _tvaValue,
                Timbre = _timbre,
                Numbre = facture.FactureNum.ToString(),
                Facture = facture
            };
            Initialiser();

          //  InvoiceReport report = new InvoiceReport(factureCase);


          // // if (settings.Logo != null)  report.Logo = Validator.ConvertByteArrayToImage(settings.Logo);
          //  //report.HeaderContent = "ITCastle Company   Boumerdes";            
          //  report.PayMode = facture.TypePayment;
          //  report.SetSettings();
          //  PrintHelper.ShowPrintPreview(null, report);//.WindowState = WindowState.Maximized;

          ////  settings.FactureProNumber = (Convert.ToInt32(settings.FactureProNumber) + 1).ToString();
          // // SettingsClient.MajSettings(settings);


          //  /* fin de modifiction de l'après impression de la facture */
          
        }

        private void InvoiceBtn_Click(object sender, RoutedEventArgs e)
        {
            String message = Enregistrer();
           if (!message.Equals("Votre commande a été enregistrer avec succes")) return;
                OrderClient oc = new OrderClient();
                Order ordre = oc.GetOrderById(theOrder.OrderID);

                if (ordre == null) return;

                /* Ajout de la fcture dans la base de données */

               
             
                var settingsClient = new SettingsClient();
             //   Setting settings = settingsClient.GetSetting();
                Order getOrder = GetTheOrder(ordre);
            var facture = new Facture
            {
                FactureDate = DateTime.Now,
                Order = getOrder,
               // FactureNum = Convert.ToInt32(settings.FactureNumber),
                TVA = _tvaClient.GetTvaByName(_tvaValue.ToString(CultureInfo.InvariantCulture)),
                TypePayment = "Espèce",
                Status = 0,
                Type = "Facture"
            };
            gestionDb.Factures.Add(facture);
            gestionDb.SaveChanges();
            //ou facture proforma
           // factureClient.AddFacture(facture);


                /* fin de l'ajout de la facture a la base de données */

                /* debut de l'impression de la facture */


            FactureCase factureCase = new FactureCase
            {
                Order = getOrder,
                TotalHt = _total,
                Ttc = _ttc,
                Tva = _tvaValue,
                Timbre = _timbre,
                Numbre = facture.FactureNum.ToString(),
                Facture = facture
            };
            Initialiser();

            //InvoiceReport report = new InvoiceReport(factureCase)
            //{
            //    PayMode = facture.TypePayment
            //};


            // if (settings.Logo != null) report.Logo = Validator.ConvertByteArrayToImage(settings.Logo);
            //report.HeaderContent = "ITCastle Company   Boumerdes";            
            //report.SetSettings();
                //PrintHelper.ShowPrintPreview(null, report).WindowState = WindowState.Maximized;

                //settings.FactureNumber = (Convert.ToInt32(settings.FactureNumber) + 1).ToString();
                //SettingsClient.MajSettings(settings);
                

                /* fin de modifiction de l'après impression de la facture */
          
        }

        private Order GetTheOrder(Order thisOrder)
        {

            try
            {
                return gestionDb.Orders.FirstOrDefault(c => c.OrderID == thisOrder.OrderID);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void TvaComboBox_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            Calculate();
            Afficher();
        }


        private void AllProductsDataGrid_OnCustomUnboundColumnData(object sender, GridColumnDataEventArgs e)
        {
            try
            {
                decimal venteComptoire = (decimal)AllProductsDataGrid.GetCellValueByListIndex(e.ListSourceRowIndex, "VentePriceComptoire");
                int qte = (int)AllProductsDataGrid.GetCellValueByListIndex(e.ListSourceRowIndex, "Qte");
                decimal dateconsultation = (decimal)AllProductsDataGrid.GetCellValueByListIndex(e.ListSourceRowIndex, "VentePrice");
            }
            catch (Exception)
            {

                //
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using GestionCommerciale.DomainModel.ClassesClients;
using GestionCommerciale.DomainModel.Entities;
using GestionCommerciale.Helpers;
using GestionCommerciale.Views.Products;
using GestionCommerciale.Views.Suppliers;
using GestionCommerciale.Views.TVAs;

namespace GestionCommerciale.Views.Purchases
{
    /// <summary>
    /// Interaction logic for AddPurchaseView.xaml
    /// </summary>
    public partial class AddPurchaseView : UserControl
    {
        public Purchase ThePurchase
        {
            get { return (Purchase)GetValue(ThePurchaseProperty); }
            set { SetValue(ThePurchaseProperty, value); }
        }

        public static readonly DependencyProperty ThePurchaseProperty =
            DependencyProperty.Register("ThePurchase", typeof(Purchase), typeof(AddPurchaseView), null);


        private readonly TabHelper _tabHlp;


        private readonly TvaClient _tvaClient;
        private readonly SuppliersManager _suppliersManager;
        private readonly ProductManger _productManger;

        private List<Product> _productsList;
        List<PurchaseStore> ProductsPurchase;
        private List<Provider> _suppliersList;


        private float _total;
        private float _ttc;
        private float _discount;
        private float _tva;
        private float _timbre;



        private Provider _provider;


        public AddPurchaseView()
        {
            InitializeComponent();
        }
        public AddPurchaseView(string animationName, TabHelper hlp)
        {
            InitializeComponent();
            
            _tabHlp = hlp;
            ProductsPurchase = new List<PurchaseStore>();
            _productManger = new ProductManger();

            _tvaClient = new TvaClient();
            TvaComboBox.ItemsSource = _tvaClient.GetTvaValues();

            _suppliersManager = new SuppliersManager();
            _productManger.ResetUnitsOnOrder();
            if (string.IsNullOrEmpty(animationName)) return;
            var animation = (Storyboard)Application.Current.Resources[animationName];
            LayoutRoot.BeginStoryboard(animation);
            this.ThePurchase = new Purchase()
            {
                IsCommand = false

            };
        }
        public AddPurchaseView(string animationName, TabHelper hlp,Purchase purchase)
        {
            InitializeComponent();

            _tabHlp = hlp;
            ProductsPurchase = new List<PurchaseStore>();
            _productManger = new ProductManger();

            _tvaClient = new TvaClient();
            TvaComboBox.ItemsSource = _tvaClient.GetTvaValues();

            _suppliersManager = new SuppliersManager();
            _productManger.ResetUnitsOnOrder();
            if (string.IsNullOrEmpty(animationName)) return;
            var animation = (Storyboard)Application.Current.Resources[animationName];
            LayoutRoot.BeginStoryboard(animation);

            this.ThePurchase = purchase;
            this.ProductsPurchase = purchase.PurchaseStores.ToList();

        }



        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (SettingsClient.IsNotLegal()) Application.Current.Shutdown();
            OrderDateDateEdit.DateTime = DateTime.Now;
            AchatDateDte.DateTime = DateTime.Now;
            LoadGridAllSuppliers();
            LoadGridAllProducts();
            TvaComboBox.ItemsSource = _tvaClient.GetTvaValues();

            
            


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


        private void LoadGridAllSuppliers()
        {
            var getProviders = new BackgroundWorker();
            getProviders.DoWork += GetAllProvidersOnDoWork;
            getProviders.RunWorkerCompleted += GetAllProvidersOnRunWorkerCompleted;
            getProviders.RunWorkerAsync();

        }

        private void GetAllProvidersOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {

            var suppliersList = _suppliersManager.GetTheSuppliers();
            doWorkEventArgs.Result = suppliersList;
        }

        private void GetAllProvidersOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs workerCompleted)
        {
            _suppliersList = workerCompleted.Result as List<Provider>;
            ProvidersLookUpEdit.ItemsSource = _suppliersList;
            


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

        private float SetTimbre(float total)
        {
            if (AchatTypeCbx.Text != "Facture") return 0;
            _timbre = total / 100;
            if (_timbre > 2500) _timbre = 2500;
            return _timbre;
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

            foreach (var pp in ProductsPurchase)
            {
                _total += (float)pp.TotalPriceAchat.Value;
                _discount += pp.Discount.Value;
            }

            bool IsTvaValidFloat = float.TryParse(TvaComboBox.Text, out _tva);
            _tva =  _tva / 100;

            _timbre = SetTimbre(_total);

            _ttc = _total + (_total * _tva) + _timbre - _discount;

            TotalBeforDiscountTxtBox.Value = (decimal)_total;
            DiscountTxtBox.Value = (decimal)_discount;
            TotalAfterDiscountTxtBox.Value = (decimal)_total - (decimal)_discount;
            TotalPaymentTxtBox.Value = (decimal)_ttc;
            TimbreTxtBox.Value = (decimal)_timbre;

        }



        //******************************************************************************************************
        private void CommandBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                //   Employee employeeEntity = WhoEmployee.Employee; 
                _provider = ProvidersLookUpEdit.SelectedItem as Provider;
                if (_provider == null)
                {
                    ShowErrorAlert("Vous devez choisir un Fournissuer");
                    return;
                }
                if (ProductsPurchase.Count == 0)
                {
                    ShowErrorAlert("Vous devez choisir au moin un produit");
                    return;
                }
                DateTime commandeDtae = OrderDateDateEdit.DateTime;
                float getDicount = (float)DiscountTxtBox.Value;
                string etatPayement = EtatPaymentCbx.Text;
                string modePayement = ModePaymentCbx.Text;
                string modeAchat = AchatTypeCbx.Text;
                string commandNum = CommandeNumberTxt.Text;
                string factureNum = AchatfactureNumTxt.Text;
                float facturePriceHt = (float)AchatfacturePriceHt.Value;
                var remarque = RemarqueRichTxtBox.Text;
                float tvaValue = float.Parse(TvaComboBox.Text);

                Purchase TheNewPurchase = CreatePurchase(getDicount, etatPayement, modePayement, modeAchat,
                    commandNum, factureNum, facturePriceHt, remarque, _provider, commandeDtae, tvaValue,
                    true);

                CommandsManager CommandsMNG = new CommandsManager();
                Command TheNewCommand =  CommandsMNG.CreateCommand(TheNewPurchase, DateTime.Now, "");
                NewCommandView NewCommandWND = new NewCommandView(TheNewCommand);
                NewCommandWND.Show();

                ResetAll();
                ShowSuccessAlert("Votre Commande a été enregistré avec succès");

            }
            catch (Exception ex)
            {
                ShowErrorAlert(ex.Message);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                //   Employee employeeEntity = WhoEmployee.Employee; 
                _provider = ProvidersLookUpEdit.SelectedItem as Provider;
                if (_provider == null)
                {
                    ShowErrorAlert("Vous devez choisir un Fournissuer");
                    return;
                }
                if (ProductsPurchase.Count == 0)
                {
                    ShowErrorAlert("Vous devez choisir au moin un produit");
                    return;
                }
                DateTime commandeDtae = OrderDateDateEdit.DateTime;
                float getDicount = (float)DiscountTxtBox.Value;
                string etatPayement = EtatPaymentCbx.Text;
                string modePayement = ModePaymentCbx.Text;
                string modeAchat = AchatTypeCbx.Text;
                string commandNum = CommandeNumberTxt.Text;
                string factureNum = AchatfactureNumTxt.Text;
                float facturePriceHt = (float)AchatfacturePriceHt.Value;
                var remarque = RemarqueRichTxtBox.Text;
                float tvaValue = float.Parse(TvaComboBox.Text);
           
                CreatePurchase(getDicount, etatPayement, modePayement, modeAchat,
                    commandNum, factureNum, facturePriceHt, remarque, _provider, commandeDtae, tvaValue,
                    false);

                ResetAll();
                ShowSuccessAlert("Votre Achat a été enregistré avec succès");

            }
            catch (Exception ex)
            {
                ShowErrorAlert(ex.Message);
            }
        }

        private Purchase CreatePurchase(float getDicount, string etatPayement, string modePayement,
            string modeAchat, string commandNum, string factureNum, float facturePriceHt,
            string remarque, Provider provider, DateTime commandeDtae, float tvaValue,bool isCommand)
        {

            try
            {
                var gestionDb = new GcdbEntities();
                Provider getProvider = gestionDb.Providers.FirstOrDefault(c => c.SupplierID == provider.SupplierID);
                if (getProvider == null) return null;
                var newPurchase = new Purchase
                {
                    TvaValue = tvaValue,
                    FacturePrice = facturePriceHt,
                    PurchaseMontant = _total,
                    EtatPaiement = etatPayement,
                    ModePaiement = modePayement,
                    ModeAchat = modeAchat,
                    CommandeNum = commandNum,
                    FactureNum = factureNum,
                    PurchaseDate = AchatDateDte.DateTime.Date,
                    CommandeDate = commandeDtae,
                    Status = 0,
                    Provider = getProvider,
                    SupplierID = provider.SupplierID,
                    Description = remarque,
                    Discount = (float?)getDicount,
                    Timbre = (double?)_timbre,
                    IsCommand = isCommand
                };
                foreach (var ProdPurchase in ProductsPurchase)
                {
                    var getId = NewStockStore(ProdPurchase, tvaValue);
                    Product getProduct = gestionDb.Products.FirstOrDefault(c => c.ProductID == ProdPurchase.Product.ProductID);
                    if (getProduct == null) continue;
                    var nwStore = new PurchaseStore
                    {
                        Product = getProduct,
                        ProductID = getProduct.ProductID,
                        PurchasePrice = ProdPurchase.PurchasePrice,
                        TotalPriceAchat = ProdPurchase.TotalPriceAchat,
                        UnitsOnOrder = (double?)ProdPurchase.UnitsOnOrder,
                        VentePriceGros = ProdPurchase.VentePriceGros,
                        VentePriceDetail = ProdPurchase.VentePriceDetail,
                        VentePriceComptoire = ProdPurchase.VentePriceComptoire,
                        Discount = ProdPurchase.Discount,
                        TvaValue = tvaValue,
                        StockStoreID = getId
                    };
                    newPurchase.PurchaseStores.Add(nwStore);
                }
                gestionDb.Purchases.Add(newPurchase);
                gestionDb.SaveChanges();
                return newPurchase;
            }
            catch (Exception e)
            {
                ShowErrorAlert(e.ToString());
                return null;

            }

        }

        private int NewStockStore(PurchaseStore ProdPurchase, float tvaValue)
        {
            string stockageId = "";//StockageIDtxt.Text;
            string productSnumber = "";//ProductSNtxt.Text;
            string productState = "";//ProductStateCbx.Text;
            string refrenceNum = ""; //ReferenceTxt.Text;
            string stockObs = "";//StockObStxt.Text;
            var gestionDb = new GcdbEntities();

            var newStockStore = new StockStore
            {
                Discount = ProdPurchase.Discount,
                VentePriceGros = ProdPurchase.VentePriceGros,
                VentePriceDetail = ProdPurchase.VentePriceDetail,
                VentePriceComptoire = ProdPurchase.VentePriceComptoire,
                PurchasePrice = ProdPurchase.PurchasePrice,
                UnitsOnOrder = (double?)ProdPurchase.UnitsOnOrder,
                TvaValue = tvaValue,
                TotalPriceAchat = ProdPurchase.TotalPriceAchat,
                ProductID = ProdPurchase.Product.ProductID,
                ProductState = productState,
                Serialnumber = productSnumber,
                Observation = stockObs,
                InsertionDate = AchatDateDte.DateTime.Date,
                RefrenceNum = refrenceNum,
                StockageID = stockageId
            };
            gestionDb.StockStores.Add(newStockStore);
            gestionDb.SaveChanges();
            return newStockStore.StockStoreID;
        }


       

        //******************************************************************************************************
        private void AddProductToPurchaseBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                Product TheProduct = AllProductsDataGrid.SelectedItem as Product;

                if (TheProduct != null)
                {
                    ProductPurchaseDetails ProductToPurchaseWnd = new ProductPurchaseDetails();
                    ProductToPurchaseWnd.ValidateBtnText = "Ajouter";
                
                    PurchaseStore ProductPurchase = new PurchaseStore() { Product = TheProduct };
                    ProductToPurchaseWnd.ProductPurchase = ProductPurchase;
                    if (ProductToPurchaseWnd.ShowDialog() == true)
                    {

                        int unitsOnorder = Convert.ToInt16(ProductToPurchaseWnd.Quantite_SpinEdit.Value);
                        float purchasePrice = (float)ProductToPurchaseWnd.AchatPriceUnit_SpinEdit.Value;
                        float ventePriceGros = (float)ProductToPurchaseWnd.VentePriceUnitGros_SpinEdit.Value;
                        float ventePriceDetail = (float)ProductToPurchaseWnd.VentePriceUnitDetail_SpinEdit.Value;
                        float ventePriceComptoire = (float)ProductToPurchaseWnd.VentePriceUnitComptoire_SpinEdit.Value;

                        float discount = (float)ProductToPurchaseWnd.Discount_SpinEdit.Value;

                        var NewProductPurchase = new PurchaseStore
                        {
                            Product = TheProduct,
                            UnitsOnOrder = unitsOnorder,
                            PurchasePrice = purchasePrice,
                            VentePriceGros = ventePriceGros,
                            VentePriceDetail = ventePriceDetail,
                            VentePriceComptoire = ventePriceComptoire,
                            Discount = discount,
                            TotalPriceAchat = purchasePrice * unitsOnorder
                        };


                        ProductsPurchase.Add(NewProductPurchase);
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

        private void RemoveProductFromPurchaseBtn_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                PurchaseStore TheProductPurchase = SelectedProductsDataGrid.SelectedItem as PurchaseStore;

                if (TheProductPurchase != null)
                {
                    
                    var index = ProductsPurchase.IndexOf(TheProductPurchase);
                    ProductsPurchase.RemoveAt(index);
                    UpdateProductsDataGridsFromSources();


                    CalculateAndDisplay();
                }
            }
            catch (Exception ex)
            {
                ShowErrorAlert(ex.Message);
            }


        }

        private void SelectedProductsTableView_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            try
            {
                PurchaseStore TheProductPurchase = SelectedProductsDataGrid.SelectedItem as PurchaseStore;

                if (TheProductPurchase != null)
                {

                    ProductPurchaseDetails ProductToPurchaseWnd = new ProductPurchaseDetails();
                    ProductToPurchaseWnd.ValidateBtnText = "Modifier";
                    ProductToPurchaseWnd.ProductPurchase = TheProductPurchase;
                    if (ProductToPurchaseWnd.ShowDialog() == true)
                    {

                        var index = ProductsPurchase.IndexOf(TheProductPurchase);
                        ProductsPurchase[index] = ProductToPurchaseWnd.ProductPurchase;
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
            AllProductsDataGrid.ItemsSource = _productsList.Where(p => !IsProductsPurchaseContainProduct(p));
            AllProductsDataGrid.RefreshData();
            SelectedProductsDataGrid.ItemsSource = this.ProductsPurchase;
            SelectedProductsDataGrid.RefreshData();
        }

        bool IsProductsPurchaseContainProduct(Product TheProduct)
        {
            var result = this.ProductsPurchase.FirstOrDefault(pp => pp.Product.ProductID == TheProduct.ProductID);
            return (result!=null) ? true : false;
        }
        void ResetAll()
        {
            this.ProductsPurchase.Clear();
            SelectedProductsDataGrid.ItemsSource = this.ProductsPurchase;
            SelectedProductsDataGrid.RefreshData();
            LoadGridAllProducts();

            ProvidersLookUpEdit.SelectedItem = null;
            OrderDateDateEdit.DateTime = DateTime.Now;
            AchatDateDte.DateTime = DateTime.Now;
            CommandeNumberTxt.Clear();
            AchatfactureNumTxt.Clear();
            AchatTypeCbx.SelectedIndex = 0;
            EtatPaymentCbx.SelectedIndex = 0;
            ModePaymentCbx.SelectedIndex = 0;
            AchatfacturePriceHt.Clear();
            RemarqueRichTxtBox.Clear();

            CalculateAndDisplay();
        }

        //******************************************************************************************************
        private void AllProductsTableView_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            AddProductToPurchaseBtn_Click(null, null);
        }

        private void AllProductsDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddProductToPurchaseBtn_Click(null, null);
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

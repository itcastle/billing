using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GestionCommerciale.DomainModel.Entities;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.LayoutControl;
using GestionCommerciale.Helpers;
using GestionCommerciale.Views.ProductFolder;
using GestionCommerciale.Views.SupplierFolder;
using GestionCommerciale.Views.TVAFolder;
using GestionCommerciale.DomainModel;
using GestionCommerciale.DomainModel.Validator;

namespace GestionCommerciale.Views.PurchaseFolder
{
    /// <summary>
    /// Interaction logic for AddSale.xaml
    /// </summary>
    public partial class AddPurchaseView
    {
        private struct ProduitsSel
        {
            public String ProductName;
            public Decimal UnitsOnOrder;
            public float TotalPriceAchat;
            public float TotalPriceVente;
            public float PurchasePrice;
            public float VentePriceGros;
            public float VentePriceDetail;
            public float VentePriceComptoire;
            public float Discount;
            public int ProductId;
        }

        private readonly TabHelper _tabHlp;


        private readonly TvaClient _tvaClient;
        private readonly SuppliersManager _suppliersManager;
        private readonly ProductManger _productManger;

        private List<Product> _productsList;
        private List<ProduitsSel> _productsSelSelected;
        private List<Product> _productSelected;
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
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            _tabHlp = hlp;
            _productsSelSelected = new List<ProduitsSel>();
            //produit = new Products();
            _productManger = new ProductManger();

            _tvaClient = new TvaClient();
            TvaComboBox.ItemsSource = _tvaClient.GetTvaValues();

            _suppliersManager = new SuppliersManager();
            _productManger.ResetUnitsOnOrder();
            _productSelected = new List<Product>();
            if (string.IsNullOrEmpty(animationName)) return;
            var animation = (Storyboard) Application.Current.Resources[animationName];
            LayoutRoot.BeginStoryboard(animation);
        }


        private void GroupBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            GroupBox groupBox = (GroupBox) sender;
            groupBox.Background = Brushes.AliceBlue;
        }

        private void GroupBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var groupBox = (GroupBox) sender;
            groupBox.Background = null;
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

            if (_productsList != null)

                AllProductsDataGrid.ItemsSource = (from t in _productsList
                    select new
                    {
                        ProductName = t.ProductName,
                        CategoryName = t.SubCategory.Category.CategoryName,
                        SubCategoryName = t.SubCategory.SubCategoryName,
                        ProductID = t.ProductID
                    }
                    ).ToList();

        }


        private void LoadGridAllSuppliers()
        {
            var getProviders = new BackgroundWorker();
            getProviders.DoWork += GetAllProvidersOnDoWork;
            getProviders.RunWorkerCompleted += GetAllProvidersOnRunWorkerCompleted;
            getProviders.RunWorkerAsync();
            //*****

        }

        private void GetAllProvidersOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {

            var suppliersList = _suppliersManager.GetTheSuppliers();
            doWorkEventArgs.Result = suppliersList;
        }

        private void GetAllProvidersOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs workerCompleted)
        {
            _suppliersList = workerCompleted.Result as List<Provider>;
            if (_suppliersList == null) return;
            ProviderGridControl.ItemsSource = (from t in _suppliersList
                select new
                {
                    CompanyName = t.CompanyName,
                    ContactName = t.ContactName,
                    ContactTitle = t.ContactTitle
                }
                ).ToList();
        }

        private void LoadGridProductsSelected()
        {

         Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_productsSelSelected != null)
                    SelectedProductsDataGrid.ItemsSource = (from t in _productsSelSelected
                        select new
                        {
                            ProductName = t.ProductName,
                            PurchasePrice = t.PurchasePrice,
                            VentePriceGros = t.VentePriceGros,
                            ventePriceDetail = t.VentePriceDetail,
                            ventePriceComptoire = t.VentePriceComptoire,
                            TotalPriceAchat = t.TotalPriceAchat,
                            TotalPriceVente = t.TotalPriceVente,
                            UnitsOnOrder = t.UnitsOnOrder,
                            ProductID = t.ProductId
                        }
                        ).ToList();
                SelectedProductsTableView.AutoWidth = true;
            }));
        }

        private void Display()
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
            _tva = 0;

            foreach (ProduitsSel entity in _productsSelSelected)
            {
                _total += entity.TotalPriceAchat; // a refaire en cas de comande distribuée
            }
            _tva = GetTva();
            _discount = GetDiscount()*_total;

            _timbre = SetTimbre(_total);

            _ttc = _total - _discount + (_total*_tva) + _timbre;
        }

        private float SetTimbre(float total)
        {
            if (AchatTypeCbx.Text != "Facture") return 0;
            _timbre = total / 100;
            if (_timbre > 2500) _timbre = 2500;
            return _timbre;
        }

        private float GetDiscount()
        {
            decimal result = ConvertToDecimal(DiscountTxtBox.Text);
            if (result == 0) return 0;
            return (float) (result/100);

        }

        private float GetTva()
        {
            try
            {
                return (float) Convert.ToDecimal(TvaComboBox.Text);
            }
            catch (Exception)
            {
                return (float) Convert.ToDecimal(0.17); //s
            }

        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            DXMessageBox.Show(this, Enregistrer(), "ITcastle Solutions");
        }

        private String Enregistrer()
        {
            try
            {

                //   Employee employeeEntity = WhoEmployee.Employee; 
                _provider = GetProvider();
                if (_provider == null)
                {
                    DXMessageBox.Show(this, "Vous devez choisir un Fournissuer");
                    return "pas bien";
                }
                if (_productsSelSelected.Count == 0)
                {
                    DXMessageBox.Show(this, "Vous devez choisir au moin un produit");
                    return "Achat na pas été enregistrer";
                }
                DateTime commandeDtae = OrderDateDateEdit.DateTime;
                float getDicount = GetDiscount();
                string etatPayement = EtatPaymentCbx.Text;
                string modePayement = ModePaymentCbx.Text;
                string modeAchat = AchatTypeCbx.Text;
                string commandNum = CommandeNumberTxt.Text;
                string factureNum = AchatfactureNumTxt.Text;
                float facturePriceHt = ConvertToFloat(AchatfacturePriceHt.Text);
                var remarque = RemarqueRichTxtBox.Text;
                float tvaValue = ConvertToFloat(TvaComboBox.Text);
                //...



                //**************
                CreatePurchase(getDicount, etatPayement, modePayement, modeAchat,
                    commandNum, factureNum, facturePriceHt, remarque, _provider, commandeDtae, tvaValue);


                ClearAfterSave();
                return "Votre Achat a été bien enregistrer";
            }
            catch (Exception)
            {
                return "Erreur";
            }
        }
        private float ConvertToFloat(string stringVal)
        {
            try
            {
                decimal decimalVal = Convert.ToDecimal(stringVal);
                return (float) decimalVal;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        private void ClearAfterSave()
        {

            _productSelected.Clear();
            _productsSelSelected.Clear();

            RemarqueRichTxtBox.Clear();
            OrderDateDateEdit.Clear();
            ClearFields();
            LoadGridAllProducts();
        }



        private void CreatePurchase(float getDicount, string etatPayement, string modePayement,
            string modeAchat, string commandNum, string factureNum, float facturePriceHt,
            string remarque, Provider provider, DateTime commandeDtae, float tvaValue)
        {

            try
            {
                var gestionDb = new GcdbEntities();
                Provider getProvider = gestionDb.Providers.FirstOrDefault(c => c.SupplierID == provider.SupplierID);
                if(getProvider==null)return;
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
                    Discount = (float?) getDicount,
                    Timbre = (double?) _timbre
                };
                foreach (var entitys in _productsSelSelected)
                {
                    var getId = Getid(entitys, tvaValue);
                    Product getProduct = gestionDb.Products.FirstOrDefault(c => c.ProductID == entitys.ProductId);
                    if(getProduct==null)continue;
                    var nwStore = new PurchaseStore
                    {
                        Product = getProduct,
                        ProductID = entitys.ProductId,
                        PurchasePrice = entitys.PurchasePrice,
                        TotalPriceAchat = entitys.TotalPriceAchat,
                        UnitsOnOrder = (double?) entitys.UnitsOnOrder,
                        VentePriceGros = entitys.VentePriceGros,
                        VentePriceDetail = entitys.VentePriceDetail,
                        VentePriceComptoire = entitys.VentePriceComptoire,
                        Discount = entitys.Discount,
                        TvaValue = tvaValue,
                        StockStoreID = getId
                    };
                    newPurchase.PurchaseStores.Add(nwStore);
                }
                gestionDb.Purchases.Add(newPurchase);
                gestionDb.SaveChanges();
            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString());
            }

        }

        private int Getid(ProduitsSel entitys, float tvaValue)
        {
            string stockageId = StockageIDtxt.Text;
            string productSnumber = ProductSNtxt.Text;
            string productState = ProductStateCbx.Text;
            string refrenceNum = ReferenceTxt.Text;
            string stockObs = StockObStxt.Text;
            var gestionDb = new GcdbEntities();

            var newStockStore = new StockStore
            {
                Discount = entitys.Discount,
                VentePriceGros = entitys.VentePriceGros,
                VentePriceDetail = entitys.VentePriceDetail,
                VentePriceComptoire = entitys.VentePriceComptoire,
                PurchasePrice = entitys.PurchasePrice,
                UnitsOnOrder = (double?) entitys.UnitsOnOrder,
                TvaValue = tvaValue,
                TotalPriceAchat = entitys.TotalPriceAchat,
                ProductID = entitys.ProductId,
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


        private Provider GetProvider()
        {
            if (ProviderGridControl.VisibleRowCount == 0) return null;
            int rowHandle = ProviderGridControl.View.FocusedRowHandle;
            String companyName = ProviderGridControl.GetCellValue(rowHandle, "CompanyName").ToString();
            _provider = _suppliersManager.GetProviderByName(companyName);
            return _provider;

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (SettingsClient.IsNotLegal()) Application.Current.Shutdown();
            OrderDateDateEdit.DateTime = DateTime.Now;
            LoadGridAllSuppliers();
            LoadGridAllProducts();
            TvaComboBox.ItemsSource = _tvaClient.GetTvaValues();

        }

        private void AddProductToSaleBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (AllProductsDataGrid.VisibleRowCount == 0) return;
            var rowHandle = AllProductsDataGrid.View.FocusedRowHandle;
            if (rowHandle < 0) return;


            int productId = (int) AllProductsDataGrid.GetCellValue(rowHandle, "ProductID");

            var getProduit = _productManger.GetProductById(productId);

            if (getProduit == null) return;
            if (String.IsNullOrEmpty(QuantiteSaleTxtBox.Text) || !Validator.IsNumberValid(QuantiteSaleTxtBox.Text))
            {
                DXMessageBox.Show(this, "Vous devez saisir une quantité ");
                QuantiteSaleTxtBox.Text = "";
                return;
            }

            int unitsOnorder = Convert.ToInt16(QuantiteSaleTxtBox.Text);
            float purchasePrice = ConvertToFloat(AchatPriceUnitTxt.Text);
            float ventePriceGros = ConvertToFloat(VentePriceUnitGrosTxt.Text);
            float ventePriceDetail = ConvertToFloat(VentePriceUnitDetailTxt.Text);
            float ventePriceComptoire = ConvertToFloat(VentePriceUnitComptoireTxt.Text);

            float discount = ConvertToFloat(DiscountTxt.Text);

            var prodSel = new ProduitsSel
            {
                ProductId = getProduit.ProductID,
                ProductName = getProduit.ProductName,
                UnitsOnOrder = unitsOnorder,
                PurchasePrice = purchasePrice,
                VentePriceGros = ventePriceGros,
                VentePriceDetail = ventePriceDetail,
                VentePriceComptoire = ventePriceComptoire,
                Discount = discount,
                TotalPriceVente = ventePriceGros*unitsOnorder,
                TotalPriceAchat = purchasePrice*unitsOnorder
            };


            var exist = _productSelected.Exists(item => item.ProductID == getProduit.ProductID);

            //  int index = _productSelected.IndexOf(getProduit);
            if (!exist)
            {
                _productSelected.Add(getProduit);
                _productsSelSelected.Add(prodSel);
            }
            else
            {
                UpdateFunction(getProduit, unitsOnorder, purchasePrice, discount);

            }
            ClearFields();
        }

        private void UpdateFunction(Product getProduit, int unitsOnorder, float purchasePrice, float discount)
        {
            try
            {
                bool exist = _productsSelSelected.Exists(item => item.ProductId == getProduit.ProductID);
                if (!exist) return;
                var returnProduct = _productsSelSelected.Find(item => item.ProductId == getProduit.ProductID);
                var prodSel = new ProduitsSel
                {
                    ProductId = getProduit.ProductID,
                    ProductName = getProduit.ProductName,
                    UnitsOnOrder = unitsOnorder + returnProduct.UnitsOnOrder,
                    PurchasePrice = purchasePrice + returnProduct.PurchasePrice,
                    Discount = discount,
                    VentePriceGros = returnProduct.VentePriceGros,
                    VentePriceDetail = returnProduct.VentePriceDetail,
                    VentePriceComptoire = returnProduct.VentePriceComptoire,
                    TotalPriceVente = returnProduct.VentePriceGros*(float) (unitsOnorder + returnProduct.UnitsOnOrder),
                    TotalPriceAchat = purchasePrice*(float) (unitsOnorder + returnProduct.UnitsOnOrder)
                };

                var index = _productsSelSelected.IndexOf(returnProduct);
                _productsSelSelected.RemoveAt(index);
                _productsSelSelected.Add(prodSel);
            }
            catch (Exception)
            {
                //
            }
        }

        private void RemoveProductFromSaleBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {

            try
            {
                if (SelectedProductsDataGrid.VisibleRowCount == 0) return;
                var rowHandle = SelectedProductsDataGrid.View.FocusedRowHandle;
                if (rowHandle < 0) return;
                int productId = (int) SelectedProductsDataGrid.GetCellValue(rowHandle, "ProductID");
                var getProduit = _productManger.GetProductById(productId);
                if (getProduit == null) return;

                decimal unitsOnOrder = (decimal)SelectedProductsDataGrid.GetCellValue(rowHandle, "UnitsOnOrder");
                float purchasePrice = (float)SelectedProductsDataGrid.GetCellValue(rowHandle, "PurchasePrice");
                float ventePriceGros = (float)SelectedProductsDataGrid.GetCellValue(rowHandle, "VentePriceGros");
                float ventePriceDetail =
                    (float)SelectedProductsDataGrid.GetCellValue(rowHandle, "VentePriceDetail");
                float ventePriceComptoire =
                    (float)SelectedProductsDataGrid.GetCellValue(rowHandle, "VentePriceComptoire");
                float discount = (float)SelectedProductsDataGrid.GetCellValue(rowHandle, "Discounts");
                var prodSel = new ProduitsSel
                {
                    ProductId = getProduit.ProductID,
                    ProductName = getProduit.ProductName,
                    UnitsOnOrder = unitsOnOrder,
                    PurchasePrice = purchasePrice,
                    VentePriceGros = ventePriceGros,
                    VentePriceDetail = ventePriceDetail,
                    Discount = discount,
                    VentePriceComptoire = ventePriceComptoire,
                    TotalPriceVente = ventePriceGros*(float) unitsOnOrder,
                    TotalPriceAchat = purchasePrice*(float) unitsOnOrder
                };

                var index = _productsSelSelected.IndexOf(prodSel);
                _productsSelSelected.RemoveAt(index);


                _productSelected.Remove(getProduit);
                ClearFields();


            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        private void ClearFields()
        {
            Calculate();
            Display();
            LoadGridProductsSelected();
            QuantiteSaleTxtBox.Clear();
            ProductNameTxtBox.Clear();
            CategoryTxt.Clear();
            SubCategoryTxt.Clear();
            AchatPriceUnitTxt.Clear();
            VentePriceUnitGrosTxt.Clear();
            VentePriceUnitDetailTxt.Clear();
            VentePriceUnitComptoireTxt.Clear();
            AchatTotalHTtxt.Clear();
        }

        private decimal ConvertToDecimal(string stringVal)
        {
            try
            {
                decimal decimalVal = Convert.ToDecimal(stringVal);
                return decimalVal;
            }
            catch (Exception)
            {
                return 0;
            }
        }


        private void AddTvaBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var addTva = new AddTvaView();
            addTva.ShowDialog();
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }


        private void AddSupplierBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = _tabHlp.AddNewTab(typeof (AddSupplierView), "Nouveau fournisseur ", "FadeToLeftAnim");
        }

        private void AddProductBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = _tabHlp.AddNewTab(typeof (AddProductView), "Nouveau produit ", "FadeToLeftAnim");
        }

        private void DiscountTxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Calculate();
            Display();
        }

        private void TvaComboBox_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            Calculate();
            Display();
        }

        //
        private void SelectedProductsDataGrid_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName != "VentePrice") return;
            e.DisplayText = string.Format("{0:n} Dinars", e.Value);
        }

        private void AllProductsTableView_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (AllProductsDataGrid.VisibleRowCount == 0) return;
                var rowHandle = AllProductsDataGrid.View.FocusedRowHandle;
                if (rowHandle < 0) return;

                int productId = (int) AllProductsDataGrid.GetCellValue(rowHandle, "ProductID");

                var getProduit = _productManger.GetProductById(productId);

                if (getProduit == null)
                {
                    ProductNameTxtBox.Clear();
                    CategoryTxt.Clear();
                    SubCategoryTxt.Clear();
                    return;
                }
                ProductNameTxtBox.Text = getProduit.ProductName;
                CategoryTxt.Text = getProduit.SubCategory.Category.CategoryName;
                SubCategoryTxt.Text = getProduit.SubCategory.SubCategoryName;
            }
            catch (Exception)
            {
                // 
            }

        }

        private void AchatPriceUnitTxt_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {

            AutoCalculateTotalByProduct();
        }

        private void AutoCalculateTotalByProduct()
        {

            decimal salePrice = ConvertToDecimal(AchatPriceUnitTxt.Text);
            decimal qte = ConvertToDecimal(QuantiteSaleTxtBox.Text);
            AchatTotalHTtxt.Text = (salePrice*qte).ToString("###,###.00");
        }



        private void AchatTotalHTtxt_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            try
            {
                decimal getProductDiscount = ConvertToDecimal(DiscountTxt.Text);
                decimal getTotalByProduct = ConvertToDecimal(AchatTotalHTtxt.Text);
                decimal result = getTotalByProduct - (getTotalByProduct*(getProductDiscount/100));
                AfterDiscountTxtBox.Text = result.ToString("###,###.00");
            }
            catch (Exception)
            {
                //
            }

        }

        private void DiscountTxt_OnLostFocus(object sender, RoutedEventArgs e)
        {
            decimal result = ConvertToDecimal(DiscountTxt.Text);
            if (result == 0)
            {

            }
            else
            {

                decimal salePrice = ConvertToDecimal(AchatPriceUnitTxt.Text);
                decimal qte = ConvertToDecimal(QuantiteSaleTxtBox.Text);
                AchatTotalHTtxt.Text = ((result/100)*salePrice*qte).ToString("###,###.00");
            }

        }


        private void AchatTypeCbx_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {

           Calculate();
            Display();
        }
    }
}

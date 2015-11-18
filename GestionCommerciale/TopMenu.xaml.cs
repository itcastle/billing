using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using GestionCommerciale.DomainModel;
using GestionCommerciale.Helpers;
using GestionCommerciale.Views;
using GestionCommerciale.Views.Customers;
using GestionCommerciale.Views.Employees;
using GestionCommerciale.Views.Options;
using GestionCommerciale.Views.Products;
using GestionCommerciale.Views.Purchases;
using GestionCommerciale.Views.Sales;
using GestionCommerciale.Views.Statistics;
using GestionCommerciale.Views.Suppliers;

namespace GestionCommerciale
{
	/// <summary>
	/// Interaction logic for TopMenu.xaml
	/// </summary>
	public partial class TopMenu
	{
		private readonly TabHelper _tabHlp;
		private readonly Image _header;
		private readonly Window _mainWnd;

		private bool _listB;
		private bool _nouveauB;

		private Storyboard _nouveauOpenAnim;
		private Storyboard _nouveauCloseAnim;
		private Storyboard _listesOpenAnim;
		private Storyboard _listesCloseAnim;

		
		public TopMenu(TabHelper tabhlp,Window mainwnd,Image hdr)
		{
			this.InitializeComponent();
			_tabHlp = tabhlp;
			_header = hdr;
			_mainWnd = mainwnd;
		}



		private void TopMenuUserControl_Loaded(object sender, RoutedEventArgs e)
		{
			_nouveauOpenAnim = (Storyboard)Resources["NouveauSbMnOpen"];
			_nouveauCloseAnim = (Storyboard)Resources["NouveauSbMnClose"];

			_listesOpenAnim = (Storyboard)Resources["ListesSbMnOpen"];
			_listesCloseAnim = (Storyboard)Resources["ListesSbMnClose"];
			
		}


		private void OptionsBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
		  
		
			_tabHlp.AddNewTab(typeof(Options), "Option et paramètres", _mainWnd,_header, "FadeToLeftAnim");
		}



	

		private void AjouterUnBtn_MouseEnter(object sender, MouseEventArgs e)
		{
			_nouveauOpenAnim.Begin();
			_nouveauB = true;
		}
		private void AjouterUnBtn_MouseLeave(object sender, MouseEventArgs e)
		{
			_nouveauCloseAnim.Begin();
		}

		private void NouveauSubMenuGrid_MouseEnter(object sender, MouseEventArgs e)
		{
			if (_nouveauB)
			{
				_nouveauCloseAnim.Stop();
				
			}
		}

		private void NouveauSubMenuGrid_MouseLeave(object sender, MouseEventArgs e)
		{
			if (_nouveauB)
			{
				_nouveauCloseAnim.Begin();
				_nouveauB = false;
			}
		}

		



		private void ListesBtn_MouseEnter(object sender, MouseEventArgs e)
		{
			_listesOpenAnim.Begin();
			_listB = true;
		}
		private void ListesBtn_MouseLeave(object sender, MouseEventArgs e)
		{
			_listesCloseAnim.Begin();
		}

		private void ListesSubMenuGrid_MouseEnter(object sender, MouseEventArgs e)
		{
			if (_listB)
			{
				_listesCloseAnim.Stop();
			  
			}
		}

		private void ListesSubMenuGrid_MouseLeave(object sender, MouseEventArgs e)
		{
			if (_listB)
			{
				_listesCloseAnim.Begin();
				_listB = false;
			}
			
		}

	

		private void AddProductBtn_Click(object sender, RoutedEventArgs e)
		{
			var item = _tabHlp.AddNewTab(typeof(AddProductView), "Nouveau produit ", "FadeToLeftAnim");
			
		}

		private void AddPurchaseBtn_Click(object sender, EventArgs e)
		{
			var item = _tabHlp.AddNewTab(typeof(AddPurchaseView), "Effectuer un achat ", "FadeToLeftAnim", _tabHlp);
		}

	  

		private void AddSaleBtn_Click(object sender, RoutedEventArgs e)
		{
			var item = _tabHlp.AddNewTab(typeof(NewSaleView), "Effectuer une vente ", "FadeToLeftAnim", _tabHlp);
		}

		private void AddSupplierBtn_click(object sender, RoutedEventArgs e)
		{
			var item = _tabHlp.AddNewTab(typeof(AddSupplierView), "Nouveau fournisseur ", "FadeToLeftAnim");

		}

		private void AddCustomerBtn_Click(object sender, RoutedEventArgs e)
		{
		   var item = _tabHlp.AddNewTab(typeof(AddCustomerView), "Nouveau client ", "FadeToLeftAnim");

		}

		private void AddEmployeeBtn_Click(object sender, RoutedEventArgs e)
		{
			if (!WhoEmployee.IsAdministrator) return;
			var item = _tabHlp.AddNewTab(typeof(AddEmployeeView), "Nouveau employée ", "FadeToLeftAnim");

		}

		private void ListProductsBtn_Click(object sender, RoutedEventArgs e)
		{
			var item = _tabHlp.AddNewTab(typeof(ListProductsView), "Mes produits ", "FadeToLeftAnim", _tabHlp);
		}

		private void ListPurchasesBtn_Click(object sender, RoutedEventArgs e)
		{
            var item = _tabHlp.AddNewTab(typeof(ListSalesView), "Mes achats ", "FadeToLeftAnim", _tabHlp);

		}

		private void ListSalesBtn_Click(object sender, RoutedEventArgs e)
		{
			var item = _tabHlp.AddNewTab(typeof(ListPurchasesView), "Mes ventes ", "FadeToLeftAnim", _tabHlp);

		}

		private void ListSupplierBtn_Click(object sender, RoutedEventArgs e)
		{
			var item = _tabHlp.AddNewTab(typeof(ListSuppliersView), "Mes fournisseurs ", "FadeToLeftAnim", _tabHlp);

		}

		private void ListCustomersBtn_Click(object sender, RoutedEventArgs e)
		{
			var item = _tabHlp.AddNewTab(typeof(ListCustomersView), "Mes clients ", "FadeToLeftAnim", _tabHlp);

		}

		private void ListEmployeesBtn_Click(object sender, RoutedEventArgs e)
		{
			if (!WhoEmployee.IsAdministrator) return;
			var item = _tabHlp.AddNewTab(typeof(ListEmployeesView), "Mes employées ", "FadeToLeftAnim", _tabHlp);

		}

			
	

		private void StatistiqueBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_tabHlp.AddNewTab(typeof(StatisticsView), "Statistiques générale ", "FadeToLeftAnim");
		}

		

	   
	}
}
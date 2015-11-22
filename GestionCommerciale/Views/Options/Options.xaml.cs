using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace GestionCommerciale.Views.Options
{
	/// <summary>
	/// Interaction logic for Option.xaml
	/// </summary>
	public partial class Options
	{
		private readonly Image _headerImage;
		private readonly Window _mainWnd;
		public Options(Window mainwnd,Image header,string animationName="")
		{
		    InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
			_headerImage = header;
			_mainWnd = mainwnd;

		    if (string.IsNullOrEmpty(animationName)) return;
		    Storyboard animation = (Storyboard)Application.Current.Resources[animationName];
		    LayoutRoot.BeginStoryboard(animation);
		}

		

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{

			GeneralParams gnrlPrms = new GeneralParams();
			options_layout.Children.Clear();
			options_layout.Children.Add(gnrlPrms);
			
		}

		private void BackstageTabItem_Click(object sender, EventArgs e)
		{
			var intrfPref = new InterfacePreference(_mainWnd, _headerImage);
			options_layout.Children.Clear();
			options_layout.Children.Add(intrfPref);
		}

		private void backstageTabItem1_Click(object sender, EventArgs e)
		{
			var gnrlPrms = new GeneralParams();
			options_layout.Children.Clear();
			options_layout.Children.Add(gnrlPrms);
		}
	}
}
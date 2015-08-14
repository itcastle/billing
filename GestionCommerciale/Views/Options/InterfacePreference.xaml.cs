using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core;

namespace GestionCommerciale.Views.Options
{
	/// <summary>
	/// Interaction logic for changer_theme.xaml
	/// </summary>
	public partial class InterfacePreference : UserControl
	{
		private readonly Image _headerImage;

		private readonly Window _mainWindow;
		public InterfacePreference(Window mainwnd, Image header)
		{
			InitializeComponent();
			_mainWindow = mainwnd;
			_headerImage = header;
		}


	

		private void seven_theme_btn_Selected(object sender, RoutedEventArgs e)
		{
			ThemeManager.SetTheme(_mainWindow, Theme.Seven);
			HelpFunctions.AppTheme = Theme.Seven;
		}

		private void deepblue_theme_btn_Selected(object sender, RoutedEventArgs e)
		{
			ThemeManager.SetTheme(_mainWindow, Theme.DeepBlue);
			HelpFunctions.AppTheme = Theme.DeepBlue;
		}

		private void lightgray_theme_btn_Selected(object sender, RoutedEventArgs e)
		{
			ThemeManager.SetTheme(_mainWindow, Theme.LightGray);
			HelpFunctions.AppTheme = Theme.LightGray;
		}

		private void office2007blue_theme_btn_Selected(object sender, RoutedEventArgs e)
		{
			//try
			//{
			//    ThemeManager.SetTheme(mainWindow, Theme.Office2007Blue);
			//    HelpFunctions.AppTheme = Theme.Office2007Blue;
			//}
			//catch (Exception exception)
			//{
				

			//}
		}

		private void office2007black_theme_btn_Selected(object sender, RoutedEventArgs e)
		{
			ThemeManager.SetTheme(_mainWindow, Theme.Office2007Black);
			HelpFunctions.AppTheme = Theme.Office2007Black;
		}

		private void office2007silver_theme_btn_Selected(object sender, RoutedEventArgs e)
		{
			ThemeManager.SetTheme(_mainWindow, Theme.Office2007Silver);
			HelpFunctions.AppTheme = Theme.Office2007Silver;
		}

		private void office2010black_theme_btn_Selected(object sender, RoutedEventArgs e)
		{
			ThemeManager.SetTheme(_mainWindow, Theme.Office2010Black);
			HelpFunctions.AppTheme = Theme.Office2010Black;
			
		}



	

		private void header_color_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			ComboBoxItem item = (ComboBoxItem)header_color_combobox.SelectedItem;
			
			var img = "";
			string content = item.Content.ToString();
			switch (content)
			{
				case "Noir": img = "header_black.jpg"; break;
				case "Blue": img = "header_blue.jpg"; break;
				case "Gris": img = "header_gray.jpg"; break;
				case "Vert": img = "header_green.jpg"; break;
				case "Rose": img = "header_pink.jpg"; break;
				case "Rouge": img = "header_red.jpg"; break;
				case "Blue ciel": img = "header_skyblue.jpg"; break;

			}


		  
			
			ImageSourceConverter imgConv = new ImageSourceConverter();
			string path = "pack://application:,,/Images/Headers/" + img;
			ImageSource imageSource = (ImageSource) imgConv.ConvertFromString(path);
			_headerImage.Source = imageSource;


			


			
		}




	 
	}
}
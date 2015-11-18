using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using GestionCommerciale.Helpers;

namespace GestionCommerciale.Views.Options
{
    /// <summary>
    /// Interaction logic for changer_theme.xaml
    /// </summary>
    public partial class InterfacePreference
    {
        private readonly Image _headerImage;

        private readonly Window _mainWindow;
        public InterfacePreference(Window mainwnd, Image header)
        {
            InitializeComponent();
            _mainWindow = mainwnd;
            _headerImage = header;
            

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ThemesListbox.ItemsSource = Theme.Themes;
                string searchFolder = @".\Headers";
                var filters = new string[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp" };
                var filesNames = Helper.GetFilesFrom(searchFolder, filters, false);
                var files = Helper.GetFilesinfosFromFilesnames(filesNames);
                HeadersCombobox.ItemsSource = files;
            }
            catch (Exception ex)
            {
                Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }
        }

        private void ThemesListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Theme SelectedTheme = ThemesListbox.SelectedItem as Theme;
                if (SelectedTheme != null)
                {
                    ThemeManager.ApplicationThemeName = SelectedTheme.Name;
                   
                }
            }
            catch (Exception ex)
            {
                Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }
        }

        private void HeadersCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                FileInfo HeaderFile = HeadersCombobox.SelectedItem as FileInfo;
                if (HeaderFile != null)
                {
                    _headerImage.Source = new BitmapImage(new Uri(HeaderFile.FullName));
                }
            }
            catch (Exception ex)
            {
                Helper.ShowErrorMessageBox("Erreur", ex.Message);
            }

        }

       
    }
    public class FileinfonameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string FileName = value as string;
            if (FileName != null)
            {
                FileName = FileName.Replace('_', ' ');
                FileName = FileName.Replace('-', ' ');
                var FileNameSplited = FileName.Split('.');
                if (FileNameSplited.Length > 0) return FileNameSplited[0];
                else return FileName;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }


}
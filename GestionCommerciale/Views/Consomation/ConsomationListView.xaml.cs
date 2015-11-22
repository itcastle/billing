using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;

namespace GestionCommerciale.Views.Consomation
{
    /// <summary>
    /// Interaction logic for ConsomationListView.xaml
    /// </summary>
    public partial class ConsomationListView
    {
        public ConsomationListView()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
         
        }
        public ConsomationListView(string animationName)
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            if (string.IsNullOrEmpty(animationName)) return;
            var animation = (Storyboard)Application.Current.Resources[animationName];
            LayoutRoot.BeginStoryboard(animation);
        }
    }
}

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
         
        }
        public ConsomationListView(string animationName)
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(animationName)) return;
            var animation = (Storyboard)Application.Current.Resources[animationName];
            LayoutRoot.BeginStoryboard(animation);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using GestionCommerciale.Helpers;

namespace GestionCommerciale.Modals
{
    /// <summary>
    /// Interaction logic for MessageBx.xaml
    /// </summary>
    public partial class MessageBx : Window
    {
        private string _message = null;
        public MessageBx()
        {
            InitializeComponent();
            this.Loaded+=new RoutedEventHandler(MessageBx_Loaded);
                              
        }
        void MessageBx_Loaded(object sender, EventArgs e)
        {
            ThemeManager.SetTheme(this, HelpFunctions.AppTheme);
            ThemeManager.SetTheme(btnOK, HelpFunctions.AppTheme);
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                lblMessage.Content = _message;
            }
        }
    }
}

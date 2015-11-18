using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using GestionCommerciale.Views.ServiceFolder;

namespace GestionCommerciale.Views
    {

    public partial class NewModelNameWindow
    {

        private readonly UserControl _theChild;
        private readonly string _theViewType;
        private readonly string _theOldModelName;

        public NewModelNameWindow(UserControl child, string modelName, string viewType)
            {
            InitializeComponent();
            _theChild = child;
            _theOldModelName = modelName;
            ModelNameTxtBox.Text = modelName;
            _theViewType = viewType;
            }

        private void Button_OnClick(object sender, RoutedEventArgs e)
            {
            switch (_theViewType)
                {
              
                case "ServiceModel":
                        {

                            var serviceManager = new DomainModel.ClassesClients.ServiceManager();
                       
                            if ((!serviceManager.IsServiceModelNameExist(ModelNameTxtBox.Text) || ModelNameTxtBox.Text == _theOldModelName) &&
                                !string.IsNullOrWhiteSpace(ModelNameTxtBox.Text))
                            {
                                
                                var leServiceView = (ServiceView)_theChild;
                                leServiceView.ServiceModelName = ModelNameTxtBox.Text;
                                leServiceView.SaveServiceModel(ModelNameTxtBox.Text);
                                leServiceView.ExitEditModel = true;
                                Close();
                            }
                            else if (string.IsNullOrWhiteSpace(ModelNameTxtBox.Text))
                                DXMessageBox.Show("Le nom du modèle ne peut pas être vide !", "Erreur !", MessageBoxButton.OK,
                                                MessageBoxImage.Error);
                            else
                                DXMessageBox.Show("Ce nom du modèle existe déjà !", "Erreur !", MessageBoxButton.OK,
                                                MessageBoxImage.Error);
                            break;
                        }

            

                
           
       
             
                }
            }

        private void ButtonClick2(object sender, RoutedEventArgs e)
            {
           
            }

        private void Window_OnLoaded(object sender, RoutedEventArgs e)
            {
            ModelNameTxtBox.Focus();
            }

        public void Connect(int connectionId, object target)
        {
            

        }
        }
    }

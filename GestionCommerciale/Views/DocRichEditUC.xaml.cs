using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using DevExpress.Xpf.RichEdit.Controls.Internal;
using DevExpress.Xpf.RichEdit.UI;
using GestionCommerciale.Helpers;

namespace GestionCommerciale.Views
{
    /// <summary>
    /// Interaction logic for DocRichEditUc.xaml
    /// </summary>
    public partial class DocRichEditUC
    {
        public object DocumentContent
        {
            get { return (object)this.GetValue(DocumentContentProperty); }
            set { this.SetValue(DocumentContentProperty, value); }
        }
        public static readonly DependencyProperty DocumentContentProperty = DependencyProperty.Register(
          "DocumentContent", typeof(object), typeof(DocRichEditUC), new PropertyMetadata());


        public Dictionary<string,object> DocumentVariables
        {
            get { return (Dictionary<string, object>)this.GetValue(DocumentVariablesProperty); }
            set 
            { 
                this.SetValue(DocumentVariablesProperty, value);
                UpdateDocumentVariables();
            }
        }
        public static readonly DependencyProperty DocumentVariablesProperty = DependencyProperty.Register(
          "DocumentVariables", typeof(Dictionary<string, object>), typeof(DocRichEditUC));

        public DocRichEditUC()
        {
            InitializeComponent();
            
        }

        
        public void UpdateDocumentVariables()
        {
            if (DocumentVariables != null)
            {
                DocRichEdit.Document.Variables.Clear();
                foreach (var v in DocumentVariables)
                {
                    DocRichEdit.Document.Variables.Add(v.Key, v.Value);
                }
                DocRichEdit.Document.Fields.Update();
            }
        }

        public void DocumentVariablesProperty_Changed(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {

        }

        private Window _UserControlOwnerWindow;
        private RichEditSearchPanel _SearchPanel;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           // _UserControlOwnerWindow = Helper.GetTopParent(this) as Window;
            _SearchPanel = Helper.FindVisualChildByName<RichEditSearchPanel>(DocRichEdit, "SearchPanel");
          
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TopMenu_RibbonControl.Opacity == 0)
            {
                var ShowRibbonControlAnim = (Storyboard)this.Resources["TopMenuRibbonControl_ShowAnim"];
                TopMenu_RibbonControl.BeginStoryboard(ShowRibbonControlAnim);
            }
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TopMenu_RibbonControl.Opacity == 1 && _UserControlOwnerWindow.IsActive)
            {
                var focusedControl = FocusManager.GetFocusedElement(_UserControlOwnerWindow) as DependencyObject;
                if (focusedControl != null && focusedControl.GetType() != typeof(KeyCodeConverter) 
                    && _SearchPanel !=null && !_SearchPanel.IsVisible)//context menu , search panel
                {
                            var HideRibbonControlAnim = (Storyboard)this.Resources["TopMenuRibbonControl_HideAnim"];
                            TopMenu_RibbonControl.BeginStoryboard(HideRibbonControlAnim);
                   
                    

                }
            }
        }

        private void DocRichEdit_ContentChanged(object sender, EventArgs e)
        {

        }

        private void DocRichEdit_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }

        

       


        
    }

}

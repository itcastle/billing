using System;
using System.Windows.Controls;
using DevExpress.Utils;
using DevExpress.Xpf.Core;

namespace GestionCommerciale.Helpers
{
    public class TabHelper
    {
       

        private DXTabControl TabControl
        {
            get;
            set;
        }

        public TabHelper()
        {
            TabControl = null;
        }
        public TabHelper(DXTabControl tab)
        {
            TabControl = tab;
        }
        public DXTabItem AddNewTab(Type type, string header, params object[] ConstructArgs)
        {
            foreach (DXTabItem tabItem in TabControl.Items)
            {
                if ((string) tabItem.Header != header) continue;
                TabControl.SelectedItem = tabItem;
                return tabItem;
            }
         
           
            UserControl cntr  = (UserControl)Activator.CreateInstance(type, ConstructArgs);

            DXTabItem tabitem = new DXTabItem { Header = header, AllowHide =DefaultBoolean.True };
            TabControl.Items.Add(tabitem);
            tabitem.Content = cntr;
            TabControl.SelectedItem = tabitem;
            return tabitem;

        }


    }
}

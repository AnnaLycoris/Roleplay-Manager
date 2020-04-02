using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RoleplayManager_Client.UI_Resources {
    public partial class PluginButton:UserControl {
        public UserControl pluginFrame { get; }
        public PluginButton(string pluginName, UserControl uc) {
            InitializeComponent();
            TB_Caption.Text = pluginName;
            pluginFrame = uc;
        }

        public void AddPluginEventHandler(RoutedEventHandler handler) {
            Btn_Clickable.Click += handler;
            MainWindow.WriteChatMessage("Button clicked: " + TB_Caption.Text);
        }

        public void AddPluginControlToPluginContainer(object sender, RoutedEventArgs e) {
            MainWindow.mWindow.AddPluginFrameToGrid(pluginFrame);
        }
    }
}

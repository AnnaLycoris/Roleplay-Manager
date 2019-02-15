using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RoleplayManager_Server {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:Window {

        public static MainWindow mWindow;
        private Net.TCPServer Server;

        
        public MainWindow() {
            mWindow = this;
            InitializeComponent();
            Net.ServerNetworkDataHandler.InitializeNetworkPackets();
        }

        private void Button_Click(object sender,RoutedEventArgs e) {
            Net.TCPServer.StartServer(int.Parse(PortBox.Text));
        }

        public static void WriteChatMessage(string msg) {
            mWindow.Dispatcher.Invoke(() => {
                mWindow.ChatBox.AppendText("\n" + msg);
                mWindow.ChatBox.ScrollToEnd();
            });
        }
    }
}

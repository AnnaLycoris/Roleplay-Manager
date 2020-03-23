using System.Windows;
using System.Windows.Input;

namespace RoleplayManager_Server {

    public partial class MainWindow:Window {

        #region Properties and Variables

        public static MainWindow mWindow;
        private Net.TCPServer Server;

        #endregion

        #region Constructor

        public MainWindow() {
            mWindow = this;
            InitializeComponent();
            Net.ServerNetworkDataHandler.InitializeNetworkPackets();
        }

        #endregion

        #region UI Functionality

        private void Grid_MouseDown(object sender,System.Windows.Input.MouseButtonEventArgs e) {
            if(e.ChangedButton == MouseButton.Left) {
                this.DragMove();
            }
        }

        private void Btn_Maximize_Click(object sender,RoutedEventArgs e) {
            if(WindowState == WindowState.Maximized) {
                WindowState = WindowState.Normal;
            } else if(WindowState == WindowState.Normal) {
                WindowState = WindowState.Maximized;
            }
        }

        private void Btn_Minimize_Click(object sender,RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void Btn_Exit_Click(object sender,RoutedEventArgs e) {
            //TODO: Save things here.
            Application.Current.Shutdown();
        }

        private void Btn_Github_Click(object sender,RoutedEventArgs e) {
            System.Diagnostics.Process.Start("https://github.com/Njorgrim/Roleplay-Manager");
        }

        private void Btn_StartServer_Click(object sender,RoutedEventArgs e) {
            int port;
            if(int.TryParse(TB_PortBox.Text,out port)) {
                if(!((port > 1024) && port < 65536)) {
                    TB_Error.Text = "Please enter a Port between 1024 and 65535!";
                    return;
                }
            } else {
                TB_Error.Text = "Please enter a Port between 1024 and 65535!";
                return;
            }

            Net.TCPServer.StartServer(int.Parse(TB_PortBox.Text),100);
        }

        #endregion

        #region Chat Functionality

        private void Btn_Send_Click(object sender,RoutedEventArgs e) {
            TB_ChatBox.AppendText("\n[Server]: " + TB_InputBox.Text);
            Net.TCPServer.BroadcastChatMessage(TB_InputBox.Text);
            TB_InputBox.Text = "";
            TB_ChatBox.ScrollToEnd();
        }

        private void InputBox_KeyDown(object sender,System.Windows.Input.KeyEventArgs e) {
            if(e.Key == System.Windows.Input.Key.Enter) {
                TB_ChatBox.AppendText("\n[Server]: " + TB_InputBox.Text);
                Net.TCPServer.BroadcastChatMessage(TB_InputBox.Text);
                TB_InputBox.Text = "";
                TB_ChatBox.ScrollToEnd();
            }
        }

        public static void WriteChatMessage(string msg) {
            mWindow.Dispatcher.Invoke(() => {
                mWindow.TB_ChatBox.AppendText("\n" + msg);
                mWindow.TB_ChatBox.ScrollToEnd();
            });
        }

        #endregion

        #region Network Functionality

        #endregion

    }
}

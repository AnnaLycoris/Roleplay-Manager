using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RoleplayManager_Client {

    public partial class MainWindow:Window {
        #region Properties and Variables

        public static MainWindow mWindow;
        public static string ip;
        public static int port;
        public static string username;

        #endregion

        #region Constructor

        public MainWindow() {
            //Set up singleton MainWindow
            if(mWindow == null) {
                mWindow = this;
            } else {
                mWindow.Close();
                mWindow = this;
            }
            InitializeComponent();

            Net.ClientNetworkDataHandler.InitializeNetworkPackets();
        }

        #endregion

        #region Network Functionality

        public static void StartClient() {
            Net.TCPClient.StartClient(ip,port);
        }

        public static void SendUsername(string name) {
            Net.TCPClient.SendUserName(name);
        }

        public void ChangeUsername(string name) {
            username = name;
            TB_Username.Text = name;
        }

        public void RefreshUserList(List<string> users) {
            mWindow.Dispatcher.Invoke(() => {
                SP_UserList.Children.Clear();

                foreach (string s in users) {
                    TextBlock tb = new TextBlock();
                    tb.Text = s;
                    tb.Margin = new Thickness(10,5,0,0);
                    SP_UserList.Children.Add(tb);
                }
            });
        }

        #endregion

        #region UI Functionality

        private void Btn_CloseMenu_Click(object sender,RoutedEventArgs e) {
            Btn_OpenMenu.Visibility = Visibility.Visible;
            Btn_CloseMenu.Visibility = Visibility.Collapsed;
        }

        private void Btn_OpenMenu_Click(object sender,RoutedEventArgs e) {
            Btn_CloseMenu.Visibility = Visibility.Visible;
            Btn_OpenMenu.Visibility = Visibility.Collapsed;
        }

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

        private void Btn_PopupExit_Click(object sender,RoutedEventArgs e) {
            //TODO: Save things here.
            Application.Current.Shutdown();
        }

        private void Btn_Github_Click(object sender,RoutedEventArgs e) {
            System.Diagnostics.Process.Start("https://github.com/Njorgrim/Roleplay-Manager");
        }

        #endregion

        #region Chat Functionality

        private void Btn_Send_Click(object sender,RoutedEventArgs e) {
            ChatBox.AppendText("\n" + username + ": " + InputBox.Text);
            Net.TCPClient.SendChatMessage(InputBox.Text);
            InputBox.Text = "";
            ChatBox.ScrollToEnd();
        }

        private void InputBox_KeyDown(object sender,System.Windows.Input.KeyEventArgs e) {
            if(e.Key == System.Windows.Input.Key.Enter) {
                ChatBox.AppendText("\n" + username + ": " + InputBox.Text);
                Net.TCPClient.SendChatMessage(InputBox.Text);
                InputBox.Text = "";
                ChatBox.ScrollToEnd();
            }
        }

        public static void WriteChatMessage(string msg) {
            mWindow.Dispatcher.Invoke(() => {
                mWindow.ChatBox.AppendText("\n" + msg);
                mWindow.ChatBox.ScrollToEnd();
            });
        }

        #endregion
    }
}

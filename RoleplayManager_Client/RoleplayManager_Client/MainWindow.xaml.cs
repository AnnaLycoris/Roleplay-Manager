using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RoleplayManager_Client {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:Window {

        public static MainWindow mWindow;
        public static string ip;
        public static int port;
        public static string username;

        public MainWindow() {
            //Set up singleton MainWindow
            if (mWindow == null) {
                mWindow = this;
            } else {
                mWindow.Close();
                mWindow = this;
            }
            InitializeComponent();
            try {
                BGPath.Data = Geometry.Parse(File.ReadAllText("SVG.txt",Encoding.UTF8));
            } catch {
                WriteChatMessage("No background SVG found.");
            }
            Net.ClientNetworkDataHandler.InitializeNetworkPackets();
        }

        public static void StartClient() {
            Net.TCPClient.StartClient(ip, port);
        }

        public static void WriteChatMessage(string msg) {
            mWindow.Dispatcher.Invoke(() => {
                mWindow.ChatBox.AppendText("\n" + msg);
                mWindow.ChatBox.ScrollToEnd();
            });
        }

        public void ChangeUsername(string name) {
            username = name;
            TB_Username.Text = name;
        }

        private void Button_Click_1(object sender,RoutedEventArgs e) {
            ChatBox.AppendText("\n" + InputBox.Text);
            Net.TCPClient.ChatMessage(InputBox.Text);
            InputBox.Text = "";
            ChatBox.ScrollToEnd();
        }

        private void InputBox_KeyDown(object sender,System.Windows.Input.KeyEventArgs e) {
            if (e.Key == System.Windows.Input.Key.Enter) {
                ChatBox.AppendText("\n" + InputBox.Text);
                Net.TCPClient.ChatMessage(InputBox.Text);
                InputBox.Text = "";
                ChatBox.ScrollToEnd();
            }
        }

        private void Grid_MouseDown(object sender,System.Windows.Input.MouseButtonEventArgs e) {
            if(e.ChangedButton == MouseButton.Left) {
                this.DragMove();
            }
        }

        private void Btn_PopupExit_Click(object sender,RoutedEventArgs e) {
            //Save things here.
            Application.Current.Shutdown();
        }

        private void Btn_CloseMenu_Click(object sender,RoutedEventArgs e) {
            Btn_OpenMenu.Visibility = Visibility.Visible;
            Btn_CloseMenu.Visibility = Visibility.Collapsed;
        }

        private void Btn_OpenMenu_Click(object sender,RoutedEventArgs e) {
            Btn_CloseMenu.Visibility = Visibility.Visible;
            Btn_OpenMenu.Visibility = Visibility.Collapsed;
        }

        private void Button_Click_2(object sender,RoutedEventArgs e) {
            if(WindowState == WindowState.Maximized) {
                WindowState = WindowState.Normal;
            } else if(WindowState == WindowState.Normal) {
                WindowState = WindowState.Maximized;
            }
        }

        private void Button_Click_3(object sender,RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }
    }
}

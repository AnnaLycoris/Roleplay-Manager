using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RoleplayManager_Client
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window {

        public static LoginWindow lWindow;
        public static MainWindow mWindow = new MainWindow();

        public LoginWindow()
        {
            //Set up singleton LoginWindow
            if(lWindow == null) {
                lWindow = this;
            } else {
                this.Close();
            }

            InitializeComponent();
        }

        private void Button_Click(object sender,RoutedEventArgs e) {
            IPAddress ip;
            if(IPAddress.TryParse(IPBox.Text,out ip)) {
                if(ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                    //Gud
                } else {
                    TB_Error.Text = "Your IP is not a valid IPv4 Address.";
                    return;
                }
            } else {
                TB_Error.Text = "Please enter a valid IP Address!";
                return;
            }
            int port;
            if(int.TryParse(PortBox.Text,out port)) {
                if ((port > 1024) && port < 65536) {
                    //Gud
                } else {
                    TB_Error.Text = "Please enter a Port between 1024 and 65535!";
                    return;
                }
            } else {
                TB_Error.Text = "Please enter a Port between 1024 and 65535!";
                return;
            }

            MainWindow.ip = IPBox.Text;
            MainWindow.port = port;
            MainWindow.StartClient();
        }

        public static void OpenMainWindow() {
            lWindow.Dispatcher.Invoke(() => {
                lWindow.PortBox.Visibility = Visibility.Collapsed;
                lWindow.IPBox.Visibility = Visibility.Collapsed;
                lWindow.Btn_Connect.Visibility = Visibility.Collapsed;

                lWindow.NameBox.Visibility = Visibility.Visible;
                lWindow.Btn_SetName.Visibility = Visibility.Visible;

                lWindow.TB_Error.Text = "Connection established!\nPlease choose a Username.";
                lWindow.TB_Error.Foreground = new SolidColorBrush(Colors.LightGreen);
            });
        }

        public static void WriteError(string msg) {
            lWindow.Dispatcher.Invoke(() => {
                lWindow.TB_Error.Text = "Connection failed.\nCheck Tooltip for more Information.";
                lWindow.TB_Error.ToolTip = msg;
            });
        }

        private void Button_Click_1(object sender,RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void Button_Click_2(object sender,RoutedEventArgs e) {
            if (NameBox.Text.Length <= 16 && NameBox.Text.Length > 0) {
                mWindow.ChangeUsername(NameBox.Text);
                mWindow.Show();
                lWindow.Close();
            } else {
                TB_Error.Text = "Usernames must be between 1 and 16 characters long.";
                TB_Error.Foreground = new SolidColorBrush(Colors.IndianRed);
            }
        }
    }
}

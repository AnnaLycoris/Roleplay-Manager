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

    public partial class LoginWindow : Window {
        #region Properties and Variables

        public static LoginWindow lWindow;
        public static MainWindow mWindow = new MainWindow();

        #endregion

        #region Constructor

        public LoginWindow() {
            //Set up singleton LoginWindow
            if(lWindow == null) {
                lWindow = this;
            } else {
                this.Close();
            }

            InitializeComponent();
        }

        #endregion

        #region UI Button Events

        private void Btn_Connect_Click(object sender,RoutedEventArgs e) {
            IPAddress ip;
            //Verify integrity of IP Address.
            //Currently, the program is set up to accept only IPv4 connections.
            if(IPAddress.TryParse(IPBox.Text,out ip)) {
                if(!(ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)) {
                    TB_Error.Text = "Your IP is not a valid IPv4 Address.";
                    return;
                }
            } else {
                TB_Error.Text = "Please enter a valid IP Address!";
                return;
            }
            int port;
            //Verify whether the port number is above the reserved port threshold as well as within range of 8 bit.
            if(int.TryParse(PortBox.Text,out port)) {
                if(!((port > 1024) && port < 65536)) {
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

        private void Btn_SetName_Click(object sender,RoutedEventArgs e) {
            //Applies the respective username, if it is within reasonable length.
            if(NameBox.Text.Length <= 16 && NameBox.Text.Length > 0) {
                mWindow.ChangeUsername(NameBox.Text);
                MainWindow.SendUsername(NameBox.Text);
                mWindow.Show();
                lWindow.Close();
            } else {
                TB_Error.Foreground = new SolidColorBrush(Colors.IndianRed);
                TB_Error.Text = "Usernames must be between 1 and 16 characters long.";
            }
        }

        private void Btn_Exit_Click(object sender,RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        #endregion

        #region Network Client Hooks

        //Replaces the network input panel for the username input panel.
        public static void ConnectionSuccessful() {
            lWindow.Dispatcher.Invoke(() => {
                //Hide Port/IP input panel.
                lWindow.PortBox.Visibility = Visibility.Collapsed;
                lWindow.IPBox.Visibility = Visibility.Collapsed;
                lWindow.Btn_Connect.Visibility = Visibility.Collapsed;

                //Show Username input panel.
                lWindow.NameBox.Visibility = Visibility.Visible;
                lWindow.Btn_SetName.Visibility = Visibility.Visible;

                //Display instructions for username input.
                lWindow.TB_Error.Text = "Please choose a Username.";
                lWindow.TB_Error.Foreground = new SolidColorBrush(Colors.LightGreen);
            });
        }

        //General UI method for displaying connection errors to the user.
        public static void WriteError(string msg) {
            lWindow.Dispatcher.Invoke(() => {
                lWindow.TB_Error.Text = "Connection failed.\nCheck Tooltip for more Information.";
                lWindow.TB_Error.ToolTip = msg;
            });
        }

        #endregion
    }
}

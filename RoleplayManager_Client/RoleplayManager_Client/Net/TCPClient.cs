using Shared;
using System;
using System.Net;
using System.Net.Sockets;

namespace RoleplayManager_Client.Net {

    class TCPClient {

        private static Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        private static byte[] asyncbuffer = new byte[1024];
        private static bool connected;

        public static void StartClient(string ip, int port) {
            //Console.WriteLine("Connecting to server...");
            LoginWindow.WriteError("Attempting to connect...");
            socket.BeginConnect(ip,port,new AsyncCallback(ConnectCallback),socket);
        }

        private static void ConnectCallback(IAsyncResult ar) {
            //Catch exception when connection cannot be established
            try {
                socket.EndConnect(ar);
            } catch (Exception e) {
                LoginWindow.WriteError(e.Message);
                return;
            }

            LoginWindow.OpenMainWindow();
            connected = true;
            while(connected) {
                OnReceive();
            }
        }

        private static void OnReceive() {
            byte[] sizeInfo = new byte[4];
            byte[] receiveBuffer = new byte[1024];

            int totalRead = 0, currentRead = 0;

            try {
                currentRead = totalRead = socket.Receive(sizeInfo);

                if (totalRead <= 0) {
                    //Console.WriteLine("Connection Lost.");
                    MainWindow.WriteChatMessage("Connection Lost.");
                    connected = false;
                } else {
                    while(totalRead < sizeInfo.Length && currentRead > 0) {
                        currentRead = socket.Receive(sizeInfo,totalRead,sizeInfo.Length - totalRead,SocketFlags.None);
                        totalRead += currentRead;
                    }

                    int messageSize = 0;
                    messageSize |= sizeInfo[0];
                    messageSize |= (sizeInfo[1] << 8);
                    messageSize |= (sizeInfo[2] << 16);
                    messageSize |= (sizeInfo[3] << 24);

                    byte[] data = new byte[messageSize];

                    totalRead = 0;
                    currentRead = totalRead = socket.Receive(data,totalRead,data.Length - totalRead,SocketFlags.None);
                    while(totalRead < messageSize && currentRead > 0) {
                        currentRead = socket.Receive(data,totalRead,data.Length - totalRead,SocketFlags.None);
                        totalRead += currentRead;
                    }

                    //HandleNetworkInformation
                    ClientNetworkDataHandler.HandleNetworkInformation(data);
                }
            } catch (Exception e) {
                //Console.WriteLine("Connection Lost.");
                MainWindow.WriteChatMessage("Connection Lost: Exception " + e.Message);
                connected = false;
            }
        }

        public static void SendData(byte[] data) {
            socket.Send(data);
        }

        public static void ConnectionOK() {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger((int) ClientPackets.CConnectionOK);
            buffer.WriteString("Successfully Connected!");
            SendData(buffer.ToArray());
            buffer.Dispose();
        }

        public static void ChatMessage(string msg) {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger((int) ClientPackets.CChatMessage);
            buffer.WriteString(msg);
            SendData(buffer.ToArray());
            buffer.Dispose();
        }
    }
}

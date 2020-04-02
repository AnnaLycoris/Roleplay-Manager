using Shared;
using System;
using System.Net;
using System.Net.Sockets;

namespace RoleplayManager_Client.Net {

    class TCPClient {

        #region Properties and Variables

        private static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static byte[] asyncbuffer = new byte[1024];
        private static bool connected;

        #endregion

        //Opens socket connection via Async context using ConnectCallback
        public static void StartClient(string ip, int port) {
            LoginWindow.WriteError("Attempting to connect...");
            socket.BeginConnect(ip, port, new AsyncCallback(ConnectCallback), socket);
        }

        private static void ConnectCallback(IAsyncResult ar) {
            //Catch exception when connection cannot be established
            //and output the error to login window.
            try {
                socket.EndConnect(ar);
            } catch (Exception e) {
                LoginWindow.WriteError(e.Message);
                return;
            }

            LoginWindow.ConnectionSuccessful();
            connected = true;
            //Establish infinite listening loop for receiving network data.
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
                    MainWindow.WriteChatMessage("Connection Lost.");
                    connected = false;
                } else {
                    while(totalRead < sizeInfo.Length && currentRead > 0) {
                        currentRead = socket.Receive(sizeInfo, totalRead, sizeInfo.Length - totalRead, SocketFlags.None);
                        totalRead += currentRead;
                    }

                    int messageSize = 0;
                    messageSize |= sizeInfo[0];
                    messageSize |= (sizeInfo[1] << 8);
                    messageSize |= (sizeInfo[2] << 16);
                    messageSize |= (sizeInfo[3] << 24);

                    byte[] data = new byte[messageSize];

                    totalRead = 0;
                    currentRead = totalRead = socket.Receive(data, totalRead, data.Length - totalRead, SocketFlags.None);
                    while(totalRead < messageSize && currentRead > 0) {
                        currentRead = socket.Receive(data, totalRead, data.Length - totalRead, SocketFlags.None);
                        totalRead += currentRead;
                    }

                    ClientNetworkDataHandler.HandleNetworkInformation(data);
                }
            } catch (Exception e) {
                MainWindow.WriteChatMessage("Connection Lost: Exception " + e.Message);
                connected = false;
            }
        }

        public static void SendData(byte[] data) {
            socket.Send(data);
        }

        #region Individial Packet Senders

        //Client Packet 1
        public static void SendConnectionOK() {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger((int) ClientPackets.CConnectionOK);
            buffer.WriteString("Successfully Connected!");
            SendData(buffer.ToArray());
            buffer.Dispose();
        }

        //Client Packet 2
        public static void SendChatMessage(string msg) {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger((int) ClientPackets.CChatMessage);
            buffer.WriteString(msg);
            SendData(buffer.ToArray());
            buffer.Dispose();
        }

        //Client Packet 3
        public static void SendUserName(string name) {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger((int) ClientPackets.CUsername);
            buffer.WriteString(name);
            SendData(buffer.ToArray());
            buffer.Dispose();
        }

        //Client Packet 4
        public static void SendPluginPacket(string pluginName, string data) {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger((int) ClientPackets.CPluginPacket);
            buffer.WriteString(pluginName);
            buffer.WriteString(data);
            SendData(buffer.ToArray());
            buffer.Dispose();
        }

        #endregion
    }
}

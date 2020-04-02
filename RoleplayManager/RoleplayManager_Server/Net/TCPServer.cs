using System;
using Shared;
using System.Net;
using System.Net.Sockets;

namespace RoleplayManager_Server.Net {
    class TCPServer {

        #region Properties and Variables

        private static Socket socket;
        private static byte[] buffer = new byte[1024];
        private static int serverSize = 100;

        //Refactor as dynamic structure
        public static Client[] clients;

        #endregion

        public static void StartServer(int port, int slots) {
            serverSize = slots;
            clients = new Client[serverSize];

            for (int i = 0; i < serverSize; i++) {
                clients[i] = new Client();
                clients[i].index = i;
            }

            socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.ReuseAddress,true);

            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(10);
            socket.BeginAccept(new AsyncCallback(AcceptCallback),null);
        }

        private static void AcceptCallback(IAsyncResult ar) {
            Socket s = socket.EndAccept(ar);
            socket.BeginAccept(new AsyncCallback(AcceptCallback),null);

            //TODO: Refactor as a dynamic data type.
            foreach (Client c in clients) {
                if (c.socket == null) {
                    c.username = "Pending";
                    c.socket = s;
                    c.ip = s.RemoteEndPoint.ToString();
                    c.StartClient();

                    MainWindow.WriteChatMessage("Connection from " + c.ip + " received.");
                    SendConnectionOK(c.index);
                    return;
                }
            }
        }

        public static void SendDataTo(int clientIndex, byte[] data) {
            byte[] sizeInfo = new byte[4];
            sizeInfo[0] = (byte) data.Length;
            sizeInfo[1] = (byte) (data.Length >> 8);
            sizeInfo[2] = (byte) (data.Length >> 16);
            sizeInfo[3] = (byte) (data.Length >> 24);

            clients[clientIndex].socket.Send(sizeInfo);
            clients[clientIndex].socket.Send(data);
        }

        public static string GetUsernameFromIndex(int index) {
            foreach (Client c in clients) {
                if (c.socket != null && c.index == index) {
                    return c.username;
                }
            }

            return "UsernameError";
        }

        public static void SetClientUsername(int index, string name) {
            foreach (Client c in clients) {
                if (c.index == index) {
                    c.SetUsername(name);
                }
            }
        }

        #region Individual Packet Senders

        public static void SendConnectionOK(int index) {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger((int) ServerPackets.SConnectionOK);
            buffer.WriteString("Connection successful!");
            SendDataTo(index,buffer.ToArray());
            buffer.Dispose();
        }

        public static void SendChatMessage(int index, string msg, string sender) {
            foreach(Client c in clients) {
                if(c.socket != null && c.index != index) {
                    PacketBuffer buffer = new PacketBuffer();
                    buffer.WriteInteger((int) ServerPackets.SChatMessage);
                    buffer.WriteString(sender + ": " + msg);
                    SendDataTo(c.index,buffer.ToArray());
                    buffer.Dispose();
                }
            }
        }

        public static void BroadcastChatMessage(string msg) {
            foreach (Client c in clients) {
                if (c.socket != null) {
                    PacketBuffer buffer = new PacketBuffer();
                    buffer.WriteInteger((int) ServerPackets.SChatMessage);
                    buffer.WriteString("[Server]: " + msg);
                    SendDataTo(c.index,buffer.ToArray());
                    buffer.Dispose();
                }
            }
        }

        public static void BroadcastUsernames() {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger((int) ServerPackets.SBroadcastUsernames);

            int userAmount = 0;

            foreach(Client c in clients) {
                if (c.socket != null && !c.closing) {
                    userAmount++;
                }
            }

            buffer.WriteInteger(userAmount);
            
            foreach(Client c in clients) {
                if (c.socket != null  && !c.closing) {
                    buffer.WriteString(c.username);
                }
            }
            
            foreach(Client c in clients) {
                if(c.socket != null  && !c.closing) {
                    SendDataTo(c.index, buffer.ToArray());
                }
            }

            buffer.Dispose();
        }

        public static void BroadcastPluginPacket(string pluginName, int index, string pluginData) {
            foreach (Client c in clients) {
                if (c.socket != null) {
                    PacketBuffer buffer = new PacketBuffer();
                    buffer.WriteInteger((int) ServerPackets.SPluginPacket);
                    buffer.WriteString(pluginName);
                    buffer.WriteString(TCPServer.GetUsernameFromIndex(index));
                    buffer.WriteString(pluginData);
                    SendDataTo(c.index,buffer.ToArray());
                    buffer.Dispose();
                }
            }
        }

        #endregion
    }

    class Client {

        #region Properties and Variables

        public int index;
        public string ip;
        public Socket socket;
        public string username;
        public bool closing = false;
        private byte[] buffer = new byte[1024];

        #endregion

        public void StartClient() {
            socket.BeginReceive(buffer,0,buffer.Length,SocketFlags.None,new AsyncCallback(ReceiveCallback), socket);
            closing = false;
        }

        public void SetUsername(string name) {
            username = name;
        }

        private void ReceiveCallback(IAsyncResult ar) {
            Socket socket = (Socket)ar.AsyncState;

            try {
                int received = socket.EndReceive(ar);
                if (received <= 0) {
                    CloseClient(index);
                } else {
                    byte[] databuffer = new byte[received];
                    Array.Copy(buffer,databuffer,received);

                    ServerNetworkDataHandler.HandleNetworkInformation(index,databuffer);
                    socket.BeginReceive(buffer,0,buffer.Length,SocketFlags.None,new AsyncCallback(ReceiveCallback),socket);
                }
            } catch {
                CloseClient(index);
            }
        }

        private void CloseClient(int index) {
            closing = true;

            TCPServer.BroadcastUsernames();

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            MainWindow.WriteChatMessage("Connection from " + ip + " has been terminated.");
            socket = null;
        }
    }
}

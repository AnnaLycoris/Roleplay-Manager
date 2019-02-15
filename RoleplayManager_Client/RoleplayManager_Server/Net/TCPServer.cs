using System;
using Shared;
using System.Net;
using System.Net.Sockets;

namespace RoleplayManager_Server.Net {
    class TCPServer {
        private static Socket socket;
        private static byte[] buffer = new byte[1024];

        //Maybe rewrite as list.
        public static Client[] clients = new Client[100];
        
        public static void StartServer(int port) {

            for (int i = 0; i<100; i++) {
                clients[i] = new Client();
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

            //Maybe rewrite this as a list that simply adds clients.
            for (int i = 0; i < 100; i++) {
                if (clients[i].socket == null) {
                    clients[i].socket = s;
                    clients[i].index = i;
                    clients[i].ip = s.RemoteEndPoint.ToString();
                    clients[i].StartClient();
                    //Console.WriteLine("Connection from " + clients[i].ip + " received.");
                    MainWindow.WriteChatMessage("Connection from " + clients[i].ip + " received.");
                    SendConnectionOK(i);
                    return;
                }
            }
        }

        public static void SendDataTo(int index, byte[] data) {
            byte[] sizeInfo = new byte[4];
            sizeInfo[0] = (byte) data.Length;
            sizeInfo[1] = (byte) (data.Length >> 8);
            sizeInfo[2] = (byte) (data.Length >> 16);
            sizeInfo[3] = (byte) (data.Length >> 24);

            clients[index].socket.Send(sizeInfo);
            clients[index].socket.Send(data);
        }

        public static void SendConnectionOK(int index) {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger((int) ServerPackets.SConnectionOK);
            buffer.WriteString("Connection successful!");
            SendDataTo(index,buffer.ToArray());
            buffer.Dispose();
        }

        public static void SendChatMessage(int index, string msg) {
            foreach(Client c in clients) {
                if (c.socket != null && c.index != index) {
                    PacketBuffer buffer = new PacketBuffer();
                    buffer.WriteInteger((int) ServerPackets.SChatMessage);
                    buffer.WriteString(index + ": " + msg);
                    SendDataTo(c.index,buffer.ToArray());
                    buffer.Dispose();
                }
            }
        }
    }

    class Client {
        public int index;
        public string ip;
        public Socket socket;
        public bool closing = false;
        private byte[] buffer = new byte[1024];

        public void StartClient() {
            socket.BeginReceive(buffer,0,buffer.Length,SocketFlags.None,new AsyncCallback(ReceiveCallback), socket);
            closing = false;
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
                    //HandleNetworkInformation;
                    ServerNetworkDataHandler.HandleNetworkInformation(index,databuffer);
                    socket.BeginReceive(buffer,0,buffer.Length,SocketFlags.None,new AsyncCallback(ReceiveCallback),socket);
                }
            } catch {
                CloseClient(index);
            }
        }

        private void CloseClient(int index) {
            closing = true;
            //Console.WriteLine("Connection from " + ip + " has been terminated.");
            MainWindow.WriteChatMessage("Connection from " + ip + " has been terminated.");

            socket.Close();
        }
    }
}

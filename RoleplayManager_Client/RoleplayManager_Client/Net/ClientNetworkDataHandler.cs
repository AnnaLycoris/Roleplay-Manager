using Shared;
using System.Collections.Generic;

namespace RoleplayManager_Client.Net {
    class ClientNetworkDataHandler {
        private delegate void Packet_(byte[] data);
        private static Dictionary<int,Packet_> Packets;

        public static void InitializeNetworkPackets() {
            //Console.WriteLine("Initialize Network Packages");
            MainWindow.WriteChatMessage("Initializing Network Packages");
            Packets = new Dictionary<int,Packet_> {
                {(int)ServerPackets.SConnectionOK, HandleConnectionOK},
                {(int)ServerPackets.SChatMessage, HandleChatMessage}
            };
        }

        public static void HandleNetworkInformation(byte[] data) {
            int packetnum;
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            packetnum = buffer.ReadInteger();
            buffer.Dispose();
            if(Packets.TryGetValue(packetnum,out Packet_ Packet))
            {
                Packet.Invoke(data);
            }
        }

        public static void HandleConnectionOK(byte[] data) {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            string msg = buffer.ReadString();
            buffer.Dispose();

            //add your code you wanna execute here
            //Console.WriteLine(msg);
            MainWindow.WriteChatMessage(msg);

            TCPClient.ConnectionOK();
        }

        public static void HandleChatMessage(byte[] data) {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            string msg = buffer.ReadString();
            buffer.Dispose();

            MainWindow.WriteChatMessage(msg);
        }
    }
}

using Shared;
using System;
using System.Collections.Generic;

namespace RoleplayManager_Server.Net
{
    class ServerNetworkDataHandler {
        private delegate void Packet_(int index, byte[] data);
        private static Dictionary<int,Packet_> Packets;

        public static void InitializeNetworkPackets() {
            //Console.WriteLine("Initialize Network Packages");
            MainWindow.WriteChatMessage("Initializing Network Packages");
            Packets = new Dictionary<int,Packet_> {
                {(int)ClientPackets.CConnectionOK, HandleConnectionOK},
                {(int)ClientPackets.CChatMessage, HandleChatMessage}
            };
        }

        public static void HandleNetworkInformation(int index, byte[] data) {
            int packetnum;
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            packetnum = buffer.ReadInteger();
            buffer.Dispose();
            if(Packets.TryGetValue(packetnum,out Packet_ Packet)) {
                Packet.Invoke(index, data);
            }
        }

        public static void HandleConnectionOK(int index, byte[] data) {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            string msg = buffer.ReadString();
            buffer.Dispose();

            //add your code you wanna execute here
            //Console.WriteLine(msg);
            MainWindow.WriteChatMessage(msg);
        }

        public static void HandleChatMessage(int index, byte[] data) {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            string msg = buffer.ReadString();
            buffer.Dispose();
            
            MainWindow.WriteChatMessage(index + ": " + msg);
            TCPServer.SendChatMessage(index, msg);
        }
    }
}

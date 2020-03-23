using Shared;
using System;
using System.Collections.Generic;

namespace RoleplayManager_Server.Net
{
    class ServerNetworkDataHandler {

        #region Properties and Variables

        private delegate void Packet_(int index,byte[] data);
        private static Dictionary<int,Packet_> Packets;

        #endregion

        public static void InitializeNetworkPackets() {
            Packets = new Dictionary<int,Packet_> {
                {(int)ClientPackets.CConnectionOK, HandleConnectionOK},
                {(int)ClientPackets.CChatMessage, HandleChatMessage},
                {(int)ClientPackets.CUsername, HandleUsername }
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

        #region Individual Packet Handlers

        public static void HandleConnectionOK(int index,byte[] data) {
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

            string name = TCPServer.GetUsernameFromIndex(index);

            MainWindow.WriteChatMessage("[" + index + "]" + name + ": " + msg);
            TCPServer.SendChatMessage(index, msg, name);
        }

        public static void HandleUsername(int index,byte[] data) {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            string name = buffer.ReadString();
            buffer.Dispose();

            TCPServer.SetClientUsername(index, name);

            MainWindow.WriteChatMessage("User " + index + " has registered as: " + name);

            TCPServer.BroadcastUsernames();
        }

        #endregion
    }
}

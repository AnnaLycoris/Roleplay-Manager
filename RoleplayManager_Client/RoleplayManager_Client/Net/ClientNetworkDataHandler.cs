using Shared;
using System.Collections.Generic;

namespace RoleplayManager_Client.Net {
    class ClientNetworkDataHandler {

        #region Properties and Variables

        private delegate void Packet_(byte[] data);
        private static Dictionary<int,Packet_> Packets;

        #endregion

        //Registers the different available network packages to their respective handler methods
        public static void InitializeNetworkPackets() {
            Packets = new Dictionary<int,Packet_> {
                {(int)ServerPackets.SConnectionOK, HandleConnectionOK},
                {(int)ServerPackets.SChatMessage, HandleChatMessage},
                {(int)ServerPackets.SBroadcastUsernames, HandleUsernameBroadcast}
            };
        }

        //Receives raw message data, then isolates the packet type from the data chunk and invokes the respective handler method.
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

        #region Individual Packet Handlers

        //Handles the ConnectionOK package. Tells the Application, that the login process was successful.
        //TODO: Silence method from MainWindow Log for Release versions (or move it to dedicated debug logger)
        public static void HandleConnectionOK(byte[] data) {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            string msg = buffer.ReadString();
            buffer.Dispose();

            MainWindow.WriteChatMessage(msg);

            TCPClient.SendConnectionOK();
        }

        //Handles any regular chat messages and writes it to the chat field via MainWindow.
        public static void HandleChatMessage(byte[] data) {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            string msg = buffer.ReadString();
            buffer.Dispose();

            MainWindow.WriteChatMessage(msg);
        }

        public static void HandleUsernameBroadcast(byte[] data) {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            int userAmount = buffer.ReadInteger();
            var users = new List<string>();

            for (int i = 0; i < userAmount; i++) {
                users.Add(buffer.ReadString());
            }

            MainWindow.mWindow.RefreshUserList(users);
        }

        #endregion

    }
}

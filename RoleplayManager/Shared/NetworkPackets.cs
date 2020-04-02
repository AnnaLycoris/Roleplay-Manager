namespace Shared {

    #region Server Packets

    //Packets sent from server to client
    //Client listens to server's packets
    public enum ServerPackets {
        SConnectionOK = 1,
        SChatMessage = 2,
        SBroadcastUsernames = 3,
        SPluginPacket = 4,
    }

    #endregion

    #region Client Packets

    //Packets sent from client to server
    //Server listens to client's packets
    public enum ClientPackets {
        CConnectionOK = 1,
        CChatMessage = 2,
        CUsername = 3,
        CPluginPacket = 4,
    }

    #endregion
}

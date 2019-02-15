using System;
using System.Collections.Generic;
using System.Text;

namespace Shared {
    //Packets sent from server to client
    //Client listens to server's packets
    public enum ServerPackets {
        SConnectionOK = 1,
        SChatMessage = 2,
    }

    //Packets sent from client to server
    //Server listens to client's packets
    public enum ClientPackets {
        CConnectionOK = 1,
        CChatMessage = 2,
    }
}

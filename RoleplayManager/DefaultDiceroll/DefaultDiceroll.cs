using RoleplayManager.PluginBase;
using System.Windows;
using System.Windows.Controls;

namespace DefaultDiceroll {
    class DefaultDiceroll:IPlugin {
        public static DefaultDiceroll entity;

        public DefaultDiceroll() {
            entity = this;
        }
        public string Name { get => "Default Diceroll"; }

        public string Description { get => "The default diceroll plugin"; }

        public string Version { get => "1.0.0"; }

        public PluginFrame pf = new PluginFrame();

        public UserControl PluginFrame => pf as UserControl;

        public event IPlugin.SentPluginPacketEventHandler SentPluginPacket;

        public void OnReceivedPluginPacket(string senderPluginName,string senderUsername,bool isOwnPacket,string data) {
            if (senderPluginName == Name) {
                if(isOwnPacket) {
                    //Handle case of receiving the packet this plugin itself sent.
                }
                pf.ReceiveData(senderUsername,data);
            }
        }

        public void SendPluginPacket(string data) {
            IPlugin.SentPluginPacketEventHandler handler = SentPluginPacket;
            handler?.Invoke(Name, data);
        }
    }
}
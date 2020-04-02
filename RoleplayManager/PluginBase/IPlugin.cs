using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace RoleplayManager.PluginBase
{
    public interface IPlugin
    {
        string Name { get; }
        string Description { get; }
        string Version { get; }
        //Bitmap/Icon/Picture icon { get; }
        UserControl PluginFrame { get; }

        //void SendNetworkData();
        //event EventHandler PluginSendNetworkData;

        //void execute(object sender, RoutedEventArgs e);

        void OnReceivedPluginPacket(string pluginName, string senderUsername, bool isOwnPacket, string data);

        void SendPluginPacket(string data);

        public event SentPluginPacketEventHandler SentPluginPacket;
        public delegate void SentPluginPacketEventHandler(string pluginName, string data);
    }
}

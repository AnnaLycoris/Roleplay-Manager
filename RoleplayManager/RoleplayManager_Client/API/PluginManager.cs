using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Windows.Controls;
using RoleplayManager.PluginBase;

namespace RoleplayManager_Client.API
{
    public class PluginManager {

        #region Constructor

        public PluginManager() {
            try {

                string[] pluginPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Plugins" + Path.DirectorySeparatorChar, "*.dll");
                /*
                IEnumerable<IPlugin> plugins = pluginPaths.SelectMany(pluginPath => {
                    Assembly pluginAssembly = LoadPlugin(pluginPath);
                    return CreatePlugins(pluginAssembly);
                }).ToList();*/

                var plugins = new List<IPlugin>();

                foreach(string path in pluginPaths) {
                    plugins.Add(CreatePlugins(LoadPlugin(path)).First());
                }

                foreach(IPlugin plugin in plugins) {
                    var pb = MainWindow.mWindow.CreatePluginButton(plugin.Name, plugin.PluginFrame);
                    pb.Btn_Clickable.Click += new System.Windows.RoutedEventHandler(pb.AddPluginControlToPluginContainer);
                    ReceivedPluginPacket += plugin.OnReceivedPluginPacket;
                    plugin.SentPluginPacket += OnSentPluginPacket;
                    //pb.AddPluginEventHandler(new System.Windows.RoutedEventHandler(plugin.execute));
                }

                /*
                foreach(IPlugin plugin in plugins) {
                    //Do Plugin init stuff here

                    plugin.execute();
                }*/

            } catch(Exception ex) {
                MainWindow.WriteChatMessage("Encountered exception during Plugin Load: " + ex.Message);
            }
        }

        #endregion

        #region Reflections

        static Assembly LoadPlugin(string relativePath) {
            string root = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
            MainWindow.WriteChatMessage("root: " + root);

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            MainWindow.WriteChatMessage("pluginLocation: " + pluginLocation);
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        static IEnumerable<IPlugin> CreatePlugins(Assembly assembly) {
            int count = 0;

            foreach(Type type in assembly.GetTypes()) {
                if(typeof(IPlugin).IsAssignableFrom(type)) {
                    IPlugin result = Activator.CreateInstance(type) as IPlugin;
                    if(result != null) {
                        count++;
                        yield return result;
                    }
                }
            }

            if(count == 0) {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements IPlugin in {assembly} from {assembly.Location}\n" +
                    $"Available types: {availableTypes}");
            }
        }

        #endregion

        #region Plugin Network Functionality

        public void OnSentPluginPacket(string pluginName, string data) {
            Net.TCPClient.SendPluginPacket(pluginName,data);
        }

        public event ReceivedPluginPacketEventHandler ReceivedPluginPacket;

        public delegate void ReceivedPluginPacketEventHandler(string pluginName, string senderUsername, bool isOwnPacket, string data);

        public void OnReceivedPluginPacket(string pluginName, string senderUsername, bool isOwnPacket, string data) {
            ReceivedPluginPacketEventHandler handler = ReceivedPluginPacket;
            handler?.Invoke(pluginName, senderUsername, isOwnPacket, data);
        }

        #endregion

    }
    class PluginLoadContext : AssemblyLoadContext {
        private AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath) {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName) {
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null) {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll (string unmanagedDllName) {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null) {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}

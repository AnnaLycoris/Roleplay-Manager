# Roleplay-Manager
A small tool providing a chat and a plugin base for implementing different pen&amp;paper (or other) systems for online roleplay. An application-based alternative to Tabletop Simulator, you could say!
Disclaimer: Some features may contain small bugs that may crash the application. I have not yet conducted thorough unit testing (As I am still learning how to professionally do exactly that)
After thorough testing, I will build an installer for the application.

# Installation
Release version will follow soon, as I am currently working on an installer.
This application is developed for Windows only, as it is using WPF for it's overall aesthetic.
Linux and OSX versions are currently not planned.

- 1. Install [Dotnet Core 3.1](https://dotnet.microsoft.com/download) or newer
- 2. Clone the Repository locally
- 3. Build the projects you need:

   - a. RoleplayManager_Client if you intend to join a server
  
   - b. RoleplayManager_Server if you intend to host a server (not required if you just want to play on an existing server)
  
   - c. PluginBase if you run ANY plugins to the application (recommended)
  
   - d. DefaultDiceroll for a very basic Diceroll Simulator.
  
     - d.1 The resulting files DefaultDiceroll.deps.json, DefaultDiceroll.dll and DefaultDiceroll.pdb need to be placed inside a Folder named "Plugins" contained in your RoleplayManager_Client.exe folder. (If it does not exist, create it yourself. Case Sensitive!)
 
   - e. Shared (mandatory for Client and Server)
  
- 4. You can now run both Server and Client.

Currently connection is only available directly via IP and Port (including 127.0.0.1 instead of localhost).
A matchmaking server is not planned!
This software is intended to be used in an established group of Pen & Paper Players, and thus for a small circle of players.

Furthermore the server host needs to forward the port they intend to use, as well as whitelist it in their Firewall and Antivirus software.

# Plugin Development

Clone the solution and create a new project.
Implement the PluginBase project and create your main class, implementing IPlugin.
Your Project needs to target Microsoft.NET.Sdk.WindowsDesktop on netcoreapp3.1 and set the <UseWPF> tag to true.

Finally implement IPlugin - The Button on the core app will be generated from the plugin name (no image yet).
The Button is set up to provide your Plugins' UserControl to the MainWindow of the application.
Create said UserControl and provide it as the PluginFrame (refer to Diceroll, if in doubt.)
Network Messages inside of plugins are currently limited to string objects only, and require the usage of string manipulation to properly read, if complicated.
The Plugin will receive Packets straight from the core app, along with: The username of the sender, a boolean designating whether the packet came frm this instance in the first place and of course your dataset.

MaterialDesign themes will be automatically applied to your design, so please utilize thorough testing to make sure that your plugin integrates well with the application.

If you design a plugin for this application, I'd be delighted to take a look at it. Please send me a DM here on github including your repository!

# Credit
[Njorgrim (aka Yulivee)](https://github.com/Njorgrim) - Development of the application

[Alexankitty ](https://github.com/Alexankitty) - Application Icons for Client and Server

# FleeFifoFrom

## Project Description

TODO: Add project description

## Tech Stack

* Unity 2020.3.7f1 (recommended)
* MLAPI v. 0.1.1

## Online Multiplayer Setup

In addition to local single-device multiplayer, our project supports online multiplayer sessions both through local area network (LAN) and over the internet. Each multiplayer solution requires the user to run a server build, which has to be restarted for every session, in addition to the client builds. All of the aforementioned builds can be found in the [Builds]() folder in the form of executables. In this section we will go through how to set up online multiplayer sessions for both LAN and Internet variants. 

### LAN Multiplayer

Players who are connected to the same network can easily start a session without modifying any settings. Clients only have to type in the **private (local)** IPv4 address of the device hosting the server into the input field before starting the game as shown in the following image:

<p align="center">
  <img width="460" height="280" src=https://github.com/metul/FleeFifoFrom/blob/networking_merge/Docs/ConnectionUI_Filled.PNG "Connection UI">
</p>


In case you wish to use a different port than the default one (7777), you should also append that into to IP address when using the input field (e.g. 192.168.0.10:8080).

It is also technically possible to establish LAN connection inbetween multiple clients on the same device as the server, although we are not sure why you would like to do this besides debugging purposes (maybe you have four displays attached to your PC?). In that case, you can simply leave the input field empty and just click Start Client since the default connect address is set to 127.0.0.1 (localhost).

### Internet Multiplayer

The current version of our project supports over the internet connections through two different alternatives, namely **Relay Server** and **Port-Forwarding**. Since MLAPI does not have an official release for a relay server up to this date, this alternative can be rather unstable at times and we strongly recommend establishing internet multiplayer session by using port-forwarding. However, we still provide documentations for establishing connection over both alternatives. Contrary to LAN connection, with internet connection version you have to type in the **public** IPv4 address of the device running the server instance in the input field, instead of a private address.

#### Relay Server

In order to establish internet connection through the relay server, some transport settings need to be adjusted and the project has to be rebuilt (both server and client). After opening the Unity project, switch to the *NetworkScene* (Scenes/NetworkScene.unity) and click on the *NetworkManager* game object in the hierarchy. In the *U Net Transport* component of the *NetworkManager*, make sure if the box *Use MLAPI Relay* is checked and build the project again. Both server and client can easily be rebuilt by using the editor button *Build/BuildAll*. Since we are continuously hosting a relay server on an AWS Linux instance, you do not have to change any other settings in the *U Net Transport* component which is already set up with our relay server information (3.17.61.242/8888). However, if you wish to connect through your own relay server instance, you also have manually change *MLAPI Relay Address* and *MLAPI Relay Port* fields in the aforementioned component. 

<p align="center">
  <img width="420" height="280" src=https://github.com/metul/FleeFifoFrom/blob/networking_merge/Docs/UNetTransport.PNG "Transport Settings">
</p>

**Do not forget to uncheck the *Use MLAPI Relay* box for the remaining kind of multiplayer sessions (e.g. LAN, Port-Forwarding) or you will not be able to establish connection!**

#### Port-Forwarding

Port-forwarding alternative offers the most stable connection over the internet and therefore should be preferred over relay servers. However, it also requires a more advanced setup than other options. In order to establish internet connection with this approach you have to open UDP port 7777, which is the default port set in the *NetworkManager* in *NetworkScene*. If you would like to use another port instead, you can do so by modifying the *Connect Port* and *Server Listen Port* fields in the *U Net Transport* component of the *NetworkManager*. 

It is also recommended to set up a static IP address for the device running the server build in order to avoid any potential connection issues. [Port Forward Website](https://portforward.com/) contains detailed information on setting up a static IP for your device and enabling port forwarding for a wide variety of routers.

After setting up port-forwarding, simply run a server instance on the device you're forwarding ports to and connect to this server from other clients by typing in the **public** IPv4 address (and the port in case you are using a different port than 7777) of the server device into the input field and clicking *Start Client*.

## License & 3rd Party Content

"Flee FiFo From" is licensed under a MIT license. See the [LICENSE](LICENSE) file for details.

This project contains third-party content governed by the license(s) indicated below. For more information about the content, see the following sections.

### Game Icons by Kenny

 * Link: [Game Icons](https://www.kenney.nl/assets/game-icons)
 * License: [CC0 1.0 Universal (CC0 1.0)](https://creativecommons.org/publicdomain/zero/1.0/)
 * Creator: [Kenny](https://www.kenney.nl/)

### Generic Items by Kenny

 * Link: [Generic Items](https://www.kenney.nl/assets/generic-items)
 * License: [CC0 1.0 Universal (CC0 1.0)](https://creativecommons.org/publicdomain/zero/1.0/)
 * Creator: [Kenny](https://www.kenney.nl/)

### UI Audio by Kenny

 * Link: [UI Audio](https://www.kenney.nl/assets/ui-audio)
 * License: [CC0 1.0 Universal (CC0 1.0)](https://creativecommons.org/publicdomain/zero/1.0/)
 * Creator: [Kenny](https://www.kenney.nl/)


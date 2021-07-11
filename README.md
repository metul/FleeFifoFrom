# FleeFifoFrom

## Project Description

TODO: Add project description

## Tech Stack

* Unity 2020.3.7f1 (recommended)
* MLAPI v. 0.1.1

## Online Multiplayer Setup

In addition to local single-device multiplayer, our project supports online multiplayer sessions both through local area network (LAN) and over the internet. Each multiplayer solution requires the user to run a server build, which has to be restarted for every session, in addition to the client builds. All of the aforementioned builds can be found in the [Builds]() folder in the form of executables. In this section we will go through how to set up online multiplayer sessions for both LAN and Internet variants. 

### LAN Multiplayer

Players who are connected to the same network can easily start a session without modifying any settings. Clients only have to type in the local IPv4 address of the device hosting the server into the input field before starting the game as shown in the following image:

<p align="center">
  <img width="460" height="300" src=https://github.com/metul/FleeFifoFrom/blob/networking_merge/Docs/ConnectionUI.PNG "Connection UI">
</p>


In case you wish to use a different port than the default one (7777), you should also append that into to IP address when using the input field (e.g. 192.168.0.10:8080).

It is also technically possible to establish LAN connection inbetween multiple clients on the same device as the server, although we are not sure why you would like to do this besides debugging purposes (maybe you have four displays attached to your PC?). In that case, you can simply leave the input field empty and just click Start Client since the default connect address is set to 127.0.0.1 (localhost).

### Internet Multiplayer

The current version of our project supports over the internet connections through two different alternatives, namely **Relay Server** and **Port-Forwarding**. Since MLAPI does not have an official release for a relay server up to this date, this alternative can be rather unstable at times and we strongly recommend establishing internet multiplayer session by using port-forwarding. However, we still provide documentations for establishing connection over both alternatives.

#### Relay Server

TODO

#### Port-Forwarding

TODO

## License & 3rd Party Content

"Flee FiFo From" is licensed under a MIT license. See the [LICENSE](LICENSE) file for details.

This project contains third-party content governed by the license(s) indicated below. For more information about the content, see the following sections.

TODO: Add license attributions

### Asset by Creator (Alias)

 * Link: [Asset]()
 * License: [LicenseType]()
 * Creator: [Creator (Alias)]()


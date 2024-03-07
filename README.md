# GH_WebSockets

GH_WebSockets is a plugin to grasshopper for Rhino 8. It provides functionality for creating a WebSocket client within grasshopper for sending and receiving data with an external WebSocket server. It implements a websocket client using .net core and is not backwards compatible with previous version of rhino. For WebSocket support in previous version of rhino, refer to [Bengesht](https://www.food4rhino.com/en/app/bengesht) for similar functionality. 

The server used can be written in any language that supports the WebSocket protocol. Example files for an echo server can be found here. 

There are three components in GH_WebSockets and after installation these are available under the **Params** tab in grasshopper. 

**`Start`**
**`Send`**
**`Receive`**

It is generally best to start your server before starting the client. If you start the server after you create a start node in grasshopper you will need to reset the start node before the connection will be active. 

The **`Start`** node has four optional inputs: **Host**, **Port**, **Reset**, and **Message**. 

  **Host** - Address of server (Defaults to 127.0.0.1).
  **Port** - Port of the server (Defaults to 2000).
  **Reset** - Boolean flag for resetting the connection (Default state is False)

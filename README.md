# GH_WebSockets

GH_WebSockets is a plugin to grasshopper for Rhino 8. It provides functionality for creating a WebSocket client within grasshopper for sending and receiving data with an external WebSocket server. It implements a WebSocket client using .net core and is not backwards compatible with previous version of rhino. For WebSocket support in previous version of rhino, refer to [Bengesht](https://www.food4rhino.com/en/app/bengesht) for similar functionality.

The preferred installation method is using the PackageManager in Rhino. Just search *GH_WebSockets*.

GH_WebSockets is designed to be as neutral as possible and not make any assumptions about the server configuration. 

The server used can be written in any language that supports the WebSocket protocol. Example files for an echo server can be found [in Examples](Examples). 

There are three components in GH_WebSockets and after installation these are available under the **Params** tab in grasshopper: **`Start`**, **`Send`**, and **`Receive`**.

It is generally best to start your server before starting the client. If you start the server after you create a start node in grasshopper you will need to reset the start node before the connection will be active. 

The **`Start`** node has four optional inputs: **Host**, **Port**, **Reset**, and **Message**. The output, **WS**, is a WebSocket object that can be passed to a **`Send`** or
**`Receive`** node.

- **Host** - Address of server (Defaults to *127.0.0.1*).
- **Port** - Port of the server (Defaults to *2000*).
- **Reset** - Boolean flag for resetting the connection (Default state is *False*).
- **Message** - Message sent to the server upon connection (Defaults to *init*).

The **`Send`** has two required inputs: **WS** and **Message**. There is also an optional **Update** input. 

- **WS** - WebSocket object from the **`Start`** node.
- **Update** - Boolean flag for sending updates (Defaults to *True*). Attach a button to only send messages to your server occasionally.
- **Message** - The message to send to the WebSocket server. This can be any message. No assumptions are made about the message type.

The **`Receive`** node has one required input **WS**. There is also an optional **Update** input. 

- **WS** - WebSocket object from the **`Start`** node.
- **Update** - Boolean flag for receiving updates (Defaults to *True*). Attach a boolean toggle or button to prevent downstream updates.

>[!NOTE]
>**`Receive`** is receiving messages in the background even though it is not updating the downstream components. If multiple messages have been sent back to the receive component from the server while **Update** is set to false, setting it to true will update the ouptput with the *last* message sent back to grasshopper. 


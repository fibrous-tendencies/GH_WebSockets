using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;

using Grasshopper.Kernel;
using Microsoft.VisualBasic;
using Rhino.Geometry;
using Rhino;
using System.ComponentModel.Design;
using Rhino.Commands;
using System.IO;
using System.Net.Sockets;
using Grasshopper;

namespace GH_WebSockets
{
    public class GH_WebSocketReceive : GH_Component
    {
        ClientWebSocket ws = null;
        string msg = null;
        bool update = true;

        /// <summary>
        /// Initializes a new instance of the WebSocketReceive class.
        /// </summary>
        public GH_WebSocketReceive()
          : base("Receive", "Receive",
              "Receive a message from a WebSocket server.",
              "Params", "WebSocket")
        {

        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("WebSocket Object", "WS", "WebSocket Object", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Update", "U", "Update the WebSocket Client", GH_ParamAccess.item, true);
            
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Message", "M", "Message from WebSocket Server", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData(0, ref ws)) return;
            if (!DA.GetData(1, ref update)) return;



            if (ws != null)
            {
                if (ws.State == WebSocketState.Open)
                {
                    if (update)
                    {
                        try
                        {
                            Message = "Listening";
                            DA.SetData(0, msg);
                            Task.Run(() => Listen(null, ws));
                        }
                        catch (Exception e)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                        }
                    }
                    else
                    {
                        Message = "Not Listening";
                        DA.SetData(0, msg);                  
                    }
                }
            }


        }

        private async Task Listen(string[] args, ClientWebSocket ws)
        {
            try
            {
                while (ws.State == WebSocketState.Open)
                {

                        ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[8192]);

                        WebSocketReceiveResult result = null;

                        using var ms = new MemoryStream();
                        do
                        {
                            result = await ws.ReceiveAsync(buffer, CancellationToken.None);
                            ms.Write(buffer.Array, buffer.Offset, result.Count);
                        }
                        while (!result.EndOfMessage);

                        ms.Seek(0, SeekOrigin.Begin);

                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            using var reader = new StreamReader(ms, Encoding.UTF8);

                            if (update)
                        {
                            msg = reader.ReadToEnd();
                        }
                            
                            RhinoApp.InvokeOnUiThread((Action)delegate { ExpireSolution(true); });
                        }
                    }
                }
            

            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
            }           
        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.Receive;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("AD2C82E1-0904-4D00-8709-18E680EF7215"); }
        }
    }
}
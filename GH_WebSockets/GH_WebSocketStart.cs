using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using Grasshopper;
using Rhino.UI;


namespace GH_WebSockets
{
    public class GH_WebSocket: GH_Component
    {
        ClientWebSocket ws = null;
        GH_Document ghDoc = null;

        /// <summary>
        /// Initializes a new instance of the GH_WebSocketStart class.
        /// </summary>
        public GH_WebSocket()
          : base("Start", "Start",
              "Initalize a WebSocket Object",
              "Params", "WebSocket")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Host", "H", "WebSocket Server Host", GH_ParamAccess.item, "127.0.0.1");
            pManager.AddTextParameter("Port", "P", "WebSocket Server Port", GH_ParamAccess.item, "2000");
            pManager.AddBooleanParameter("Reset", "R", "Reset the WebSocket Client", GH_ParamAccess.item, false);
            pManager.AddTextParameter("Message", "M", "Initial message to send when starting or resetting the websocket client.", GH_ParamAccess.item, "init");
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("WebSocket Object", "WS", "WebSocket Object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override async void SolveInstance(IGH_DataAccess DA)
        {
            string host = "";
            string port = "";
            bool reset = false;
            string msg = "init";

            if (!DA.GetData(0, ref host)) return;
            if (!DA.GetData(1, ref port)) return;
            if (!DA.GetData(2, ref reset)) return;
            if (!DA.GetData(3, ref msg)) return;

            GH_Document doc = OnPingDocument();

            if (ghDoc == null || ghDoc != doc)
            {
                if (ghDoc != null)
                {
                    Instances.DocumentServer.DocumentRemoved -= DocumentServerOnDocumentClosed;
                    ObjectChanged -= OnObjectChanged;
                }
                ghDoc = doc;
                Instances.DocumentServer.DocumentRemoved += DocumentServerOnDocumentClosed;
                ObjectChanged += OnObjectChanged;
            }

            if (reset)
            {
                Message = "Resetting";
                if (ws != null)
                {
                    try
                    {
                        await Close(ws);
                        ws = new();
                    }
                    catch (Exception e)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                    }
                }
                else
                {
                    ws = new();
                }
                return;
            }

            if (ws != null)
            {
                if (ws.State != WebSocketState.Open)
                {
                    try
                    {
                        await Close(ws);
                    }
                    catch (Exception e)
                    {
                        ws.Dispose();
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                    }

                    ws = new();
                    try
                    {
                        await Connect(ws, host, port);
                    }
                    catch (Exception e)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                    }
                    return;

                }
                
            }
            else
            {
                ws = new();
                try
                {
                    await Connect(ws, host, port);
                }
                catch (Exception e)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                }
                return;
            }

            if (ws != null && ws.State == WebSocketState.Open)
            {
                Message = "Connected";
                await Send(ws, msg);
                DA.SetData(0, ws);            }
            else
            {
                Message = "Not Connected";
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, ws.State.ToString());
            }
        }

        private async Task Connect(ClientWebSocket ws, string host, string port)
        {
            await ws.ConnectAsync(new Uri($"ws://{host}:{port}/ws"), CancellationToken.None);
            ExpireSolution(true);
        }

        static async Task Close(ClientWebSocket ws)
        {
            await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Close", CancellationToken.None);
            ws.Dispose();
        }

        static async Task Send(ClientWebSocket ws, string msg)
        {
            await ws.SendAsync(Encoding.UTF8.GetBytes(msg), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        //The closing behavior safely closes the connection to the server. This method is adapted from Bengesht https://www.food4rhino.com/en/app/bengesht

        private void DocumentOnObjectsDeleted(object sender, GH_DocObjectEventArgs e)
        {
            if (e.Objects.Contains(this))
            {
                e.Document.ObjectsDeleted -= DocumentOnObjectsDeleted;
                Task.Run(() => Close(ws));
            }
        }

        private void DocumentServerOnDocumentClosed(GH_DocumentServer sender, GH_Document doc)
        {
            if (ghDoc != null && doc.DocumentID == ghDoc.DocumentID)
            {
                Task.Run(() => Close(ws));

            }
        }

        void OnObjectChanged(IGH_DocumentObject sender, GH_ObjectChangedEventArgs e)
        {
            if (Locked)
                Task.Run(() => Close(ws));
        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                return Properties.Resources.Start;

            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("D6BDD621-CA92-428E-82D8-66D5FCBE1D34"); }
        }
    }
}
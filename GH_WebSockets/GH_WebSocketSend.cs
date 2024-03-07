using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace GH_WebSockets
{
    public class GH_WebSocketSend : GH_Component
    {
        ClientWebSocket ws = null;

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public GH_WebSocketSend()
          : base("Send", "Send",
            "Send a message to a WebSocket server.",
            "Params", "WebSocket")
        {

        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("WebSocket Object", "WS", "WebSocket Object", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Update", "U", "Send message again", GH_ParamAccess.item, true);
            pManager.AddTextParameter("Message", "M", "Message to send", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override async void SolveInstance(IGH_DataAccess DA)
        {
            string msg = null;
            bool update = true;

            if (!DA.GetData(0, ref ws)) return;
            if (!DA.GetData(1, ref update)) return;
            if (!DA.GetData(2, ref msg)) return;
            

            if (ws != null)
            {
                if (ws.State == WebSocketState.Open)
                {
                    if (update)
                    {
                        try
                        {
                            Message = "Sending";
                            await Send(ws, msg);
                        }
                        catch (Exception e)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                        }
                    }
                    else
                    {
                        Message = "Not Updating";
                        return;
                    }

                }
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, ws.State.ToString());
            }  
        }

        static async Task Send(ClientWebSocket ws, string msg)
        {
            await ws.SendAsync(Encoding.UTF8.GetBytes(msg), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override Bitmap Icon
        {
            get
            {
                return Properties.Resources.Send;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("121704f0-386e-4972-bf20-47090a5e0f9e");
    }
}
using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace GH_WebSockets
{
    public class GH_WebSocketsInfo : GH_AssemblyInfo
    {
        public override string Name => "WebSockets";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        //public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "Utility for creating a WebSocket client inside grasshopper for send and receiving data with an external server.";

        public override Guid Id => new Guid("4ba3f2e9-36a4-4545-97fc-a904b176e4c2");

        //Return a string identifying you or your company.
        public override string AuthorName => "Adam Burke";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "aburke3@mit.edu";
    }
}
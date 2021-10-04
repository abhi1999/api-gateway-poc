using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMSAPIGateway
{
    public class Route
    {
        public string Client { get; set; }
        public bool IsStaticContent{ get; set; }
        public string Endpoint { get; set; }
        public Destination Destination { get; set; }
    }
}

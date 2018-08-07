using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RaceTrack {


    public class Program {
        // State object for reading client data asynchronously  

        // Thread signal.  
       
        public static int Main (String[] args) {
            var track = new TrackConnection();
            var player = new PlayerConnection();
            return 0;
        }
    }
}
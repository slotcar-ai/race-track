using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RaceTrack {

    public class Program {
        
        public static int Main (String[] args) {
            //var track = new TrackConnection();
            using (var player = new PlayerConnection ()) {
                var speed = -1;
                while (true) {
                    if (speed != player.GetLatestSpeed ()) {
                        speed = player.GetLatestSpeed ();
                        //track.SetSpeed(speed);
                    }
                    player.SendTrackUpdate ("En track update: " + DateTime.Now.Ticks);
                    Thread.Sleep (1000);
                }
            };
            return 0;
        }
    }
}
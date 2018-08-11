using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RaceTrack {

    public class Program {
        
        public static int Main (String[] args) {
           // var ;
            //using (var track = new TrackConnection())
            Console.WriteLine("RaceTrack started");
            using (var eventHub = new SlotcarAiEventHub())
            using (var player = new PlayerConnection ()) {
                var speed = -1;
                while (true) {
                    if (speed != player.GetLatestSpeed ()) {
                        speed = player.GetLatestSpeed ();
                        //track.SetSpeed(speed);
                    }

                    string trackUpdate = "En track update: " + DateTime.Now.Ticks;
                    player.SendTrackUpdate (trackUpdate);
                    eventHub.SendMessage(trackUpdate);
                    Thread.Sleep (1000);
                }
            };
            return 0;
        }
    }
}
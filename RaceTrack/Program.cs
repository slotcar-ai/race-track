﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RaceTrack
{

    public class Program
    {

        public static int Main(String[] args)
        {
            Console.WriteLine("RaceTrack started");
            using (var track = new TrackConnection())
            // using (var eventHub = new SlotcarAiEventHub())
            // using (var player = new PlayerConnection())
            {
                var speed = 0;
                while (speed <= 10000)
                {
                    // if (speed != player.GetLatestSpeed())
                    // {

                    // speed = player.GetLatestSpeed();
                    track.SetSpeed(speed);
                    // }
                    speed += 1000;
                    // string trackUpdate = "En track update: " + DateTime.Now.Ticks;
                    // player.SendTrackUpdate(trackUpdate);
                    // eventHub.SendMessage(trackUpdate);
                    Thread.Sleep(2000);
                }
                    Thread.Sleep(2000);
                    track.SetSpeed(0);
            };
            
            return 0;
        }
    }
}
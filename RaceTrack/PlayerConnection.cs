using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RaceTrack {
    internal class PlayerConnection : IDisposable {
        public class StateObject {
            public Socket workSocket = null;
            public const int BufferSize = 1024;
            public byte[] buffer = new byte[BufferSize];
            public StringBuilder sb = new StringBuilder ();
        }
        private char unitSeperatorChar = (char) Convert.ToInt32 ("0x1f", 16);
        private (int speed, Socket updateStreamer, Socket speedUpdate) _playerA;
        public PlayerConnection () {
            StartListening ();
            updateStreamerConected.WaitOne();
        }
        public ManualResetEvent updateStreamerConected = new ManualResetEvent (false);

        public int GetLatestSpeed () {
            return _playerA.speed;
        }
        public void StartListening () {
            updateStreamerConected.Reset ();

            var hostname = Environment.GetEnvironmentVariable("PLAYER_HOSTNAME");
            if (hostname == null)
            {
                hostname = "localhost";
            }

            var portString = Environment.GetEnvironmentVariable("PLAYER_PORT");
            var port = 11000;
            if (portString != null)
            {
                port = Int32.Parse(portString);
            }

            IPHostEntry ipHostInfo = Dns.GetHostEntry (hostname);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint (ipAddress, port);
            Socket listener = new Socket (ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try {
                listener.Bind (localEndPoint);
                listener.Listen (4);
                listener.BeginAccept (new AsyncCallback (AcceptCallback), listener);
            } catch (Exception e) {
                Console.WriteLine (e.ToString ());
                throw;
            }
        }

        public void AcceptCallback (IAsyncResult ar) {
            Socket listener = (Socket) ar.AsyncState;
            Socket handler = listener.EndAccept (ar);
            listener.BeginAccept (new AsyncCallback (AcceptCallback), listener);

            StateObject state = new StateObject ();
            state.workSocket = handler;
            handler.BeginReceive (state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback (ReadCallback), state);
        }

        public void ReadCallback (IAsyncResult ar) {
            String content = String.Empty;
            StateObject state = (StateObject) ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive (ar);

            if (bytesRead > 0) {
                state.sb.Append (Encoding.ASCII.GetString (
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                content = state.sb.ToString ();
                if (content.IndexOf (unitSeperatorChar) > -1) {
                    string data = content.Replace (unitSeperatorChar.ToString (), "");
                    Console.WriteLine ("Data : {1}",
                        content.Length, data);
                    state.sb.Clear ();
                    if (data == "TackUpdateStreamer") {
                        _playerA.updateStreamer= handler;
                        updateStreamerConected.Set ();
                        return;
                    } else if (data == "SpeedStreamer") {
                        _playerA.speedUpdate= handler;
                    } else {
                        int speed = 0;
                        if (Int32.TryParse (data, out speed)) {
                            _playerA.speed = speed;
                        }
                    }
                }
                handler.BeginReceive (state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback (ReadCallback), state);
            }
        }

        public void SendTrackUpdate (string trackState) {
            var socket = _playerA.updateStreamer;
            if (socket.Connected) {
                Send (socket, trackState);
            } else {
                Console.WriteLine ("Vi har mistet kontakt med trackStreamer socketen");
            }
        }

        private void Send (Socket handler, String data) {
            try {
                byte[] byteData = Encoding.ASCII.GetBytes (data);
                handler.BeginSend (byteData, 0, byteData.Length, 0,
                    new AsyncCallback (SendCallback), handler);

            } catch (System.Exception) {
                CloseHandle (handler);
            }
        }

        private void SendCallback (IAsyncResult ar) {
            try {
                Socket handler = (Socket) ar.AsyncState;
                int bytesSent = handler.EndSend (ar);
                Console.WriteLine ("Sent {0} bytes to client.", bytesSent);

            } catch (Exception e) {
                Console.WriteLine (e.ToString ());
            }
        }

        public void Dispose () {
            CloseHandle (_playerA.speedUpdate);
            CloseHandle (_playerA.updateStreamer);
        }

        private void CloseHandle (Socket socket) {
            if (socket == null) return;

            if (socket.Connected) {
                socket.Shutdown (SocketShutdown.Both);
            }
            socket.Close ();
        }
    }
}
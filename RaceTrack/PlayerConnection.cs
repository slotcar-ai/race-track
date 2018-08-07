using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RaceTrack {
    internal class PlayerConnection : IDisposable {
        public class StateObject {
            // Client  socket.  
            public Socket workSocket = null;
            // Size of receive buffer.  
            public const int BufferSize = 1024;
            // Receive buffer.  
            public byte[] buffer = new byte[BufferSize];
            // Received data string.  
            public StringBuilder sb = new StringBuilder ();
        }
        private char unitSeperatorChar = (char) Convert.ToInt32 ("0x1f", 16);
        private (int speed, Socket update, Socket speedUpdate) _playerA;
        public PlayerConnection () {
            StartListening ();
        }
        public ManualResetEvent allDone = new ManualResetEvent (false);

        public int GetLatestSpeed () {
            return _playerA.speed;
        }
        public void StartListening () {
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.GetHostEntry (Dns.GetHostName ());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint (ipAddress, 11000);
            IPEndPoint localEndPoint2 = new IPEndPoint (ipAddress, 11001);

            // Create a TCP/IP socket.  
            Socket listener = new Socket (ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            // Create a TCP/IP socket.  
            Socket listener2 = new Socket (ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try {

                listener.Bind (localEndPoint);
                listener.Listen (2);

                listener2.Bind (localEndPoint2);
                listener2.Listen (2);

                // while (true) {
                    // Set the event to nonsignaled state.  
                    allDone.Reset ();

                    Console.WriteLine ("Waiting for a connection...");
                    listener.BeginAccept (
                        new AsyncCallback (AcceptCallback),
                        listener);
                    // Start an asynchronous socket to listen for connections.  
                    listener2.BeginAccept (
                        new AsyncCallback (AcceptCallback),
                        listener2);

                    // Wait until a connection is made before continuing.  
                    // }
                    allDone.WaitOne ();
                    allDone.WaitOne ();
                // }
            } catch (Exception e) {
                Console.WriteLine (e.ToString ());
            }
        }

        public void AcceptCallback (IAsyncResult ar) {
            // Signal the main thread to continue.  
            allDone.Set ();

            // Get the socket that handles the client request.  
            Socket listener = (Socket) ar.AsyncState;
            Socket handler = listener.EndAccept (ar);

            // Create the state object.  
            StateObject state = new StateObject ();
            state.workSocket = handler;
            handler.BeginReceive (state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback (ReadCallback), state);
        }

        public void ReadCallback (IAsyncResult ar) {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject) ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive (ar);

            if (bytesRead > 0) {
                // There  might be more data, so store the data received so far.  
                state.sb.Append (Encoding.ASCII.GetString (
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                content = state.sb.ToString ();
                if (content.IndexOf (unitSeperatorChar) > -1) {
                    // All the data has been read from the   
                    // client. Display it on the console.  
                    string data = content.Replace (unitSeperatorChar.ToString (), "");
                    Console.WriteLine ("Data : {1}",
                        content.Length, data);
                    state.sb.Clear ();
                    if (data == "PlayerA:UpdateMe") {
                        _playerA = (speed: 0, update: handler, speedUpdate: null);
                        SendTrackUpdates (handler);
                        return;
                    } else if (data == "PlayerA:Speed") {
                        _playerA = (speed: 0, update: handler, speedUpdate: handler);
                        return;
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

        private void SendTrackUpdates (Socket handler) {
            var i = 1;
            while (handler.Connected) {
                i++;
                Send (handler, "AnUpdate: " + i);
                Thread.Sleep (1000);
            }
        }

        private void Send (Socket handler, String data) {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes (data);

            // Begin sending the data to the remote device.  
            handler.BeginSend (byteData, 0, byteData.Length, 0,
                new AsyncCallback (SendCallback), handler);
        }

        private void SendCallback (IAsyncResult ar) {
            try {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket) ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend (ar);
                Console.WriteLine ("Sent {0} bytes to client.", bytesSent);

            } catch (Exception e) {
                Console.WriteLine (e.ToString ());
            }
        }

        public void Dispose () {
            CloseHandle (_playerA.speedUpdate);
            CloseHandle (_playerA.update);
        }

        private void CloseHandle (Socket handle) {
            handle.Shutdown (SocketShutdown.Both);
            handle.Close ();
        }
    }
}
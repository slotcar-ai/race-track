using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RaceTrack {
    public class TrackSerialPort : ISerialPort {
        private static int OPEN_READ_WRITE = 2;

        private static int SERIAL_BUFFER_SIZE = 64;

        private byte[] serialDataBuffer = new byte[SERIAL_BUFFER_SIZE];
        [DllImport ("libc", EntryPoint = "open")]

        public static extern int Open (string fileName, int mode);

        [DllImport ("libc", EntryPoint = "close")]
        public static extern int Close (int handle);

        [DllImport ("libc", EntryPoint = "write")]
        private static extern int write (int handle, IntPtr buf, int count);

        [DllImport ("libc", EntryPoint = "read")]
        public static extern int Read (int handle, byte[] data, int length);

        [DllImport ("libc", EntryPoint = "tcgetattr")]
        public static extern int GetAttribute (int handle, [Out] byte[] attributes);

        [DllImport ("libc", EntryPoint = "tcsetattr")]
        public static extern int SetAttribute (int handle, int optionalActions, byte[] attributes);

        [DllImport ("libc", EntryPoint = "cfsetspeed")]
        public static extern int SetSpeed (byte[] attributes, int baudrate);

        public int BaudRate { get; set; }

        public string PortName { get; set; }

        public bool IsOpen { get; set; }

        public event SerialDataReceivedEventHandler DataReceived;

        public int Open () {
            Console.WriteLine("Opening port: " + PortName);
                int handle = Open (this.PortName, OPEN_READ_WRITE);

            if (handle == -1) {
                Console.WriteLine("Opening port 1: " + PortName);
                handle = Open (this.PortName, 1);
            }
            if (handle == -1) {
                Console.WriteLine("Opening port 0: " + PortName);
                handle = Open (this.PortName, 0);
                throw new Exception ($"Could not open port ({this.PortName})");
            }

            SetBaudRate (handle);

            Task.Delay (2000);

            Task.Run (() => StartReading (handle));
            return handle;
        }

        public void Close () {
            Dispose (true);
        }

        public void Write (int handle, byte[] bytes) {

            IntPtr array = Marshal.AllocHGlobal (bytes.Length);
            Marshal.Copy (bytes, 0, array, bytes.Length);
            // Call unmanaged code
            write (handle, array, bytes.Length);
            Marshal.FreeHGlobal (array);
        }

        public string ReadExisting () {
            //Todo: vi må konvertere denne riktig
            return BitConverter.ToInt64 (serialDataBuffer, 0).ToString ();
        }

        public static string[] GetPortNames () {
            var ports = new List<string> ();

            string[] ttyPorts = Directory.GetFiles ("/dev/", "tty*");
            Console.WriteLine("Listing all Dev: ");
            foreach (string port in ttyPorts) {
                Console.WriteLine(port);
                if (port.StartsWith ("/dev/ttyS") || port.StartsWith ("/dev/ttyUSB") || port.StartsWith ("/dev/ttyACM") || port.StartsWith ("/dev/ttyAMA")) {
                    ports.Add (port);
                }
            }

            return ports.ToArray ();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose (bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                }

                IsOpen = false;

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose () {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose (true);
        }
        #endregion

        private void SetBaudRate (int handle) {
            byte[] terminalData = new byte[256];

            GetAttribute (handle, terminalData);
            SetSpeed (terminalData, this.BaudRate);
            SetAttribute (handle, 0, terminalData);
        }

        private void StartReading (int handle) {
            while (true) {
                Array.Clear (serialDataBuffer, 0, serialDataBuffer.Length);

                int lengthOfDataInBuffer = Read (handle, serialDataBuffer, SERIAL_BUFFER_SIZE);

                if (lengthOfDataInBuffer != -1 && !(lengthOfDataInBuffer == 1 && serialDataBuffer[0] == 10)) {
                    DataReceived.Invoke (this, new SerialDataReceivedEventArgs ());
                }

                Task.Delay (50);
            }
        }
    }
}
public delegate void SerialDataReceivedEventHandler (object sender, SerialDataReceivedEventArgs e);
public class SerialDataReceivedEventArgs : EventArgs { }
public interface ISerialPort : IDisposable {
    int BaudRate { get; set; }

    string PortName { get; set; }

    bool IsOpen { get; set; }

    string ReadExisting ();

    int Open ();

    event SerialDataReceivedEventHandler DataReceived;

    void Close ();
}

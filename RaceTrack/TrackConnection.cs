using System;
using System.Threading;
namespace RaceTrack {
    public class TrackConnection {
        private int _handle;

        public TrackConnection () {
            var ports = TrackSerialPort.GetPortNames ();

            foreach (var port in ports) {
                Console.WriteLine ($"Serial port name: {port}");
            }

            var serialPort = new TrackSerialPort () {
                PortName = "/dev/ttyACM0",
                BaudRate = 9600
            };

            // Subscribe to the DataReceived event.
            serialPort.DataReceived += SerialPort_DataReceived;

            // Now open the port.
            _handle = serialPort.Open ();
            var start = GetBytesWithLittleEndian (100);
            var stop = GetBytesWithLittleEndian (0);
            var started = false;
            while (true) {
                var send = started ? stop : start;
                serialPort.Write (_handle, send);
                started = !started;
                Thread.Sleep (2000);
            }
        }

        private static byte[] GetBytesWithLittleEndian (int value) {
            byte[] bytes = BitConverter.GetBytes (value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse (bytes);
            return bytes;
        }

        private static void SerialPort_DataReceived (object sender, SerialDataReceivedEventArgs e) {
            var serialPort = (TrackSerialPort) sender;

            // Read the data that's in the serial buffer.
            var serialdata = serialPort.ReadExisting ();
            // Write to debug output.
            Console.WriteLine ("Vi fikk: " + serialdata);

        }
    }
}
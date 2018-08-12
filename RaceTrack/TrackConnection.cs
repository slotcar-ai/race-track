using System;
using System.Threading;
namespace RaceTrack {
    public class TrackConnection : IDisposable {
        private int _handle;
        private TrackSerialPort _serialPort;

        public TrackConnection () {
            var ports = TrackSerialPort.GetPortNames ();

            foreach (var port in ports) {
                Console.WriteLine ($"Serial port name: {port}");
            }

            var portName = ports.Length > 0 ? ports[0]:  "/dev/ttyACM0";
            _serialPort = new TrackSerialPort () {
                PortName = portName,
                BaudRate = 9600
            };

            // Subscribe to the DataReceived event.
            _serialPort.DataReceived += SerialPort_DataReceived;

            // Now open the port.
            _handle = _serialPort.Open ();
            SetSpeed (0);
        }

        public void SetSpeed (int speed) {
            var data = GetBytesWithLittleEndian (speed);
            _serialPort.Write (_handle, data);

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

        public void Dispose()
        {
            _serialPort.Close();
        }
    }
}
using System;
using System.Linq;
using System.Threading;
namespace RaceTrack
{
    public class TrackConnection : IDisposable
    {
        private int _handle;
        private TrackSerialPort _serialPort;

        public TrackConnection()
        {
            var ports = TrackSerialPort.GetPortNames();

            foreach (var port in ports)
            {
                Console.WriteLine($"Serial port name: {port}");
            }
            var portname = ports.First(port => port.StartsWith("/dev/ttyACM"));
            _serialPort = new TrackSerialPort()
            {
                PortName = portname,
                BaudRate = 9600
            };

            // Subscribe to the DataReceived event.
            _serialPort.DataReceived += SerialPort_DataReceived;

            // Now open the port.
            _handle = _serialPort.Open();
            SetSpeed(0);
            Console.WriteLine("Connected to racetrack");
        }

        public void SetSpeed(int speed)
        {
            Console.WriteLine("Vi sender til arduino: " + speed);
            var data = GetBytesWithLittleEndian(speed);
            Console.WriteLine(String.Join(", ", data));
            Console.WriteLine("");
            _serialPort.Write(_handle, data);
        }

        private static byte[] GetBytesWithLittleEndian(int value)
        {
            var valueInInt16 = (Int16) value;
            byte[] bytes = BitConverter.GetBytes(valueInInt16);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            // var bbb = new byte[1];
            // Array.Copy(bytes, bbb, 1);
            return bytes;
        }

        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (TrackSerialPort)sender;
            // Array.Reverse(serialDataBuffer);
            //Todo: vi må konvertere denne riktig
            //ar speed =  BitConverter.ToInt32 (serialDataBuffer, 0).ToString ();
            var bytes = e.DataReceived;
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            var speed = BitConverter.ToInt16(bytes);
            Console.WriteLine("Vi fikk fra arduino: " + speed +" --> "+string.Join(", ", e.DataReceived));
            // Read the data that's in the serial buffer.
        }

        public void Dispose()
        {
            _serialPort.Close();
        }
    }
}
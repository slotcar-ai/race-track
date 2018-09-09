using System;
using System.Linq;
using System.Threading;
using System.Buffers.Binary;

namespace RaceTrack
{
    public class TrackConnection : IDisposable
    {
        private int _handle;
        private int speed = 0;
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
            if (value > 255) throw new Exception("Vi only support one byte");
            byte[] bytes = BitConverter.GetBytes((Int16)value);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            var byten = new byte[1];
            Array.Copy(bytes, byten, 1);
            return byten;
        }

        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (TrackSerialPort)sender;
            // Array.Reverse(serialDataBuffer);
            //Todo: vi må konvertere denne riktig
            //ar speed =  BitConverter.ToInt32 (serialDataBuffer, 0).ToString ();
            var bytes = e.DataReceived;
            // if (!BitConverter.IsLittleEndian)
            // {
            //     Array.Reverse(bytes);
            // }


            BinaryPrimitives.TryReadInt16LittleEndian(bytes, out Int16 speed);
            Console.WriteLine("Vi fikk fra arduino: " + speed + " --> " + string.Join(", ", e.DataReceived));
        }

        public void Dispose()
        {
            _serialPort.Close();
        }
    }
}
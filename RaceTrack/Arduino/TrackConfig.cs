using Microsoft.Extensions.Configuration;

namespace Scai.RaceTrack.Arduino
{
    public class TrackConfig
    {
        public string PortName { get; set; } = "/dev/ttyACM0";

        // Must match baud rate set on Arduino
        public int BaudRate { get; set; } = 9600;

        // Timeout on read and write operations
        public int Timeout { get; set; } = 500;

        public void BindTo(IConfiguration config)
        {
            config.GetSection("arduinoTrack").Bind(this);
        }
    }
}
using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scai.RaceTrack.Data;

namespace Scai.RaceTrack.Arduino
{
    public class TrackService : IHostedService, IDisposable
    {
        private const char Separator = ',';
        private readonly ILogger _logger;
        private readonly SerialPort _serialPort;
        private readonly CancellationTokenSource _tokenSource;
        private readonly Thread _readThread;
        private readonly Thread _writeThread;

        public TrackService(IConfiguration config, ILogger<TrackService> logger)
        {
            var trackConfig = new TrackConfig();
            trackConfig.BindTo(config);

            _logger = logger;
            _serialPort = new SerialPort
            {
                PortName = trackConfig.PortName, // TODO: Find better solution using SerialPort.GetPortNames()
                BaudRate = trackConfig.BaudRate,
                // Following four values works with default values. Is this always the case?
                // Parity = _serialPort.Parity,
                // DataBits = _serialPort.DataBits,
                // StopBits = _serialPort.StopBits,
                // Handshake = _serialPort.Handshake,
                ReadTimeout = trackConfig.Timeout,
                WriteTimeout = trackConfig.Timeout,
            };
            _tokenSource = new CancellationTokenSource();
            _readThread = new Thread(() => Read(_tokenSource.Token));
            _writeThread = new Thread(() => Write(_tokenSource.Token));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ArduinoService Starting...");

            try
            {
                _serialPort.Open();
            }
            catch (Exception ex)
            {
                var availablePorts = string.Join("\n", SerialPort.GetPortNames());
                _logger.LogError(ex, $"Failed to open serial port to Arduino using \"{_serialPort.PortName}\" configured in appsettings.json.\nIs this the correct port? Available ports:\n{availablePorts}");

                throw;
            }

            _readThread.Start();
            _writeThread.Start();

            await Task.CompletedTask;
        }

        public void SetCarSpeeds(int leftCarSpeed, int rightCarSpeed)
        {
            try
            {
                _serialPort.WriteLine($"{leftCarSpeed}{Separator}{rightCarSpeed}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ArduinoService threw an exception while writing to SerialPort");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ArduinoService Stopping...");

            _tokenSource.Cancel();
            _readThread.Join();
            _writeThread.Join();
            _serialPort.Close();

            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _serialPort.Dispose();
        }

        private void Read(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var message = _serialPort.ReadLine()
                        .Split(Separator)
                        .Select(x => int.Parse(x))
                        .ToArray();

                    if (message.Length != 4)
                    {
                        _logger.LogWarning("Invalid data point received");
                    }
                    else
                    {
                        var dp = new DataPoint(
                            leftCarSpeed: message[0],
                            leftCarPosition: message[2],
                            rightCarSpeed: message[1],
                            rightCarPosition: message[3]);

                        _logger.LogInformation($"lcs: {dp.LeftCar.Speed} lcp: {dp.LeftCar.Position} rcs: {dp.RightCar.Speed} rcp: {dp.RightCar.Position}");
                    }
                }
                catch (TimeoutException)
                {
                    /*
                     * Safe to ignore. From MSDN
                     * The operation did not complete before the time-out period ended.
                     * -or -
                     * No bytes were read.
                     */
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ArduinoService threw an exception while reading from SerialPort");
                }
            }
        }

        private void Write(CancellationToken token)
        {
            Random rnd = new Random();
            while (!token.IsCancellationRequested)
            {
                var leftCarSpeed = rnd.Next(0, 100);
                var rightCarSpeed = rnd.Next(0, 100);
                SetCarSpeeds(leftCarSpeed, rightCarSpeed);
                _logger.LogInformation($"Sent lcs: {leftCarSpeed} rcs: {rightCarSpeed}");
                Thread.Sleep(3000);
            }
        }
    }
}
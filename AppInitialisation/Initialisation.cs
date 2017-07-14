using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AppInitialisation
{
    public class Initialisation
    {
        public bool IsWarm { get; private set; } = false;

        public bool IsWarmingUp { get; private set; } = false;

        public readonly TimeSpan Duration = TimeSpan.FromSeconds(500);
        private ILogger _logger;

        public Initialisation(ILogger<Initialisation> logger)
        {
            _logger = logger;
        }

        public void Initialise()
        {
            if (IsWarm)
            {
                _logger.LogInformation($"{Environment.MachineName} {Process.GetCurrentProcess().Id} already warm.");
            }
            else
            {
                IsWarmingUp = true;
                _logger.LogInformation($"{Environment.MachineName} {Process.GetCurrentProcess().Id} warming up.");
                Task.Delay(Duration).Wait();
                IsWarm = true;
                IsWarmingUp = false;
            }
        }
    }
}
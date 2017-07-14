using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AppInitialisation
{
    [Route("api/resource")]
    public class Controllers : Controller
    {
        private Initialisation _initModule;
        private ILogger _logger;

        public Controllers(Initialisation initModule, ILogger<Controllers> logger)
        {
            _initModule = initModule;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation($"{Environment.MachineName} {Process.GetCurrentProcess().Id} GET api/resource.");

            int count = 0;

            if (!_initModule.IsWarm && !_initModule.IsWarmingUp)
            {
                _initModule.Initialise();
            }

            while (!_initModule.IsWarm)
            {
                _logger.LogInformation($"Not warm yet ({count++})");
                Task.Delay(TimeSpan.FromSeconds(4)).Wait();
            }

            return Json(new { Message = "Cozy and warm!" });
        }

        [HttpGet("initialise")]
        public IActionResult Initialise()
        {
            _logger.LogInformation($"{Environment.MachineName} {Process.GetCurrentProcess().Id} initialising.");

            _initModule.Initialise();

            _logger.LogInformation($"{Environment.MachineName} {Process.GetCurrentProcess().Id} initialised.");

            return Json(new { Message = $"Ready." });
        }
    }
}
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
        private Initialisation _initialisationModule;
        private ILogger _logger;

        public Controllers(Initialisation initialisationModule, ILogger<Controllers> logger)
        {
            _initialisationModule = initialisationModule;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation($"{Environment.MachineName} {Process.GetCurrentProcess().Id} GET api/resource.");

            int count = 0;

            if (!_initialisationModule.IsWarm && !_initialisationModule.IsWarmingUp)
            {
                _initialisationModule.Initialise();
            }

            while (!_initialisationModule.IsWarm)
            {
                _logger.LogInformation($"Not warm yet ({count++})");
                Task.Delay(TimeSpan.FromSeconds(4)).Wait();
            }

            return Json(new { Message = "Cozy and warm!" });
        }

        [HttpGet("signature")]
        public string GetSignature()
        {
            return _initialisationModule.Signature.ToString();
        }

        [HttpGet("initialise")]
        public IActionResult Initialise()
        {
            _logger.LogInformation($"{Environment.MachineName} {Process.GetCurrentProcess().Id} initialising.");

            _initialisationModule.Initialise();

            _logger.LogInformation($"{Environment.MachineName} {Process.GetCurrentProcess().Id} initialised.");

            return Json(new { Message = $"Ready." });
        }
    }
}
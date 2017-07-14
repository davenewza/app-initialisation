using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AppInitialisation
{
    public class InitialisationModule
    {
        public bool IsWarm { get; private set; } = false;

        public bool IsWarmingUp { get; private set; } = false;

        public readonly TimeSpan Duration = TimeSpan.FromSeconds(500);
        private ILogger _logger;

        public InitialisationModule(ILogger<InitialisationModule> logger)
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

    [Route("api/things")]
    public class Controllers : Controller
    {
        private InitialisationModule _initModule;
        private ILogger _logger;

        public Controllers(InitialisationModule initModule, ILogger<Controllers> logger)
        {
            _initModule = initModule;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation($"{Environment.MachineName} {Process.GetCurrentProcess().Id} GET api/things.");

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

            //if (_initModule.IsWarm)
            //{
            //    return Json(new { Message = "Cozy and warm!" });
            //}
            //else
            //{
            //    return Json(new { Message = "I am not warmed up!" });
            //}
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
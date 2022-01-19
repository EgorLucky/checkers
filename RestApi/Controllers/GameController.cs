using DomainLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;

        private readonly GameService _service;

        public GameController(ILogger<GameController> logger, GameService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create()
        {
            var game = await _service.Create();

            return Ok(game);
        }

        [HttpGet("getRegistrationStatus")]
        public async Task<IActionResult> GetRegistrationStatus([FromQuery] Guid gameId)
        {
            var startGameResult = await _service.GetRegistrationStatus(gameId);

            if (startGameResult.Success)
                return Ok(startGameResult);

            return BadRequest(startGameResult);
        }

        [HttpPost("registerSecondPlayer")]
        public async Task<IActionResult> RegisterSecondPlayer([FromBody] Guid gameId)
        {
            var registerSecondUserResult = await _service.RegisterSecondUser(gameId);

            if (registerSecondUserResult.Success)
                return Ok(registerSecondUserResult);

            return BadRequest(registerSecondUserResult);
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start([FromQuery]Guid firstPlayerCode)
        {
            var startGameResult = await _service.Start(firstPlayerCode);

            if(startGameResult.Success)
                return Ok(startGameResult);

            return BadRequest(startGameResult);
        }

        [HttpPost("startWithBot")]
        public async Task<IActionResult> StartWithBot([FromQuery] Guid firstPlayerCode)
        {
            var startGameResult = await _service.StartWithBot(firstPlayerCode);

            if (startGameResult.Success)
                return Ok(startGameResult);

            return BadRequest(startGameResult);
        }

    }
}

using DomainLogic.Models;
using DomainLogic.ParameterModels;
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
        public async Task<IActionResult> Create([FromBody] GameCreateDTO dto)
        {
            var game = await _service.Create(dto);

            return Ok(game);
        }

        [HttpPost("createWithBot")]
        public async Task<IActionResult> CreateWithBot([FromBody] GameCreateDTO dto)
        {
            var game = await _service.CreateWithBot(dto);

            return Ok(game);
        }

        [HttpGet("getInfo")]
        public async Task<IActionResult> GetRegistrationStatus([FromQuery] Guid gameId)
        {
            var startGameResult = await _service.GetInfo(gameId);

            if (startGameResult.Success)
                return Ok(startGameResult);

            return BadRequest(startGameResult);
        }

        [HttpPost("registerSecondPlayer")]
        public async Task<IActionResult> RegisterSecondPlayer([FromBody] Guid gameId)
        {
            var registerSecondUserResult = await _service.RegisterSecondPlayer(gameId);

            if (registerSecondUserResult.Success)
                return Ok(registerSecondUserResult);

            return BadRequest(registerSecondUserResult);
        }

        /// <summary>
        /// Method for second player to set he ready to play
        /// </summary>
        /// <param name="playerCode"></param>
        /// <returns></returns>
        [HttpPost("readyToPlay")]
        public async Task<IActionResult> SetReadyToPlay([FromHeader] Guid playerCode)
        {
            var result = await _service.SetReadyToPlay(playerCode);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] Guid firstPlayerCode)
        {
            var startGameResult = await _service.Start(firstPlayerCode);

            if(startGameResult.Success)
                return Ok(startGameResult);

            return BadRequest(startGameResult);
        }

        [HttpPost("startWithBot")]
        public async Task<IActionResult> StartWithBot([FromBody] Guid firstPlayerCode)
        {
            var startGameResult = await _service.StartWithBot(firstPlayerCode);

            if (startGameResult.Success)
                return Ok(startGameResult);

            return BadRequest(startGameResult);
        }

        [HttpPost("move")]
        public async Task<IActionResult> Move([FromHeader] Guid playerCode, [FromHeader] Guid previousBoardStateId, [FromBody] MoveVector move)
        {
            var moveResult = await _service.Move(playerCode, previousBoardStateId, move);

            return Ok(moveResult);
        }

        [HttpPost("moveWithBot")]
        public async Task<IActionResult> MoveWithBot([FromHeader] Guid playerCode, [FromHeader] Guid previousBoardStateId, [FromBody] MoveVector move)
        {
            var moveResult = await _service.MoveWithBot(playerCode, previousBoardStateId, move);

            return Ok(moveResult);
        }
    }
}

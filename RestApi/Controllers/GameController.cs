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
    }
}

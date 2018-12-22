using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bot.Controllers {

    [Route("skill")]
    public class SkillController : ControllerBase {

        protected ILogger<SkillController> Logger { get; }

        public SkillController(
            ILogger<SkillController> logger
        ) {
            Logger = logger;
        }

        [HttpPost]
        public IActionResult Process([FromBody] SkillRequest request) {
            Logger.LogInformation("Request: {0}", request);

            SkillResponse response = ResponseBuilder.Tell("Ciao Boba, ti amo!");

            return Ok(response);
        }

    }

}

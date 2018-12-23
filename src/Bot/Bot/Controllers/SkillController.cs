using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Bot.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bot.Controllers {

    [Route("skill")]
    public class SkillController : ControllerBase {

        protected DatabaseManager Database { get; }
        protected ILogger<SkillController> Logger { get; }

        public SkillController(
            DatabaseManager database,
            ILogger<SkillController> logger
        ) {
            Database = database;
            Logger = logger;
        }

        [HttpPost]
        public IActionResult Process([FromBody] SkillRequest request) {
            Logger.LogInformation("Incoming request type '{0}'", request.Request.Type);

            if(!request.Request.Locale.Equals("it-IT", StringComparison.InvariantCultureIgnoreCase)) {
                return UnsupportedLanguage();
            }

            switch(request.Request.Type) {
                case RequestTypes.LaunchRequest:
                    return NewSession(request);
            }

            return DidNotUnderstand();
        }

        private IActionResult NewSession(SkillRequest request) {
            var sessionExists = Database.Context.Moves
                .Where(s => s.AlexaSessionId == request.Session.SessionId)
                .SingleOrDefault() != null;

            if(sessionExists) {
                Logger.LogError("New session with existing session value {0}", request.Session.SessionId);
                return InternalError();
            }

            var response = ResponseBuilder.Ask(new SsmlOutputSpeech {
                    Ssml = @"<speak>Ciao da <emphasis level=""moderate"">Cody Maze</emphasis>.
Posiziònati su uno spazio lungo il bordo della scacchiera.
Poi dimmi le coordinate che hai scelto.</speak>"
            },
                new Reprompt {
                    OutputSpeech = new SsmlOutputSpeech {
                        Ssml = @"<speak>Quali sono le <emphasis level=""moderate"">coordinate</emphasis> dello spazio in cui sei?</speak>"
                    }
                }
            );
            return Ok(response);
        }

        private IActionResult UnsupportedLanguage() {
            return Ok(ResponseBuilder.Tell("I do not speak that language yet, sorry"));
        }

        private IActionResult DidNotUnderstand() {
            return Ok(ResponseBuilder.Ask("Non ho capito", null));
        }

        private IActionResult InternalError(string msg = null) {
            return Ok(ResponseBuilder.Tell(msg ?? "C'è stato un errore, riprova"));
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
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
            Logger.LogDebug("Incoming request, type: '{0}'", request.Request.Type);

            if(!request.Request.Locale.Equals("it-IT", StringComparison.InvariantCultureIgnoreCase)) {
                return UnsupportedLanguage();
            }

            switch(request.Request) {
                case LaunchRequest lr:
                    Logger.LogInformation(LoggingEvents.Requests, "Incoming Launch Request");
                    return NewSession(request, lr);

                case SessionEndedRequest er:
                    return DoNothing();

                case IntentRequest ir:
                    Logger.LogInformation(LoggingEvents.Requests, "Incoming Intent Request, name '{0}'", ir.Intent.Name);
                    switch(ir.Intent.Name) {
                        case IntentTypes.ReachCell:
                            return ReachCell(request, ir);
                        case IntentTypes.GiveDirection:
                            return GiveDirection(request, ir);
                    }
                    break;
            }

            return DidNotUnderstand();
        }

        private IActionResult NewSession(SkillRequest request, LaunchRequest launchRequest) {
            var sessionExists = Database.Context.Moves
                .Where(s => s.AlexaSessionId == request.Session.SessionId)
                .SingleOrDefault() != null;

            if(sessionExists) {
                Logger.LogError("New session with existing session value {0}", request.Session.SessionId);
                return InternalError();
            }

            var response = ResponseBuilder.Ask(
                new SsmlOutputSpeech {
                    Ssml = @"<speak>Ciao da <emphasis level=""moderate"">Cody Maze</emphasis>.
Raggiungi uno spazio lungo il bordo della scacchiera.
Poi dimmi le coordinate che hai scelto.</speak>"
                },
                new Reprompt {
                    OutputSpeech = new SsmlOutputSpeech {
                        Ssml = @"<speak>Quali sono le <emphasis level=""moderate"">coordinate</emphasis> dello spazio in cui ti trovi?</speak>"
                    }
                }
            );
            return Ok(response);
        }

        private IActionResult ReachCell(SkillRequest request, IntentRequest intent) {
            string sCol = intent.Intent.Slots["column"]?.Value;
            string sRow = intent.Intent.Slots["row"]?.Value;
            string sDir = intent.Intent.Slots["direction"]?.Value;

            var coords = new Coordinates(sCol, sRow, sDir);

            return ProcessStep(request, coords);
        }

        private IActionResult GiveDirection(SkillRequest request, IntentRequest intent) {
            string sDir = intent.Intent.Slots["direction"]?.Value;

            var session = request.Session;
            int iRow = session.Attributes.SafeGet("row", -1);
            int iCol = session.Attributes.SafeGet("col", -1);

            var coords = new Coordinates(iCol, iRow, sDir);

            return ProcessStep(request, coords);
        }

        private IActionResult ProcessStep(SkillRequest request, Coordinates target) {
            Logger.LogInformation(LoggingEvents.Game, "User reaches coordinates {0}", target);

            if (!target.IsValid) {
                // Something went wrong
                Logger.LogError(LoggingEvents.Game, "Invalid coordinates");

                var response = ResponseBuilder.Ask(
                    new SsmlOutputSpeech {
                        Ssml = @"<speak>Non ho capito. Ripeti le coordinate per favore.</speak>"
                    },
                    new Reprompt {
                        OutputSpeech = new SsmlOutputSpeech {
                            Ssml = @"<speak>Quali sono le <emphasis level=""moderate"">coordinate</emphasis> dello spazio in cui ti trovi?</speak>"
                        }
                    }
                );
                return Ok(response);
            }
            if (!target.Direction.HasValue) {
                // Register coordinates in session and ask for direction
                Session session = request.Session;
                if (session.Attributes == null)
                    session.Attributes = new Dictionary<string, object>();
                session.Attributes["row"] = target.Row;
                session.Attributes["col"] = target.Column;

                var response = ResponseBuilder.Ask(
                    new SsmlOutputSpeech {
                        Ssml = @"<speak>OK. In che direzione stai guardando?</speak>"
                    },
                    new Reprompt {
                        OutputSpeech = new SsmlOutputSpeech {
                            Ssml = @"<speak>Dimmi in che <emphasis level=""moderate"">direzione</emphasis> stai guardando.</speak>"
                        }
                    },
                    session
                );
                return Ok(response);
            }

            return Ok();
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

        private IActionResult DoNothing() {
            return Ok(ResponseBuilder.Empty());
        }

    }

}

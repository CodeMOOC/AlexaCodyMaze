using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public Task<IActionResult> Process([FromBody] SkillRequest request) {
            Logger.LogDebug("Incoming request, type: '{0}'", request.Request.Type);

            if(!request.Request.Locale.Equals("it-IT", StringComparison.InvariantCultureIgnoreCase)) {
                return UnsupportedLanguage();
            }

            switch(request.Request) {
                case LaunchRequest lr:
                    Logger.LogInformation(LoggingEvents.Requests, "Incoming Launch Request");
                    return NewSession(request, lr);

                case SessionEndedRequest er:
                    return SayGoodbye();

                case IntentRequest ir:
                    Logger.LogInformation(LoggingEvents.Requests, "Incoming Intent Request, name '{0}'", ir.Intent.Name);
                    switch(ir.Intent.Name) {
                        case IntentTypes.ReachCell:
                            return ReachCell(request, ir);
                        case IntentTypes.GiveDirection:
                            return GiveDirection(request, ir);
                        case IntentTypes.Stop:
                            return SayGoodbye();
                    }
                    break;
            }

            return DidNotUnderstand();
        }

        private Task<IActionResult> NewSession(SkillRequest request, LaunchRequest launchRequest) {
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
            return Task.FromResult<IActionResult>(Ok(response));
        }

        private Task<IActionResult> ReachCell(SkillRequest request, IntentRequest intent) {
            string sCol = intent.Intent.Slots["column"]?.Value;
            string sRow = intent.Intent.Slots["row"]?.Value;
            string sDir = intent.Intent.Slots["direction"]?.Value;

            var coords = new Coordinates(sCol, sRow, sDir);

            return ProcessStep(request, coords);
        }

        private Task<IActionResult> GiveDirection(SkillRequest request, IntentRequest intent) {
            string sDir = intent.Intent.Slots["direction"]?.Value;

            var session = request.Session;
            int iRow = (int)session.Attributes.SafeGet("row", (long)-1);
            int iCol = (int)session.Attributes.SafeGet("col", (long)-1);

            var coords = new Coordinates(iCol, iRow, sDir);

            return ProcessStep(request, coords);
        }

        private async Task<IActionResult> ProcessStep(SkillRequest request, Coordinates coords) {
            Session session = request.Session;
            var state = new State(Database.Context, session);

            Logger.LogInformation(LoggingEvents.Game, "User reaches coordinates {0}", coords);
            Logger.LogTrace(LoggingEvents.Game, "{0} moves, last reached {1}", state.MovesCount, state.LastReached);

            if (!coords.IsValid) {
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

            if(state.IsSessionStart) {
                // First position, can ignore direction
                if(Chessboard.IsStartPosition(coords)) {
                    Direction startDir = Chessboard.GetStartDirection(coords);
                    Logger.LogDebug(LoggingEvents.Game, "User in {0} should look towards {1}", coords, startDir);

                    Coordinates effectiveTarget = new Coordinates(coords.Column, coords.Row, startDir);
                    state.RecordDestination(effectiveTarget, true);

                    if (effectiveTarget.Direction != coords.Direction) {
                        // Hint at correct direction
                        var progResp = new ProgressiveResponse(request);
                        await progResp.SendSpeech(string.Format(
                            "OK. Sei in {0}, {1}. Assicurati di girarti verso {2}.",
                            effectiveTarget.Column.FromColumnIndex(),
                            effectiveTarget.Row,
                            effectiveTarget.Direction.Value.ToLocale()
                        ));

                        coords = effectiveTarget;
                    }
                }
                else {
                    Logger.LogDebug(LoggingEvents.Game, "User in {0}, not a valid starting position", coords);

                    return Ok(ResponseBuilder.Ask("Devi partire in uno spazio sul bordo della scacchiera. Spostati e dimmi dove sei.", null));
                }
            }
            else if (!coords.Direction.HasValue) {
                // Not first position, requires direction:
                // register coordinates in session and ask for direction
                if (session.Attributes == null)
                    session.Attributes = new Dictionary<string, object>();
                session.Attributes["row"] = coords.Row;
                session.Attributes["col"] = coords.Column;

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

            session.Attributes = new Dictionary<string, object>(); // clean up session attributes

            // Check for destination to reach
            var destination = state.LastDestination;
            if (destination.HasValue) {
                if (destination.Value.Equals(coords)) {
                    Logger.LogInformation(LoggingEvents.Game, "Correct cell");

                    state.ReachDestination();

                    var progResp = new ProgressiveResponse(request);
                    await progResp.SendSpeech("Sei nel posto giusto!");
                }
                else if (destination.Value.LocationEquals(coords)) {
                    Logger.LogInformation(LoggingEvents.Game, "Wrong direction, expected {0}", destination.Value);

                    var response = ResponseBuilder.Ask("Stai guardando nella direzione sbagliata. Riprova.", null, session);
                    return Ok(response);
                }
                else {
                    Logger.LogInformation(LoggingEvents.Game, "Wrong cell, expected {0}", destination.Value);

                    var response = ResponseBuilder.Ask("Purtroppo sei nella casella sbagliata. Riprova.", null, session);
                    return Ok(response);
                }
            }

            if(state.MovesCount >= Chessboard.MaxLevel) {
                Logger.LogInformation(LoggingEvents.Game, "User completed last level");

                return Ok(ResponseBuilder.Tell(
                    new SsmlOutputSpeech {
                        Ssml = @"<speak>Complimenti! Hai completato <emphasis level=""moderate"">Cody Maze</emphasis> con successo.</speak>"
                    }
                 ));
            }

            // Assign new destination
            Logger.LogInformation(LoggingEvents.Game, "Generating new destination (step {0})", state.MovesCount);
            (var mazeDestination, var instructions) = Chessboard.GenerateMazeForLevel(coords, state.MovesCount);
            if(!mazeDestination.IsValid) {
                Logger.LogError(LoggingEvents.Game, "Invalid coordinates generated for maze level {0}", state.MovesCount);
                return await InternalError();
            }
            state.RecordDestination(mazeDestination);

            return Ok(ResponseBuilder.Ask(
                new SsmlOutputSpeech {
                    Ssml = string.Format(
                        @"<speak>Esegui le seguenti istruzioni{1}. <emphasis level=""strong"">{0}</emphasis>.</speak>",
                        instructions,
                        (state.MovesCount <= 2) ? " e poi dimmi le tue coordinate e direzione" : string.Empty
                    )
                }, new Reprompt {
                    OutputSpeech = new SsmlOutputSpeech {
                        Ssml = string.Format(
                            @"<speak>Esegui le istruzioni. <emphasis level=""strong"">{0}</emphasis>.</speak>",
                            instructions
                        )
                    }
                },
                session
            ));
        }

        private Task<IActionResult> UnsupportedLanguage() {
            return Task.FromResult<IActionResult>(Ok(ResponseBuilder.Tell("I do not speak that language yet, sorry")));
        }

        private Task<IActionResult> DidNotUnderstand() {
            return Task.FromResult<IActionResult>(Ok(ResponseBuilder.Ask("Non ho capito", null)));
        }

        private Task<IActionResult> InternalError(string msg = null) {
            return Task.FromResult<IActionResult>(Ok(ResponseBuilder.Tell(msg ?? "C'è stato un errore, riprova")));
        }

        private Task<IActionResult> DoNothing() {
            return Task.FromResult<IActionResult>(Ok(ResponseBuilder.Empty()));
        }

        private Task<IActionResult> SayGoodbye() {
            return Task.FromResult<IActionResult>(Ok(ResponseBuilder.Tell("Ciao!")));
        }

    }

}

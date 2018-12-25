using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Bot.Data;

namespace Bot {

    public class State {

        private readonly DataContext _context;
        private readonly Session _session;
        private List<Move> _moves = new List<Move>();

        public State(DataContext data, Session alexaSession) {
            _context = data;
            _session = alexaSession;

            _moves = data.Moves
                .Where(s => s.AlexaSessionId == alexaSession.SessionId)
                .OrderBy(s => s.CreationTime)
                .ToList();
        }

        public bool IsSessionStart {
            get => _moves.Count == 0;
        }

        public int MovesCount {
            get => _moves.Count;
        }

        public Coordinates? LastReached {
            get {
                if (_moves.Count == 0)
                    return null;

                Move lastMove = _moves.LastOrDefault(m => m.ReachedOn.HasValue);
                if (lastMove != null)
                    return new Coordinates(lastMove.Coordinates, lastMove.Direction);
                else
                    return null;
            }
        }

        public Coordinates? LastDestination {
            get {
                if (_moves.Count == 0)
                    return null;

                Move lastMove = _moves.SingleOrDefault(m => !m.ReachedOn.HasValue);
                if (lastMove != null)
                    return new Coordinates(lastMove.Coordinates, lastMove.Direction);
                else
                    return null;
            }
        }

        public void RecordDestination(Coordinates target, bool isReached = false) {
            if (!target.Direction.HasValue)
                throw new ArgumentException("Coordinates must have direction to be stored");

            Move m = new Move {
                AlexaSessionId = _session.SessionId,
                AlexaUserId = _session.User.UserId,
                Coordinates = string.Format("{0}{1}", target.Column.FromColumnIndex(), target.Row),
                Direction = target.Direction.Value.ToChar(),
                CreationTime = DateTime.UtcNow
            };
            if(isReached) {
                m.ReachedOn = m.CreationTime;
            }

            _moves.Add(m);
            _context.Add(m);
            _context.SaveChanges();
        }

        public void ReachDestination() {
            if (_moves.Count == 0) {
                throw new InvalidOperationException("No destination to reach");
            }

            Move destination = _moves.Last();
            if(destination.ReachedOn.HasValue) {
                throw new InvalidOperationException("Current destination already reached");
            }

            destination.ReachedOn = DateTime.UtcNow;
            _context.Update(destination);
            _context.SaveChanges();
        }

    }

}

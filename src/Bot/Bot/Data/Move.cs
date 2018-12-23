using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Data {

    public class Move {

        public ulong Id { get; set; }
        public string AlexaSessionId { get; set; }
        public string AlexaUserId { get; set; }
        public string Coordinates { get; set; }
        public char? Directions { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? ReachedOn { get; set; }

    }

}

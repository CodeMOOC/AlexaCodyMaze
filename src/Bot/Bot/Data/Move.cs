using System;

namespace Bot.Data {

    public class Move {

        public int Id { get; set; }
        public string AlexaSessionId { get; set; }
        public string AlexaUserId { get; set; }
        public string Coordinates { get; set; }
        public char? Direction { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? ReachedOn { get; set; }

    }

}

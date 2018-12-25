using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot {

    public static class Chessboard {

        private static readonly bool[,] Stars = new bool[5, 5] {
            { true , true , false, false, false },
            { false, false, false, false, true  },
            { false, false, false, false, false },
            { true , false, false, false, false },
            { false, false, true , false, false }
        };

        public static bool HasStar(Coordinates c) {
            if (!c.IsValid)
                return false;
            return Stars[c.Row, c.Column];
        }

        public static bool IsStartPosition(Coordinates c) {
            if (c.Column == 1)
                return true;
            else if(c.Column == 5)
                return true;
            else {
                if (c.Row == 1)
                    return true;
                else if (c.Row == 5)
                    return true;
                else
                    return false;
            }
        }

        public static Direction GetStartDirection(Coordinates c) {
            if (c.Column == 1)
                return Direction.East;
            else if (c.Column == 5)
                return Direction.West;
            else {
                if (c.Row == 1)
                    return Direction.South;
                else if (c.Row == 5)
                    return Direction.North;
                else
                    throw new ArgumentException("Invalid start position", nameof(c));
            }
        }

    }

}

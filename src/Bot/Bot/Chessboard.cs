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

        public const int Width = 5;
        public const int Height = 5;

        public const int MinX = 1;
        public const int MaxX = 5;
        public const int MinY = 1;
        public const int MaxY = 5;

        public static bool HasStar(Coordinates c) {
            if (!c.IsValid)
                return false;
            return Stars[c.Row, c.Column];
        }

        public static bool IsStartPosition(Coordinates c) {
            if (c.Column == 1)
                return true;
            else if (c.Column == 5)
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

        public const int MaxLevel = 5;

        public static (Coordinates Destination, string Instructions) GenerateMazeForLevel(Coordinates start, int level) {
            if (level > 5) {
                return (start, string.Empty);
            }

            switch (level) {
                case 1:
                    return GenerateMazeForLevel1(start);

                case 2:
                    return GenerateMazeForLevel2(start);

                case 3:
                    return GenerateMazeForLevel1(start);

                case 4:
                    return GenerateMazeForLevel4(start);

                case 5:
                    return GenerateMazeForLevel5(start);
            }

            throw new ArgumentException("Invalid level", nameof(level));
        }

        private static (Coordinates Destination, string Instructions) GenerateMazeForLevel1(Coordinates start) {
            if(start.FreeAhead() < 1) {
                throw new InvalidOperationException("Cannot generate level 1 maze without free space ahead");
            }

            return (start.Advance(1), "avanti");
        }

        private static (Coordinates Destination, string Instructions) GenerateMazeForLevel2(Coordinates start) {
            if(start.TurnLeft().FreeAhead() > 0) {
                return (start.TurnLeft(), "sinistra");
            }
            else {
                return (start.TurnRight(), "destra");
            }
        }

        private static (Coordinates Destination, string Instructions) GenerateMazeForLevel4(Coordinates start) {
            if (start.TurnLeft().FreeAhead() >= 3) {
                return (start.TurnLeft().Advance(), "sinistra avanti");
            }
            else {
                return (start.TurnRight().Advance(), "destra avanti");
            }
        }

        private static (Coordinates Destination, string Instructions) GenerateMazeForLevel5(Coordinates start) {
            if (start.FreeAhead() < 2)
                throw new InvalidOperationException("Cannot generate level 5 maze without 2 spaces ahead");

            return (start.Advance().Advance(), "2 volte avanti");
        }

    }
}

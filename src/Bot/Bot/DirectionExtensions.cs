using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot {

    public static class DirectionExtensions {

        public static char ToChar(this Direction d) {
            switch(d) {
                default:
                case Direction.North:
                    return 'n';
                case Direction.East:
                    return 'e';
                case Direction.South:
                    return 's';
                case Direction.West:
                    return 'w';
            }
        }

        public static Direction? ToDirection(this string s) {
            var trimmed = (s ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) {
                return null;
            }

            switch(trimmed.ToLowerInvariant()) {
                case "nord":
                    return Direction.North;
                case "est":
                    return Direction.East;
                case "sud":
                    return Direction.South;
                case "ovest":
                    return Direction.West;
            }

            return null;
        }

        public static Direction ToDirection(this char c) {
            switch(char.ToLowerInvariant(c)) {
                default:
                case 'n':
                    return Direction.North;
                case 'e':
                    return Direction.East;
                case 's':
                    return Direction.South;
                case 'w':
                    return Direction.West;
            }
        }

        public static Direction ToLeft(this Direction d) {
            switch(d) {
                default:
                case Direction.North:
                    return Direction.West;
                case Direction.East:
                    return Direction.North;
                case Direction.South:
                    return Direction.East;
                case Direction.West:
                    return Direction.South;
            }
        }

        public static Direction ToRight(this Direction d) {
            switch (d) {
                default:
                case Direction.North:
                    return Direction.East;
                case Direction.East:
                    return Direction.South;
                case Direction.South:
                    return Direction.West;
                case Direction.West:
                    return Direction.North;
            }
        }

        public static string ToLocale(this Direction d) {
            switch(d) {
                default:
                case Direction.North:
                    return "nord";
                case Direction.East:
                    return "est";
                case Direction.South:
                    return "sud";
                case Direction.West:
                    return "ovest";
            }
        }

    }

}

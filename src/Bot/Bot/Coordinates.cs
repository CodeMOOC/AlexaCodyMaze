using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot {

    public struct Coordinates : IEquatable<Coordinates> {

        public int Column { get; }
        public int Row { get; }
        public Direction? Direction { get; }

        public Coordinates(string col, string row) {
            Column = col.ToColumnIndex();
            Row = row.ToRowIndex();
            Direction = null;
        }

        public Coordinates(string col, string row, string direction) {
            Column = col.ToColumnIndex();
            Row = row.ToRowIndex();
            Direction = direction.ToDirection();
        }

        public Coordinates(int col, int row, string direction) {
            Column = col;
            Row = row;
            Direction = direction.ToDirection();
        }

        public Coordinates(int col, int row, Direction direction) {
            Column = col;
            Row = row;
            Direction = direction;
        }

        public Coordinates(string coords, char? direction) {
            if (coords.Length != 2)
                throw new ArgumentException("Coordinates must be two characters long", nameof(coords));

            Column = coords[0].ToColumnIndex();
            Row = coords[1].ToRowIndex();
            if (direction.HasValue) {
                Direction = direction.Value.ToDirection();
            }
            else {
                Direction = null;
            }
        }

        public bool IsValid {
            get => (Column >= 1 && Column <= 5 && Row >= 1 && Row <= 5);
        }

        #region Operations

        public Coordinates TurnLeft() {
            if (!Direction.HasValue)
                throw new InvalidOperationException("Cannot turn without direction");
            return new Coordinates(Column, Row, Direction.Value.ToLeft());
        }

        public Coordinates TurnRight() {
            if (!Direction.HasValue)
                throw new InvalidOperationException("Cannot turn without direction");
            return new Coordinates(Column, Row, Direction.Value.ToRight());
        }

        public Coordinates Advance(int steps = 1) {
            if (!Direction.HasValue)
                throw new InvalidOperationException("Cannot advance without direction");

            switch(Direction.Value) {
                case Bot.Direction.North:
                    return new Coordinates(Column, Row - steps, Direction.Value);
                case Bot.Direction.East:
                    return new Coordinates(Column + steps, Row, Direction.Value);
                case Bot.Direction.South:
                    return new Coordinates(Column, Row + steps, Direction.Value);
                case Bot.Direction.West:
                    return new Coordinates(Column - steps, Row, Direction.Value);
            }

            throw new InvalidOperationException("Invalid direction");
        }

        public int FreeAhead() {
            if (!Direction.HasValue)
                throw new InvalidOperationException("Cannot advance without direction");

            switch(Direction.Value) {
                case Bot.Direction.North:
                    return (Row - 1).Clamp(0, Chessboard.Height - 1);
                case Bot.Direction.East:
                    return (Chessboard.MaxX - Column).Clamp(0, Chessboard.Width - 1);
                case Bot.Direction.South:
                    return (Chessboard.MaxY - Row).Clamp(0, Chessboard.Height - 1);
                case Bot.Direction.West:
                    return (Column - 1).Clamp(0, Chessboard.Width - 1);
            }

            return 0;
        }

        #endregion

        public bool Equals(Coordinates other) {
            return (
                Column == other.Column &&
                Row == other.Row &&
                Direction == other.Direction
            );
        }

        public bool LocationEquals(Coordinates other) {
            return (
                Column == other.Column &&
                Row == other.Row
            );
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if(obj is Coordinates) {
                return Equals((Coordinates)obj);
            }

            return false;
        }

        public override int GetHashCode() {
            int h = Column * Row;
            if (Direction.HasValue)
                h *= (int)Direction.Value;
            return h.GetHashCode();
        }

        public override string ToString() {
            if (Direction.HasValue) {
                return string.Format("({0},{1},{2})", Column, Row, Direction);
            }
            else {
                return string.Format("({0},{1})", Column, Row);
            }
        }

        public static bool operator ==(Coordinates a, Coordinates b) {
            return a.Equals(b);
        }

        public static bool operator !=(Coordinates a, Coordinates b) {
            return !a.Equals(b);
        }

    }

}

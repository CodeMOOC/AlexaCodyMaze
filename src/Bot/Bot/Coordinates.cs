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

            return string.Format("({0},{1})", Column, Row);
        }

        public static bool operator ==(Coordinates a, Coordinates b) {
            return a.Equals(b);
        }

        public static bool operator !=(Coordinates a, Coordinates b) {
            return !a.Equals(b);
        }

    }

}

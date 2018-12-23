using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot {

    public class Coordinates {

        public int Column { get; set; }
        public int Row { get; set; }
        public Direction? Direction { get; set; }

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

        public bool IsValid {
            get => (Column >= 1 && Column <= 5 && Row >= 1 && Row <= 5);
        }

        public override string ToString() {
            if (Direction.HasValue) {
                return string.Format("({0},{1},{2})", Column, Row, Direction);
            }

            return string.Format("({0},{1})", Column, Row);
        }

    }

}

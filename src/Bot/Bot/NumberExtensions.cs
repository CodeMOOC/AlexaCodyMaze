using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot {

    public static class NumberExtensions {

        public static int Clamp(this int n, int min, int max) {
            if (n < min)
                return min;
            else if (n > max)
                return max;
            else
                return n;
        }

        public static int BoardClampX(this int n) {
            return Clamp(n, Chessboard.MinX, Chessboard.MaxX);
        }

        public static int BoardClampY(this int n) {
            return Clamp(n, Chessboard.MinY, Chessboard.MaxY);
        }

    }

}

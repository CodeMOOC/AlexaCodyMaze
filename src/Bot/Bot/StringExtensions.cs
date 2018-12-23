using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot {

    public static class StringExtensions {

        public static int ToColumnIndex(this string s) {
            var trimmed = (s ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) {
                return -1;
            }

            var firstChar = s.ToLowerInvariant()[0];
            switch(firstChar) {
                case 'a':
                    return 1;
                case 'b':
                    return 2;
                case 'c':
                    return 3;
                case 'd':
                    return 4;
                case 'e':
                    return 5;
            }

            return -1;
        }

        public static int ToRowIndex(this string s) {
            var trimmed = (s ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) {
                return -1;
            }

            if (int.TryParse(s, out int index))
                return index;
            else
                return -1;
        }

    }

}

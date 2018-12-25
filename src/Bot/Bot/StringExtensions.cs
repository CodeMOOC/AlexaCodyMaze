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

            return s.ToLowerInvariant()[0].ToColumnIndex();
        }

        public static int ToColumnIndex(this char c) {
            switch (c) {
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

        public static char FromColumnIndex(this int i) {
            switch(i) {
                case 1:
                    return 'a';
                case 2:
                    return 'b';
                case 3:
                    return 'c';
                case 4:
                    return 'd';
                case 5:
                    return 'e';
            }

            throw new ArgumentException("Invalid column index", nameof(i));
        }

        public static int ToRowIndex(this string s) {
            var trimmed = (s ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) {
                return -1;
            }

            return trimmed[0].ToRowIndex();
        }

        public static int ToRowIndex(this char c) {
            // This looks wonky but should be expanded for larger chessboards
            switch (c) {
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
            }

            return -1;
        }

    }

}

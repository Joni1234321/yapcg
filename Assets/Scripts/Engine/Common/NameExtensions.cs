using System;
using System.Globalization;
using Unity.Collections;

namespace YAPCG.Engine.Common
{
    public static class NameExtensions
    {
        // Define the mapping of integers to Roman numerals
        private static readonly (int Value, string Symbol)[] ROMAN_NUMERALS = {
            (1000, "M"),
            (900, "CM"),
            (500, "D"),
            (400, "CD"),
            (100, "C"),
            (90, "XC"),
            (50, "L"),
            (40, "XL"),
            (10, "X"),
            (9, "IX"),
            (5, "V"),
            (4, "IV"),
            (1, "I")
        };

        public static FixedString64Bytes ToRoman(this int num)
        {
            if (num < 1 || num > 3999)
                throw new ArgumentOutOfRangeException(nameof(num), "Value must be in the range 1-3999.");

            FixedString64Bytes romans = "";
            foreach (var (value, symbol) in ROMAN_NUMERALS)
            {
                while (num >= value)
                {
                    romans += symbol;
                    num -= value;
                }
            }

            return romans;

        } 
        public static string ToSuperscript(this string s) => $"<sup>{s}</sup>";
        public static string ToSubscript(this string s) => $"<sub>{s}</sub>";

        public static string ToScientific(this float value, string postfix = "")
        {
            // Convert the value to scientific notation with three decimal places
            string scientificString = value.ToString("0.###E0", CultureInfo.CurrentCulture);

            // Split the scientific notation into base and exponent
            string[] parts = scientificString.Split('E');

            // Format the string as "base x 10^exponent"
            string formattedString = $"{parts[0]}\u00d710{parts[1].ToSuperscript()} {postfix}";

            return formattedString;
        }

    }
}
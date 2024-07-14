using System;
using System.Globalization;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

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
        
        private static readonly char[] METRIC_SUFFIXES = { ' ', 'K', 'M', 'G', 'T', 'P', 'E' };


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
        
        [BurstCompile]
        public static string ToMetric(this float value, string postfix = "")
        {
            int magnitude = (int)math.floor(math.log10(math.abs(value)) / 3);
            float scaledValue = value / math.pow(10, magnitude * 3);

            if (magnitude < 0 || magnitude >= METRIC_SUFFIXES.Length)
                return value.ToString($"0.###E0{postfix}", CultureInfo.CurrentCulture); // Fall back to scientific notation if out of range

            return $"{scaledValue:0.00} {METRIC_SUFFIXES[magnitude]}{postfix}"; 
        }
    }
}
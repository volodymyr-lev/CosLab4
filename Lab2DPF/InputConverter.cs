using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2DPF
{
    internal static class InputConverter
    {
        public static int GetN(string input) => int.TryParse(input, out var result) ? result : -1;

        public static (double i1, double i2) GetIntervals(string i1, string i2)
        {
            if (string.IsNullOrWhiteSpace(i1) || string.IsNullOrWhiteSpace(i2))
                return (double.MinValue, double.MinValue);

            try
            {
                double val1 = ParseWithPi(i1);
                double val2 = ParseWithPi(i2);

                return (val1, val2);
            }
            catch (Exception ex)
            {
                return (double.MinValue, double.MinValue);
            }
        }

        private static double ParseWithPi(string input)
        {
            input = input.Replace(" ", "").ToLower();

            if (input.Contains("pi"))
            {
                string numberPart = input.Replace("pi", "");
                double coefficient = string.IsNullOrEmpty(numberPart) ? 1 : double.Parse(numberPart);
                return coefficient * Math.PI;
            }

            return double.Parse(input);
        }
    }
}

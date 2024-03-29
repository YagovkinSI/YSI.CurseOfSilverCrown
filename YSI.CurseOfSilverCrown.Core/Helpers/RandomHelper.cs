﻿using System;

namespace YSI.CurseOfSilverCrown.Core.Helpers
{
    public static class RandomHelper
    {
        private static readonly Random random = new();

        public static decimal AddRandom(decimal number, int percents = 10, double? randomNumber = null, int roundRequest = 0)
        {
            var randomNumberValue = !randomNumber.HasValue || randomNumber.Value < 0 || randomNumber.Value > 1
                ? random.NextDouble()
                : randomNumber.Value;

            var percentsDouble = Math.Max(Math.Min(percents / 100.0M, 1.0M), 0.01M);

            var resulPercent = 1.0M - percentsDouble * (1.0M - (decimal)randomNumberValue * 2.0M);
            var result = number * resulPercent;

            if (roundRequest > 0)
            {
                result = Math.Round(result, roundRequest);
            }
            else if (roundRequest < 0)
            {
                var round = (int)Math.Pow(10, -roundRequest);
                result = (int)result / round * round;
            }
            return result;
        }

        public static int AddRandom(int number, int percents = 10, double? randomNumber = null, int roundRequest = 0)
        {
            var result = AddRandom((decimal)number, percents, randomNumber, roundRequest);
            return (int)result;
        }

        public static double AddRandom(double number, int percents = 10, double? randomNumber = null, int roundRequest = 0)
        {
            var result = AddRandom((decimal)number, percents, randomNumber, roundRequest);
            return (double)result;
        }

        public static double DependentRandom(int id, int parameterNumber)
        {
            return (id + parameterNumber) * id % 10 / 10.0;
        }

        public static int Random2d6()
        {
            return random.Next(1, 6) + random.Next(1, 6);
        }
    }
}

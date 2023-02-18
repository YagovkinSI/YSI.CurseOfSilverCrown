﻿namespace YSI.CurseOfSilverCrown.Core.Helpers
{
    public static class FortificationsHelper
    {
        private const int GarrisonPlaceCost = 2;

        public static int GetFortCoef(int fortifications)
        {
            return fortifications / GarrisonPlaceCost;
        }
    }
}

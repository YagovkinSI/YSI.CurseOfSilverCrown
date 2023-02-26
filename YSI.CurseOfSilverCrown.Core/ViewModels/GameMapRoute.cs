﻿using YSI.CurseOfSilverCrown.Core.MainModels.Domains;

namespace YSI.CurseOfSilverCrown.Core.ViewModels
{
    public class GameMapRoute
    {
        public Domain TargetDomain { get; set; }
        public int Distance { get; set; }

        public string RouteName => $"{TargetDomain.Name} ({Distance})";

        public GameMapRoute(Domain targetDomain, int disatanse)
        {
            TargetDomain = targetDomain;
            Distance = disatanse;
        }

    }
}

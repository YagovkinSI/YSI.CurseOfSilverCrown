﻿using YSI.CurseOfSilverCrown.Core.MainModels.GameEvent;

namespace YSI.CurseOfSilverCrown.EndOfTurn.Event
{
    public class EventParametrChange
    {
        public enEventParameterType Type { get; set; }
        public int Before { get; set; }
        public int After { get; set; }
    }
}

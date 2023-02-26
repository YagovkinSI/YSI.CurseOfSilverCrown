﻿using System.ComponentModel.DataAnnotations;

namespace YSI.CurseOfSilverCrown.Core.MainModels.GameCommands
{
    public enum enCommandStatus
    {
        [Display(Name = "Уничтожен")]
        Destroyed = -1,

        [Display(Name = "Готов к движению")]
        ReadyToMove = 100,

        [Display(Name = "Отступление")]
        Retreat = 140,

        [Display(Name = "Завершено")]
        Complited = 150
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YSI.CurseOfSilverCrown.Core.Database.Enums
{
    public enum enEventParametrChange
    {
        [Display(Name = "Воины (всего)")]
        Warrior = 1,

        [Display(Name = "Казна")]
        Coffers = 2,

        [Display(Name = "Инвестиции")]
        Investments = 3,        

        [Display(Name = "Воины учавствовавшие в боях")]
        WarriorInWar = 1001,

    }
}
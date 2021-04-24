﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YSI.CurseOfSilverCrown.Web.Enums
{
    public enum enCommandType
    {
        ForDelete = -1,

        [Display(Name = "Расходы двора")]
        Idleness = 0,

        [Display(Name = "Сбор войск")]
        Growth = 1,

        [Display(Name = "Нападение")]
        War = 2,

        [Display(Name = "Сбор налогов")]
        CollectTax = 3,

        [Display(Name = "Вложение средства в экономику провинции")]
        Investments = 4,

        [Display(Name = "Защита провинции")]
        WarSupportDefense = 5
    }
}

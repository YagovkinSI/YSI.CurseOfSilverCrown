﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSI.CurseOfSilverCrown.Core.Database.Enums;

namespace YSI.CurseOfSilverCrown.EndOfTurn.Actions.Organizations.Parameters
{
    internal class CoffersActionParameter : ActionParameter
    {
        public CoffersActionParameter(int count)
            : base(count)
        {
            Type = enActionParameter.Coffers;
        }
    }
}
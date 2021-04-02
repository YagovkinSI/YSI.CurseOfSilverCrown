﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YSI.CurseOfSilverCrown.Web.Enums
{
    public enum enEventResultType
    {
        //Commands 1000+
        //Idleness = 0,
        Idleness = 1,

        //Growth = 1,
        Growth = 1001,

        //War = 2,
        FastWarSuccess = 2001,
        FastWarFail = 2002,
        FastRebelionSuccess = 2003,
        FastRebelionFail = 2004,

        //Auto 100000+
        //VasalTax
        VasalTax = 100001,
    }
}
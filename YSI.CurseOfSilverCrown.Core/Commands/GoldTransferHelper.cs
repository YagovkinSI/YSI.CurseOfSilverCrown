﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSI.CurseOfSilverCrown.Core.BL.Models;
using YSI.CurseOfSilverCrown.Core.BL.Models.Min;
using YSI.CurseOfSilverCrown.Core.Database.EF;
using YSI.CurseOfSilverCrown.Core.Database.Models;

namespace YSI.CurseOfSilverCrown.Core.Commands
{
    public static class GoldTransferHelper
    {
        public const int MaxGoldTransfer = 1000;

        public static async Task<IEnumerable<DomainMin>> GetAvailableTargets(ApplicationDbContext context, int organizationId,
            Command command)
        {
            var organizations = context.GetAllDomainMain()
                .Where(o => o.Id != organizationId);

            return await organizations.ToListAsync();
        }
    }
}

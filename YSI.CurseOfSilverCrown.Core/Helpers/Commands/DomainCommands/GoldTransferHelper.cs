﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YSI.CurseOfSilverCrown.Core.Database;
using YSI.CurseOfSilverCrown.Core.Database.Commands;
using YSI.CurseOfSilverCrown.Core.Database.Domains;

namespace YSI.CurseOfSilverCrown.Core.Helpers.Commands.DomainCommands
{
    public static class GoldTransferHelper
    {
        public const int MaxGoldTransfer = 1500;

        public static async Task<IEnumerable<Domain>> GetAvailableTargets(ApplicationDbContext context, int organizationId,
            Command command)
        {
            var organizations = context.Domains.AsQueryable();

            //не передаём себе
            organizations = organizations
                .Where(o => o.Id != organizationId);

            return await organizations.ToListAsync();
        }
    }
}

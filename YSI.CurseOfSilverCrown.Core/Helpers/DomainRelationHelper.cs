﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YSI.CurseOfSilverCrown.Core.Database;
using YSI.CurseOfSilverCrown.Core.Database.Commands;
using YSI.CurseOfSilverCrown.Core.Database.Domains;

namespace YSI.CurseOfSilverCrown.Core.Helpers
{
    public static class DomainRelationHelper
    {
        public static async Task<IEnumerable<Domain>> GetAvailableTargets(ApplicationDbContext context, int organizationId)
        {
            var organization = await context.Domains.FindAsync(organizationId);

            var result = context.Domains.AsQueryable();

            //Убираем тех к кому уже есть приказы
            var hasRelations = organization.Relations.Select(r => r.TargetDomainId);
            result = result.Where(d => !hasRelations.Contains(d.Id));

            return result;
        }

        public static async Task<IEnumerable<Domain>> GetAvailableTargets2(ApplicationDbContext context, int organizationId, Command command = null)
        {
            return await context.Domains.ToListAsync();
        }
    }
}

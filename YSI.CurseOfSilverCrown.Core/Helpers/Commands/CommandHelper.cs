﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using YSI.CurseOfSilverCrown.Core.Database;
using YSI.CurseOfSilverCrown.Core.Database.Commands;
using YSI.CurseOfSilverCrown.Core.Database.Domains;

namespace YSI.CurseOfSilverCrown.Core.Helpers.Commands
{
    public static class CommandHelper
    {
        public static Domain GetDomain(this Command command, ApplicationDbContext context)
        {
            var domain = command.ExecutorType switch
            {
                ExecutorType.Domain => context.Domains.Find(command.ExecutorId),
                ExecutorType.Unit => context.Units.Find(command.ExecutorId).Domain,
                _ => throw new NotImplementedException(nameof(command.ExecutorType))
            };
            return domain;
        }

        public static void CheckAndFix(ApplicationDbContext context, int domainId)
        {
            if (!context.Commands.Any(c => c.DomainId == domainId))
            {
                var domain = context.Domains.Find(domainId);
                CommandCreateForNewTurnHelper.CreateNewCommandsForOrganizations(context, domain);
            }
        }
    }
}

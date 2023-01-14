﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YSI.CurseOfSilverCrown.Core.Database.EF;
using YSI.CurseOfSilverCrown.Core.Database.Enums;
using YSI.CurseOfSilverCrown.Core.Database.Models.GameWorld;
using YSI.CurseOfSilverCrown.Core.ViewModels;

namespace YSI.CurseOfSilverCrown.Core.Helpers
{
    public static class WarBaseHelper
    {
        public static async Task<IEnumerable<GameMapRoute>> GetAvailableTargets(
            ApplicationDbContext context,
            int organizationId,
            Unit command,
            enArmyCommandType commandType)
        {
            var domain = await context.Domains.FindAsync(organizationId);

            var availableRoutes = context.GetAvailableRoutes(command.PositionDomainId.Value, 2);
            var unavailableTargets = GetUnavailableTargets(context, domain, commandType);
            var availableTargets = FilterAndFillGameMapRoute(context, availableRoutes, unavailableTargets);
            return availableTargets;
        }

        private static List<int> GetUnavailableTargets(
            ApplicationDbContext context,
            Domain domain,
            enArmyCommandType commandType)
        {
            switch (commandType)
            {
                case enArmyCommandType.War:
                case enArmyCommandType.WarSupportAttack:
                    //не нападаем на своё королевство
                    return context.Domains
                        .GetAllDomainsIdInKingdoms(domain);
                case enArmyCommandType.WarSupportDefense:
                    return new List<int>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(commandType));
            }
        }

        private static IOrderedEnumerable<GameMapRoute> FilterAndFillGameMapRoute(
            ApplicationDbContext context,
            List<GameMapRoute> availableRoutes,
            List<int> unavailableTargets)
        {
            var targetIds = availableRoutes.Select(t => t.TargetDomain.Id);
            var targetOrganizations = context.Domains
                .Where(o => targetIds.Contains(o.Id))
                .Where(o => !unavailableTargets.Contains(o.Id))
                .ToList()
                .Select(d => new GameMapRoute(d, availableRoutes.Single(t => t.TargetDomain.Id == d.Id).Distance))
                .OrderBy(t => t.TargetDomain.Name)
                .OrderBy(t => t.Distance);
            return targetOrganizations;
        }

        public static double GetTargetPower(Domain target)
        {
            var defender = target.Suzerain ?? target;
            var allWarriors = defender.WarriorCount;
            var warriorsInDomain = target.UnitsHere
                .Where(u => u.DomainId == defender.Id)
                .Sum(u => u.Warriors);
            return warriorsInDomain * FortificationsHelper.GetDefencePercent(target.Fortifications) / 100.0 +
                allWarriors - warriorsInDomain;
        }
    }
}
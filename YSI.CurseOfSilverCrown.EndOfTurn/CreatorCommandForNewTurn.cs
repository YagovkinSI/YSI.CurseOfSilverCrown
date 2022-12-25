﻿using System;
using System.Collections.Generic;
using System.Linq;
using YSI.CurseOfSilverCrown.Core.Database.EF;
using YSI.CurseOfSilverCrown.Core.Database.Enums;
using YSI.CurseOfSilverCrown.Core.Database.Models;
using YSI.CurseOfSilverCrown.Core.Database.Models.GameWorld;
using YSI.CurseOfSilverCrown.Core.Parameters;

namespace YSI.CurseOfSilverCrown.EndOfTurn
{
    public static class CreatorCommandForNewTurn
    {
        private static readonly Random _random = new Random();

        public static void CreateNewCommandsForOrganizations(ApplicationDbContext context)
        {
            var domains = context.Domains.ToArray();
            foreach (var organization in domains)
            {
                CreateNewCommandsForBotOrganizations(context, organization, organization.PersonId);
            }
            context.SaveChanges();
        }

        public static void CreateNewCommandsForOrganizations(ApplicationDbContext context, int initiatorId, Domain domain)
        {
            CreateNewCommandsForBotOrganizations(context, domain, initiatorId);
            context.SaveChanges();
        }

        private static void CreateNewCommandsForBotOrganizations(ApplicationDbContext context, Domain domain, int initiatorId)
        {
            var growth = GetGrowthCommand(context, domain, initiatorId);
            var investments = GetInvestmentsCommand(domain, initiatorId);
            var fortifications = GetFortificationsCommand(domain, initiatorId);
            context.AddRange(growth, investments, fortifications);

            if (initiatorId != domain.PersonId)
            {
                var domainUnits = context.Units
                    .Where(d => d.DomainId == domain.Id && d.InitiatorPersonId == domain.PersonId);
                var newUnits = new List<Unit>();
                foreach (var unit in domainUnits)
                {
                    var newUnit = new Unit
                    {
                        DomainId = unit.DomainId,
                        PositionDomainId = unit.PositionDomainId,
                        Warriors = unit.Warriors,
                        Type = enArmyCommandType.WarSupportDefense,
                        TargetDomainId = unit.PositionDomainId,
                        InitiatorPersonId = initiatorId,
                        Status = enCommandStatus.ReadyToMove,
                        ActionPoints = WarConstants.ActionPointsFullCount
                    };
                    newUnits.Add(newUnit);
                }
                context.AddRange(newUnits);
            }
        }

        private static Command GetGrowthCommand(ApplicationDbContext context, Domain domain, int? initiatorId = null)
        {
            var warriors = domain.WarriorCount;
            var wantWarriors = Math.Max(0, WarriorParameters.StartCount * 1.1 - warriors);
            var wantWarriorsRandom = wantWarriors > 0
                ? (int)Math.Max(0, wantWarriors - _random.Next(20))
                : 0;
            var needMoney = wantWarriorsRandom * (WarriorParameters.Maintenance + WarriorParameters.Price);
            if (needMoney > domain.Coffers)
            {
                wantWarriorsRandom = domain.Coffers / (WarriorParameters.Maintenance + WarriorParameters.Price);
            }
            var spendToGrowth = wantWarriorsRandom * WarriorParameters.Price;

            return new Command
            {
                Coffers = spendToGrowth,
                DomainId = domain.Id,
                Type = enCommandType.Growth,
                InitiatorPersonId = initiatorId ?? domain.PersonId,
                Status = enCommandStatus.ReadyToMove
            };
        }

        private static Command GetInvestmentsCommand(Domain organization, int? initiatorId = null)
        {
            return new Command
            {
                Coffers = 0,
                DomainId = organization.Id,
                Type = enCommandType.Investments,
                InitiatorPersonId = initiatorId ?? organization.PersonId,
                Status = enCommandStatus.ReadyToMove
            };
        }

        private static Command GetFortificationsCommand(Domain domain, int? initiatorId = null)
        {
            return new Command
            {
                Coffers = 0,
                DomainId = domain.Id,
                Type = enCommandType.Fortifications,
                InitiatorPersonId = initiatorId ?? domain.PersonId,
                Status = enCommandStatus.ReadyToMove
            };
        }
    }
}

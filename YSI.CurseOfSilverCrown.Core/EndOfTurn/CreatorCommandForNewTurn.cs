﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YSI.CurseOfSilverCrown.Core.Database.EF;
using YSI.CurseOfSilverCrown.Core.Database.Models;
using YSI.CurseOfSilverCrown.Core.Database.Enums;
using YSI.CurseOfSilverCrown.Core.Utils;
using YSI.CurseOfSilverCrown.Core.Parameters;

namespace YSI.CurseOfSilverCrown.Core.EndOfTurn
{
    public static class CreatorCommandForNewTurn
    {
        private static Random _random = new Random();

        public static void CreateNewCommandsForOrganizations(ApplicationDbContext context, params Domain[] organizations)
        {
            foreach (var organization in organizations)
            {
                CreateNewCommandsForBotOrganizations(context, organization, organization.Id);
            }
            context.SaveChanges();
        }

        public static void CreateNewCommandsForOrganizations(ApplicationDbContext context, int initiatorId, Domain organization)
        {
            CreateNewCommandsForBotOrganizations(context, organization, initiatorId);
            context.SaveChanges();
        }

        private static void CreateNewCommandsForBotOrganizations(ApplicationDbContext context, Domain organization, int initiatorId)
        {
            var growth = GetGrowthCommand(organization, initiatorId);
            var investments = GetInvestmentsCommand(organization, initiatorId);
            var fortifications = GetFortificationsCommand(organization, initiatorId);
            var idleness = GetIdlenessCommand(organization, initiatorId);
            context.AddRange(growth, investments, fortifications, idleness);
            
            var domainUnits = organization.Units
                    .Where(u => u.DomainId == organization.Id);
            if (domainUnits.Sum(u => u.Warriors) < organization.Warriors)
            {
                var newUnit = new Unit
                {
                    DomainId = organization.Id,
                    PositionDomainId = organization.Id,
                    Warriors = organization.Warriors - domainUnits.Sum(u => u.Warriors),
                    Type = enArmyCommandType.WarSupportDefense,
                    TargetDomainId = organization.Id,
                    InitiatorDomainId = organization.Id,
                    Status = enCommandStatus.ReadyToRun
                };
                context.Add(newUnit);
            }

            if (initiatorId != organization.Id)
            {
                
                foreach (var unit in domainUnits)
                {
                    var newUnit = new Unit
                    {
                        DomainId = unit.DomainId,
                        PositionDomainId = unit.PositionDomainId,
                        Warriors = unit.Warriors,
                        Type = enArmyCommandType.WarSupportDefense,
                        TargetDomainId = unit.PositionDomainId,
                        InitiatorDomainId = initiatorId,
                        Status = enCommandStatus.ReadyToSend
                    };
                    context.Add(newUnit);
                }
            }
        }

        private static Unit GetDefenceCommand(Domain organization, int warrioirs, int? initiatorId = null)
        {
            return new Unit
            {
                DomainId = organization.Id,
                PositionDomainId = organization.Id,
                Warriors = warrioirs,
                Type = enArmyCommandType.WarSupportDefense,
                TargetDomainId = organization.Id,
                InitiatorDomainId = initiatorId ?? organization.Id,
                Status = initiatorId == null || initiatorId == organization.Id
                    ? enCommandStatus.ReadyToRun
                    : enCommandStatus.ReadyToSend
            };
        }

        private static Command GetGrowthCommand(Domain organization, int? initiatorId = null)
        {
            var wantWarriors = Math.Max(0, WarriorParameters.StartCount - organization.Warriors);
            var wantWarriorsRandom = wantWarriors > 0
                ? Math.Max(0, wantWarriors + _random.Next(20))
                : 0;
            var needMoney = wantWarriorsRandom * (WarriorParameters.Maintenance + WarriorParameters.Price);
            if (needMoney > organization.Coffers)
            {
                wantWarriorsRandom = organization.Coffers / (WarriorParameters.Maintenance + WarriorParameters.Price);
            }
            var spendToGrowth = wantWarriorsRandom * WarriorParameters.Price;

            return new Command
            {
                Coffers = spendToGrowth,
                DomainId = organization.Id,
                Type = enCommandType.Growth,
                InitiatorDomainId = initiatorId ?? organization.Id,
                Status = initiatorId == null || initiatorId == organization.Id
                    ? enCommandStatus.ReadyToRun
                    : enCommandStatus.ReadyToSend
            };
        }

        private static Command GetInvestmentsCommand(Domain organization, int? initiatorId = null)
        {
            return new Command
            {
                Coffers = 0,
                DomainId = organization.Id,
                Type = enCommandType.Investments,
                InitiatorDomainId = initiatorId ?? organization.Id,
                Status = initiatorId == null || initiatorId == organization.Id
                    ? enCommandStatus.ReadyToRun
                    : enCommandStatus.ReadyToSend
            };
        }

        private static Command GetFortificationsCommand(Domain organization, int? initiatorId = null)
        {
            return new Command
            {
                Coffers = 0,
                DomainId = organization.Id,
                Type = enCommandType.Fortifications,
                InitiatorDomainId = initiatorId ?? organization.Id,
                Status = initiatorId == null || initiatorId == organization.Id
                    ? enCommandStatus.ReadyToRun
                    : enCommandStatus.ReadyToSend
            };
        }

        private static Command GetIdlenessCommand(Domain organization, int? initiatorId = null)
        {
            return new Command
            {
                Coffers = RandomHelper.AddRandom(Constants.MinIdleness, roundRequest: -1),
                DomainId = organization.Id,
                Type = enCommandType.Idleness,
                InitiatorDomainId = initiatorId ?? organization.Id,
                Status = initiatorId == null || initiatorId == organization.Id
                    ? enCommandStatus.ReadyToRun
                    : enCommandStatus.ReadyToSend
            };
        }
    }
}

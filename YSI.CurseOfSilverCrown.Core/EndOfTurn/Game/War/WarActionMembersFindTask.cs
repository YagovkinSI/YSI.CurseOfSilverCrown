﻿using System;
using System.Collections.Generic;
using System.Linq;
using YSI.CurseOfSilverCrown.Core.Database.Models.GameWorld;
using YSI.CurseOfSilverCrown.Core.Game.Map.Routes;
using YSI.CurseOfSilverCrown.Core.Game.War;
using YSI.CurseOfSilverCrown.Core.Helpers;
using YSI.CurseOfSilverCrown.Core.MainModels;
using YSI.CurseOfSilverCrown.Core.MainModels.GameCommands;
using YSI.CurseOfSilverCrown.Core.MainModels.GameCommands.UnitCommands;

namespace YSI.CurseOfSilverCrown.EndOfTurn.Game.War
{
    internal class WarActionMembersFindTask
    {
        private readonly ApplicationDbContext _context;
        private readonly Unit _agressorUnit;

        public WarActionMembersFindTask(ApplicationDbContext context, Unit agressorUnit)
        {
            _context = context;
            _agressorUnit = agressorUnit;
        }

        public List<WarActionMember> Execute()
        {
            var agressorDomain = _agressorUnit.Domain;
            var targetDomain = _context.Domains.Find(_agressorUnit.TargetDomainId);

            var warMembers = new List<WarActionMember>();

            var agressorMember = GetAgressorMember(_context, _agressorUnit);
            warMembers.Add(agressorMember);

            var agressorSupportMembers = GetAgressorSupportMembers(_context, _agressorUnit, targetDomain);
            warMembers.AddRange(agressorSupportMembers);

            var allAgressorIds = warMembers.Select(p => p.Organization.Id);
            var allDefenders = GetAllDefenderDomains(_context, targetDomain, allAgressorIds, agressorDomain);

            var targetDefenseMembers = GetTargetDefenseMembers(targetDomain, allDefenders);
            warMembers.AddRange(targetDefenseMembers);

            var targetDefenseSupportMembers =
                GetTargetDefenseSupportMembers(_context, targetDomain, allDefenders);
            warMembers.AddRange(targetDefenseSupportMembers);

            return warMembers;
        }

        private WarActionMember GetAgressorMember(ApplicationDbContext context, Unit agressorUnit)
        {
            var allWarriors = DomainHelper.GetWarriorCount(context, agressorUnit.DomainId);
            return new WarActionMember(agressorUnit, allWarriors, enTypeOfWarrior.Agressor, 0, 40);
        }

        private IEnumerable<Domain> GetAllDefenderDomains(ApplicationDbContext context,
            Domain targetDomain, IEnumerable<int> agressorDomainIds, Domain mainAgressorDomain)
        {
            var allDefenderDomains = WarActionHelper.GetAllDefenderDomains(context, targetDomain)
                .Where(d => !agressorDomainIds.Contains(d.Id))
                .Where(d => !KingdomHelper.IsSameKingdoms(context.Domains, d, mainAgressorDomain));

            return allDefenderDomains;
        }

        private IEnumerable<WarActionMember> GetTargetDefenseSupportMembers(ApplicationDbContext context,
            Domain targetDomain,
            IEnumerable<Domain> allDefenders)
        {
            var unitsForSupport = allDefenders
                .SelectMany(d => d.Units)
                .Where(u => u.Type != enUnitCommandType.ForDelete && u.Type != enUnitCommandType.CollectTax)
                .Where(c => c.Status != enCommandStatus.Retreat && c.Status != enCommandStatus.Destroyed)
                .Where(u => u.PositionDomainId != targetDomain.Id);

            var warMembers = new List<WarActionMember>();
            foreach (var unit in unitsForSupport)
            {
                var routeFindParameters = new RouteFindParameters(unit, enMovementReason.Defense, targetDomain.Id);
                var route = RouteHelper.FindRoute(context, routeFindParameters);
                if (route != null)
                {
                    var Member = new WarActionMember(unit, DomainHelper.GetWarriorCount(context, unit.DomainId),
                        enTypeOfWarrior.TargetSupport, route.Count - 1, 60);
                    warMembers.Add(Member);
                }
            }

            return warMembers;
        }

        private IEnumerable<WarActionMember> GetTargetDefenseMembers(Domain targetDomain, IEnumerable<Domain> allDefenders)
        {
            var defenseUnits = targetDomain.UnitsHere
                .Where(u => allDefenders.Any(d => d.Id == u.DomainId))
                .Where(c => c.Type != enUnitCommandType.CollectTax && c.Type != enUnitCommandType.ForDelete)
                .Where(c => c.Status != enCommandStatus.Retreat && c.Status != enCommandStatus.Destroyed);

            var warMembers = new List<WarActionMember>();
            foreach (var unit in defenseUnits)
            {
                var morality = unit.DomainId == targetDomain.Id
                    ? targetDomain.SuzerainId == null
                        ? 60
                        : 10
                    : unit.DomainId == targetDomain.SuzerainId
                        ? 50
                        : 30;
                var warMember = new WarActionMember(unit, unit.Domain.WarriorCount,
                    enTypeOfWarrior.TargetDefense, 0, morality);
                warMembers.Add(warMember);
            }

            return warMembers;
        }

        //TODO: Big method
        private IEnumerable<WarActionMember> GetAgressorSupportMembers(ApplicationDbContext context, Unit agressorUnit, Domain targetDomain)
        {
            var agressorSupport = targetDomain.ToDomainUnits
                .Where(c => (c.Type == enUnitCommandType.WarSupportAttack && c.Target2DomainId == agressorUnit.DomainId) ||
                    (c.Id != agressorUnit.Id && c.Type == enUnitCommandType.War && c.DomainId == agressorUnit.DomainId));

            var warMembers = new List<WarActionMember>();
            foreach (var unit in agressorSupport)
            {
                var distanceToCastle = 0;
                if (unit.Type != enUnitCommandType.WarSupportAttack || unit.Status != enCommandStatus.Complited)
                {
                    var routeFindParameters = new RouteFindParameters(unit, enMovementReason.SupportAttack, targetDomain.Id);
                    var route = RouteHelper.FindRoute(context, routeFindParameters);
                    if (route == null)
                        continue;
                }

                var morality = unit.DomainId == agressorUnit.DomainId ? 40 : 10;
                var member = new WarActionMember(unit, DomainHelper.GetWarriorCount(context, unit.DomainId),
                        enTypeOfWarrior.AgressorSupport, distanceToCastle, morality);
                warMembers.Add(member);
            }

            return warMembers;
        }
    }
}

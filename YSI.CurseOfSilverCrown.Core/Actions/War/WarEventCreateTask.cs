﻿using System.Collections.Generic;
using System.Linq;
using YSI.CurseOfSilverCrown.Core.Game.War;
using YSI.CurseOfSilverCrown.Core.Helpers;
using YSI.CurseOfSilverCrown.Core.MainModels;
using YSI.CurseOfSilverCrown.Core.MainModels.EventDomains;
using YSI.CurseOfSilverCrown.Core.MainModels.Events;
using YSI.CurseOfSilverCrown.Core.Parameters;

namespace YSI.CurseOfSilverCrown.Core.Actions.War
{
    internal class WarEventCreateTask
    {
        private ApplicationDbContext _context { get; }
        private WarActionParameters _warActionParameters { get; }
        public EventJson EventStoryResult { get; private set; }
        public Dictionary<int, int> DommainEventStories { get; private set; }

        public WarEventCreateTask(ApplicationDbContext context, WarActionParameters warActionParameters)
        {
            _context = context;
            _warActionParameters = warActionParameters;
        }

        public void Execute()
        {
            var organizationsMembers = _warActionParameters.WarActionMembers
                .Where(m => m.IsReadyToBattle(_warActionParameters.DayOfWar) || m.WarriorLosses > 0 || m.Morality <= 0)
                .GroupBy(p => p.Organization.Id);

            var type = _warActionParameters.IsVictory
                ? enEventType.FastWarSuccess
                : !_warActionParameters.IsBreached
                    ? enEventType.SiegeFail
                    : enEventType.FastWarFail;
            EventStoryResult = new EventJson(type);
            FillEventOrganizationList(organizationsMembers);

            var importanceByLosses = _warActionParameters.WarActionMembers.Sum(p => p.WarriorLosses) * WarriorParameters.Price * 2;
            var importanceByVitory = _warActionParameters.IsVictory
                ? DomainHelper.GetImprotanceDoamin(_context, _warActionParameters.TargetDomainId)
                : 0;

            DommainEventStories = organizationsMembers.ToDictionary(
                o => o.Key,
                o => importanceByLosses + importanceByVitory);
        }


        private void FillEventOrganizationList(IEnumerable<IGrouping<int, WarActionMember>> organizationsMembers)
        {
            foreach (var organizationsMember in organizationsMembers)
            {
                var eventOrganizationType = GetEventOrganizationType(organizationsMember);
                var allWarriorsDomainOnStart = organizationsMember.First().AllWarriorsBeforeWar;
                var allWarriorsInBattleOnStart = organizationsMember.Sum(p => p.WarriorsOnStart);
                var allWarriorsLost = organizationsMember.Sum(p => p.WarriorLosses);
                var temp = new List<EventJsonParametrChange>
                {
                    EventJsonParametrChangeHelper.Create(
                        enEventParameterType.WarriorInWar, allWarriorsInBattleOnStart, allWarriorsInBattleOnStart - allWarriorsLost
                    ),
                    EventJsonParametrChangeHelper.Create(
                        enEventParameterType.Warrior, allWarriorsDomainOnStart, allWarriorsDomainOnStart - allWarriorsLost
                    )
                };
                EventStoryResult.AddEventOrganization(organizationsMember.First().Organization.Id, eventOrganizationType, temp);
            }

            if (!organizationsMembers.Any(o => GetEventOrganizationType(o) == enEventDomainType.Defender))
            {
                var target = _context.Domains.Find(_warActionParameters.TargetDomainId);
                var temp = new List<EventJsonParametrChange>();
                EventStoryResult.AddEventOrganization(target.Id, enEventDomainType.Defender, temp);
            }
        }

        private enEventDomainType GetEventOrganizationType(IGrouping<int, WarActionMember> organizationsMember)
        {
            switch (organizationsMember.First().Type)
            {
                case enTypeOfWarrior.Agressor:
                    return enEventDomainType.Agressor;
                case enTypeOfWarrior.AgressorSupport:
                    return enEventDomainType.SupporetForAgressor;
                default:
                    return organizationsMember.First().Organization.Id == _warActionParameters.TargetDomainId
                        ? enEventDomainType.Defender
                        : enEventDomainType.SupporetForDefender;
            }
        }
    }
}
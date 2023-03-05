﻿using System;
using System.Collections.Generic;
using YSI.CurseOfSilverCrown.Core.Database;
using YSI.CurseOfSilverCrown.Core.Database.Commands;
using YSI.CurseOfSilverCrown.Core.Database.Events;
using YSI.CurseOfSilverCrown.Core.Database.Turns;
using YSI.CurseOfSilverCrown.Core.Helpers.Events;

namespace YSI.CurseOfSilverCrown.Core.Helpers.Actions
{
    internal class FortificationsAction : CommandActionBase
    {
        protected int ImportanceBase => 500;

        protected override bool RemoveCommandeAfterUse => true;

        public FortificationsAction(ApplicationDbContext context, Turn currentTurn, Command command)
            : base(context, currentTurn, command)
        {
        }

        public override bool CheckValidAction()
        {
            FixCoffersForAction();

            return Command.Type == CommandType.Fortifications &&
                Command.Coffers > 0 &&
                Command.Status == CommandStatus.ReadyToMove;
        }

        protected override bool Execute()
        {
            var coffers = Command.Domain.Coffers;
            var fortifications = Command.Domain.Fortifications;

            var spentCoffers = Math.Min(coffers, Command.Coffers);
            var getFortifications = spentCoffers;

            var newCoffers = coffers - spentCoffers;
            var newFortifications = fortifications + getFortifications;

            Command.Domain.Coffers = newCoffers;
            Command.Domain.Fortifications = newFortifications;

            var eventStoryResult = new EventJson(EventType.Fortifications);
            var eventOrganizationChanges = new List<EventParticipantParameterChange>
            {
                EventJsonParametrChangeHelper.Create(EventParticipantParameterType.Fortifications, fortifications, newFortifications),
                EventJsonParametrChangeHelper.Create(EventParticipantParameterType.Coffers, coffers, newCoffers)
            };
            eventStoryResult.AddEventOrganization(Command.DomainId, EventParticipantType.Main, eventOrganizationChanges);

            var thresholdImportance = EventHelper.GetThresholdImportance(fortifications, newFortifications);
            eventStoryResult.EventResultType = GetFortificationsEventResultType(thresholdImportance);
            var dommainEventStories = new Dictionary<int, int>
            {
                { Command.Domain.Id, spentCoffers + thresholdImportance }
            };
            CreateEventStory(eventStoryResult, dommainEventStories);

            return true;
        }

        private EventType GetFortificationsEventResultType(int thresholdImportance)
        {
            if (thresholdImportance < 3000)
                return EventType.Fortifications;
            else if (thresholdImportance < 6000)
                return EventType.FortificationsLevelI;
            else if (thresholdImportance < 12000)
                return EventType.FortificationsLevelII;
            else if (thresholdImportance < 35000)
                return EventType.FortificationsLevelIII;
            else if (thresholdImportance < 100000)
                return EventType.FortificationsLevelIV;
            else
                return EventType.FortificationsLevelV;
        }
    }
}

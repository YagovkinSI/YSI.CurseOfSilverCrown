﻿using System;
using System.Collections.Generic;
using YSI.CurseOfSilverCrown.Core.MainModels;
using YSI.CurseOfSilverCrown.Core.MainModels.Commands;
using YSI.CurseOfSilverCrown.Core.MainModels.Commands.DomainCommands;
using YSI.CurseOfSilverCrown.Core.MainModels.EventDomains;
using YSI.CurseOfSilverCrown.Core.MainModels.Events;
using YSI.CurseOfSilverCrown.Core.MainModels.Turns;

namespace YSI.CurseOfSilverCrown.Core.Actions
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

            return Command.Type == enDomainCommandType.Fortifications &&
                Command.Coffers > 0 &&
                Command.Status == enCommandStatus.ReadyToMove;
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

            var eventStoryResult = new EventJson(enEventType.Fortifications);
            var eventOrganizationChanges = new List<EventJsonParametrChange>
            {
                EventJsonParametrChangeHelper.Create(enEventParameterType.Fortifications, fortifications, newFortifications),
                EventJsonParametrChangeHelper.Create(enEventParameterType.Coffers, coffers, newCoffers)
            };
            eventStoryResult.AddEventOrganization(Command.DomainId, enEventDomainType.Main, eventOrganizationChanges);

            var thresholdImportance = EventHelper.GetThresholdImportance(fortifications, newFortifications);
            eventStoryResult.EventResultType = GetFortificationsEventResultType(thresholdImportance);
            var dommainEventStories = new Dictionary<int, int>
            {
                { Command.Domain.Id, spentCoffers + thresholdImportance }
            };
            CreateEventStory(eventStoryResult, dommainEventStories);

            return true;
        }

        private enEventType GetFortificationsEventResultType(int thresholdImportance)
        {
            if (thresholdImportance < 3000)
                return enEventType.Fortifications;
            else if (thresholdImportance < 10000)
                return enEventType.FortificationsLevelI;
            else if (thresholdImportance < 30000)
                return enEventType.FortificationsLevelII;
            else if (thresholdImportance < 100000)
                return enEventType.FortificationsLevelIII;
            else if (thresholdImportance < 300000)
                return enEventType.FortificationsLevelIV;
            else
                return enEventType.FortificationsLevelV;
        }
    }
}
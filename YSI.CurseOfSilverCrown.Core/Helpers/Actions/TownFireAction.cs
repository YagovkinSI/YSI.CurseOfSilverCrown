﻿using System.Collections.Generic;
using YSI.CurseOfSilverCrown.Core.Database;
using YSI.CurseOfSilverCrown.Core.Database.Domains;
using YSI.CurseOfSilverCrown.Core.Database.EventDomains;
using YSI.CurseOfSilverCrown.Core.Database.Events;
using YSI.CurseOfSilverCrown.Core.Database.Turns;

namespace YSI.CurseOfSilverCrown.Core.Helpers.Actions
{
    internal class TownFireAction : DomainActionBase
    {
        public TownFireAction(ApplicationDbContext context, Turn currentTurn, Domain domain)
            : base(context, currentTurn, domain)
        {
        }

        public override bool CheckValidAction()
        {
            return Domain.Investments > InvestmentsHelper.StartInvestment * 1.2;
        }

        protected override bool Execute()
        {
            var startParametr = Domain.Investments;
            var endParametr = (int)(Domain.Investments * RandomHelper.AddRandom(0.8));
            if (endParametr < InvestmentsHelper.StartInvestment * 0.9)
                endParametr = RandomHelper.AddRandom(InvestmentsHelper.StartInvestment);
            var deltaParamets = endParametr - startParametr;
            if (deltaParamets > 1)
                return false;
            Domain.Investments = endParametr;

            var eventStoryResult = CreateEventStoryResult(startParametr, endParametr);
            var dommainEventStories = new Dictionary<int, int>
            {
                { Domain.Id, - deltaParamets * 2 }
            };
            CreateEventStory(eventStoryResult, dommainEventStories);

            return true;
        }

        private EventJson CreateEventStoryResult(int startParametr, int endParametr)
        {
            var eventStoryResult = new EventJson(enEventType.TownFire);
            var temp = new List<EventJsonParametrChange>
            {
                EventJsonParametrChangeHelper.Create(enEventParameterType.Investments, startParametr, endParametr)
            };
            eventStoryResult.AddEventOrganization(Domain.Id, enEventDomainType.Main, temp);
            return eventStoryResult;
        }
    }
}
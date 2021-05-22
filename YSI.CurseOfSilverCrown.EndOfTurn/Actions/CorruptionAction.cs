﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSI.CurseOfSilverCrown.Core.Database.Models;
using YSI.CurseOfSilverCrown.Core.Database.Enums;
using YSI.CurseOfSilverCrown.Core.Utils;
using YSI.CurseOfSilverCrown.Core.Parameters;
using YSI.CurseOfSilverCrown.EndOfTurn.Event;
using YSI.CurseOfSilverCrown.Core.Database.EF;
using YSI.CurseOfSilverCrown.Core.Helpers;

namespace YSI.CurseOfSilverCrown.EndOfTurn.Actions
{
    internal class CorruptionAction : DomainActionBase
    {
        private int CorruptionLevel { get; set; }

        public CorruptionAction(ApplicationDbContext context, Turn currentTurn, Domain domain)
            : base(context, currentTurn, domain)
        {
        }

        protected override bool CheckValidAction()
        {
            CorruptionLevel = Constants.GetCorruptionLevel(Domain.User);
            return CorruptionLevel > 0;
        }

        protected override bool Execute()
        {

            var list = new List<EventParametrChange>();
            var importance = 0;

            var coffers = Domain.Coffers;
            if (coffers > CoffersParameters.StartCount * 1.1)
            {
                var maxCoffersDecrease = coffers - RandomHelper.AddRandom(CoffersParameters.StartCount, roundRequest: -1);
                var coffersDecrease = CorruptionLevel == 100
                    ? maxCoffersDecrease
                    : (int)Math.Round(maxCoffersDecrease * (CorruptionLevel / 100.0));
                var newCoffers = coffers - coffersDecrease;
                Domain.Coffers = newCoffers;
                var eventParametrChange = new EventParametrChange
                {
                    Type = enActionParameter.Coffers,
                    Before = coffers,
                    After = newCoffers
                };
                importance += newCoffers / 5;
                list.Add(eventParametrChange);
            }

            var warriors = DomainHelper.GetWarriorCount(Context, Domain.Id);
            if (warriors > WarriorParameters.StartCount * 1.1)
            {
                var maxWarriorsDecrease = warriors - RandomHelper.AddRandom(WarriorParameters.StartCount);
                var warriorsDecrease = CorruptionLevel == 100
                    ? maxWarriorsDecrease
                    : (int)Math.Round(maxWarriorsDecrease * (CorruptionLevel / 100.0));
                var newWarriors = warriors - warriorsDecrease;
                DomainHelper.SetWarriorCount(Context, Domain.Id, newWarriors);
                var eventParametrChange = new EventParametrChange
                {
                    Type = enActionParameter.Warrior,
                    Before = warriors,
                    After = newWarriors
                };
                importance += warriorsDecrease * 5;
                list.Add(eventParametrChange);
            }

            var investments = Domain.Investments;
            if (investments > 0)
            {
                var investmentsDecrease = CorruptionLevel == 100
                    ? investments
                    : (int)Math.Round(investments * (CorruptionLevel / 100.0));
                var newInvestments = investments - investmentsDecrease;
                Domain.Investments = newInvestments;
                var eventParametrChange = new EventParametrChange
                {
                    Type = enActionParameter.Investments,
                    Before = investments,
                    After = newInvestments
                };
                importance += investmentsDecrease / 9;
                list.Add(eventParametrChange);
            }



            var fortifications = Domain.Fortifications;
            if (fortifications > FortificationsParameters.StartCount)
            {
                var fortificationsDecrease = CorruptionLevel == 100
                    ? fortifications - FortificationsParameters.StartCount
                    : (int)Math.Round((fortifications - FortificationsParameters.StartCount) * (CorruptionLevel / 100.0));
                var newFortifications = fortifications - fortificationsDecrease;
                Domain.Fortifications = newFortifications;
                var eventParametrChange = new EventParametrChange
                {
                    Type = enActionParameter.Fortifications,
                    Before = fortifications,
                    After = newFortifications
                };
                importance += fortificationsDecrease / 9;
                list.Add(eventParametrChange);
            }

            if (list.Count == 0)
                return false;

            var eventStoryResult = new EventStoryResult(enEventResultType.Corruption);
            eventStoryResult.AddEventOrganization(Domain.Id, enEventOrganizationType.Main, list);

            var dommainEventStories = new Dictionary<int, int> {
                { Domain.Id, importance }
            };
            CreateEventStory(eventStoryResult, dommainEventStories);

            return true;
        }

    }
}

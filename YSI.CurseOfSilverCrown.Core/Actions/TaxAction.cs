﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YSI.CurseOfSilverCrown.Core.Helpers;
using YSI.CurseOfSilverCrown.Core.Database.EF;
using YSI.CurseOfSilverCrown.Core.Database.Models;
using YSI.CurseOfSilverCrown.Core.Database.Enums;
using YSI.CurseOfSilverCrown.Core.Parameters;
using YSI.CurseOfSilverCrown.Core.Actions;
using YSI.CurseOfSilverCrown.Core.Event;
using YSI.CurseOfSilverCrown.Core.Parameters;

namespace YSI.CurseOfSilverCrown.Core.Actions
{
    internal class TaxAction : ActionBase
    {
        private readonly ApplicationDbContext context;

        protected int ImportanceBase => 500;

        public List<OrganizationEventStory> OrganizationEventStories { get; internal set; }
        public EventStory EventStory { get; internal set; }

        public TaxAction(ApplicationDbContext context, Turn currentTurn, Command command)
            : base(context, currentTurn, command)
        {
            this.context = context;
        }

        public static int GetTax(int warriors, int investments, double random)
        {
            var additionalWarriors = warriors;
            var baseTax = Constants.MinTax;
            var randomBaseTax = baseTax * (0.99 + random / 100.0);

            var investmentTax = Constants.GetInvestmentTax(investments);
            var randomInvestmentTax = investmentTax * (0.99 + random / 100.0);

            var additionalTax = Constants.GetAdditionalTax(additionalWarriors, random);

            return (int)Math.Round(randomBaseTax + randomInvestmentTax + additionalTax);
        }

        public override bool Execute()
        {
            var getCoffers = GetTax(Command.Warriors, Command.Organization.Investments, Random.NextDouble());
            var eventOrganizationList = GetEventOrganizationList(context, Command.Organization, getCoffers);

            var eventStoryResult = new EventStoryResult
            {
                EventResultType = enEventResultType.TaxCollection,
                Organizations = eventOrganizationList 
            };

            EventStory = new EventStory
            {
                TurnId = CurrentTurn.Id,
                EventStoryJson = JsonConvert.SerializeObject(eventStoryResult)
            };

            OrganizationEventStories = eventOrganizationList
                .Select(e =>
                    new OrganizationEventStory
                    {
                        OrganizationId = e.Id,
                        Importance = getCoffers / 20,
                        EventStory = EventStory
                    })
                .ToList();

            return true;
        }

        private List<EventOrganization> GetEventOrganizationList(ApplicationDbContext context, Organization organization, 
            int allIncome, List<EventOrganization> currentList = null)
        {
            var type = enEventOrganizationType.Suzerain;
            if (currentList == null)
            {
                currentList = new List<EventOrganization>();
                type = enEventOrganizationType.Main;
            }

            var suzerainId = organization.SuzerainId;
            var getCoffers = suzerainId == null
                ? allIncome
                : (int)Math.Round(allIncome * (1 - Constants.BaseVassalTax));

            var eventOrganization = GetEventOrganization(organization, type, getCoffers);
            currentList.Add(eventOrganization);
            organization.Coffers += getCoffers;

            return suzerainId == null
                ? currentList
                : GetEventOrganizationList(context,
                    context.Organizations.Single(o => o.Id == suzerainId),
                    allIncome - getCoffers,
                    currentList);
        }

        private EventOrganization GetEventOrganization(Organization organization, enEventOrganizationType type, int getCoffers)
        {
            return new EventOrganization
            {
                Id = organization.Id,
                EventOrganizationType = type,
                EventOrganizationChanges = new List<EventParametrChange>
                        {
                            new EventParametrChange
                            {
                                Type = enEventParametrChange.Coffers,
                                Before = organization.Coffers,
                                After = organization.Coffers + getCoffers
                            }
                        }

            };
        }
    }
}
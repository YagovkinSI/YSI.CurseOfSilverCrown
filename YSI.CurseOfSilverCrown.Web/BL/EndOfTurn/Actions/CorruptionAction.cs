﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSI.CurseOfSilverCrown.Web.BL.EndOfTurn.Event;
using YSI.CurseOfSilverCrown.Web.Models.DbModels;

namespace YSI.CurseOfSilverCrown.Web.BL.EndOfTurn.Actions
{
    public class CorruptionAction
    {
        private Random _random = new Random();
        private Organization organization;
        private Turn currentTurn;

        private const int ImportanceBase = 5000;

        public EventStory EventStory { get; set; }
        public List<OrganizationEventStory> OrganizationEventStories { get; set; }

        public CorruptionAction(Organization organization, Turn currentTurn)
        {
            this.organization = organization;
            this.currentTurn = currentTurn;
        }

        internal bool Execute()
        {
            var corruptionLevel = Constants.GetCorruptionLevel(organization.User);

            var investments = organization.Investments;
            var investmentsDecrease = corruptionLevel == 100
                ? investments
                : (int)Math.Round(investments * (corruptionLevel / 100.0));
            var newInvestments = investments - investmentsDecrease;
            organization.Investments = newInvestments;

            var coffers = organization.Coffers;
            var maxCoffersDecrease = coffers - Constants.AddRandom10(Constants.StartCoffers, (new Random()).NextDouble());
            var coffersDecrease = corruptionLevel == 100
                ? maxCoffersDecrease
                : (int)Math.Round(maxCoffersDecrease * (corruptionLevel / 100.0));
            var newCoffers = coffers - coffersDecrease;
            organization.Coffers = newCoffers;

            var eventStoryResult = new EventStoryResult
            {
                EventResultType = Enums.enEventResultType.Corruption,
                Organizations = new List<EventOrganization>
                {
                    new EventOrganization
                    {
                        Id = organization.Id,
                        EventOrganizationType = Enums.enEventOrganizationType.Main,
                        EventOrganizationChanges = new List<EventParametrChange>
                        {

                            new EventParametrChange
                            {
                                Type = Enums.enEventParametrChange.Coffers,
                                Before = coffers,
                                After = newCoffers
                            },
                            new EventParametrChange
                            {
                                Type = Enums.enEventParametrChange.Investments,
                                Before = investments,
                                After = newInvestments
                            }
                        }

                    }
                }
            };

            EventStory = new EventStory
            {
                TurnId = currentTurn.Id,
                EventStoryJson = JsonConvert.SerializeObject(eventStoryResult)
            };

            OrganizationEventStories = new List<OrganizationEventStory>
            {
                new OrganizationEventStory
                {
                    Organization = organization,
                    Importance = newCoffers / 2 + investmentsDecrease / 4,
                    EventStory = EventStory
                }
            };

            return true;
        }

    }
}

﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSI.CurseOfSilverCrown.Core.Helpers;
using YSI.CurseOfSilverCrown.Core.Database.Models;
using YSI.CurseOfSilverCrown.Core.Database.Enums;
using YSI.CurseOfSilverCrown.Core.Parameters;
using YSI.CurseOfSilverCrown.Core.Event;
using YSI.CurseOfSilverCrown.Core.Database.EF;

namespace YSI.CurseOfSilverCrown.Core.Actions
{
    internal class MaintenanceAction : ActionBase
    {
        public MaintenanceAction(ApplicationDbContext context, Turn currentTurn, Organization organization)
            : base(context, currentTurn, organization)
        {
        }

        protected override bool Execute()
        {
            var coffers = Organization.Coffers;
            var warrioirs = Organization.Warriors;

            var spendCoffers = 0;
            spendCoffers += Organization.Warriors * WarriorParameters.Maintenance;
            var spendWarriors = 0;

            if (spendCoffers > coffers)
            {
                spendWarriors = (int)Math.Ceiling((spendCoffers - coffers) / (double)WarriorParameters.Maintenance);
                if (spendWarriors > warrioirs)
                    spendWarriors = warrioirs;
                spendCoffers -= spendWarriors * WarriorParameters.Maintenance;
            }

            var newCoffers = coffers - spendCoffers;
            var newWarriors = warrioirs - spendWarriors;
            Organization.Coffers = newCoffers;
            Organization.Warriors = newWarriors;

            var eventStoryResult = new EventStoryResult(enEventResultType.Maintenance);
            var temp = new List<EventParametrChange>
                        {
                            new EventParametrChange
                            {
                                Type = enActionParameter.Coffers,
                                Before = coffers,
                                After = newCoffers
                            }
                        };
            eventStoryResult.AddEventOrganization(Organization, enEventOrganizationType.Main, temp);

            if (spendWarriors > 0)
                eventStoryResult.Organizations.First().EventOrganizationChanges.Add(
                    new EventParametrChange
                    {
                        Type = enActionParameter.Warrior,
                        Before = warrioirs,
                        After = newWarriors
                    }
                    );

            EventStory = new EventStory
            {
                TurnId = CurrentTurn.Id,
                EventStoryJson = eventStoryResult.ToJson()
            };

            OrganizationEventStories = new List<OrganizationEventStory>
            {
                new OrganizationEventStory
                {
                    Organization = Organization,
                    Importance = spendWarriors * 5,
                    EventStory = EventStory
                }
            };

            return true;
        }

    }
}

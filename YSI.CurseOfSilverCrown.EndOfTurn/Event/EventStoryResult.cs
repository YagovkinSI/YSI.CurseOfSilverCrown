﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YSI.CurseOfSilverCrown.EndOfTurn.Actions.Organizations;
using YSI.CurseOfSilverCrown.Core.Database.Enums;
using YSI.CurseOfSilverCrown.Core.Database.Models;

namespace YSI.CurseOfSilverCrown.EndOfTurn.Event
{
    internal class EventStoryResult
    {
        public List<ActionOrganization> Organizations { get; set; }
        public enEventResultType EventResultType { get; set; }

        public EventStoryResult(enEventResultType eventResultType)
        {
            EventResultType = eventResultType;
            Organizations = new List<ActionOrganization>();
        }

        public void AddEventOrganization(int domainId, enEventOrganizationType organizationType, 
            List<EventParametrChange> eventParametrChanges)
        {
            var warriorInAction = eventParametrChanges.FirstOrDefault(p => p.Type == enActionParameter.WarriorInWar)?.Before ?? 0;
            var allWarriors = eventParametrChanges.FirstOrDefault(p => p.Type == enActionParameter.Warrior)?.Before ?? 0;

            var eventOrganization = new ActionOrganization(domainId, allWarriors, organizationType, warriorInAction);
            eventOrganization.EventOrganizationChanges = eventParametrChanges;
            Organizations.Add(eventOrganization);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YSI.CurseOfSilverCrown.Web.Models.DbModels
{
    public class OrganizationEventStory
    {
        public int TurnId { get; set; }
        public string OrganizationId { get; set; }
        public int EventStoryId { get; set; }

        public int Importance { get; set; }

        public Turn Turn { get; set; }
        public EventStory EventStory { get; set; }
        public Organization Organization { get; set; }
    }
}

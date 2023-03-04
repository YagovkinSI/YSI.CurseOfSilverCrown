﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using YSI.CurseOfSilverCrown.Core.MainModels.EventDomains;
using YSI.CurseOfSilverCrown.Core.MainModels.Turns;

namespace YSI.CurseOfSilverCrown.Core.MainModels.Events
{
    public class Event
    {
        public int TurnId { get; set; }
        public int Id { get; set; }

        public string EventStoryJson { get; set; }

        public virtual Turn Turn { get; set; }
        public virtual List<EventDomain> DomainEventStories { get; set; }

        internal static void CreateModel(ModelBuilder builder)
        {
            var model = builder.Entity<Event>();
            model.HasKey(m => new { m.TurnId, m.Id });

            model.HasOne(m => m.Turn)
                .WithMany(m => m.EventStories)
                .HasForeignKey(m => m.TurnId);
        }
    }
}
﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using YSI.CurseOfSilverCrown.Core.Database.Domains;
using YSI.CurseOfSilverCrown.Core.Database.Units;
using YSI.CurseOfSilverCrown.Core.Database.Users;

namespace YSI.CurseOfSilverCrown.Core.Database.Characters
{
    public class Character
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual User User { get; set; }

        public virtual List<Domain> Domains { get; set; }

        public virtual List<Unit> UnitsWithMyCommands { get; set; }

        internal static void CreateModel(ModelBuilder builder)
        {
            var model = builder.Entity<Character>();
            model.HasKey(m => m.Id);

            model.HasData(StartingData.Persons);
        }
    }
}
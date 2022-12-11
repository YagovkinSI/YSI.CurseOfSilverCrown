﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using YSI.CurseOfSilverCrown.Core.Database.PregenDatas;

namespace YSI.CurseOfSilverCrown.Core.Database.Models.GameWorld
{
    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public User User { get; set; }

        public List<Domain> Domains { get; set; }

        public List<Unit> UnitsWithMyCommands { get; set; }

        internal static void CreateModel(ModelBuilder builder)
        {
            var model = builder.Entity<Person>();
            model.HasKey(m => m.Id);

            model.HasData(PregenData.Persons);
        }
    }
}
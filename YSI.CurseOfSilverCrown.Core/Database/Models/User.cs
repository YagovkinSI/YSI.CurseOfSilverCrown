﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using YSI.CurseOfSilverCrown.Core.Database.Models.GameWorld;

namespace YSI.CurseOfSilverCrown.Core.Database.Models
{
    public class User : IdentityUser
    {
        public int? PersonId { get; set; }
        public DateTime LastActivityTime { get; set; }

        public virtual Person Person { get; set; }

        internal static void CreateModel(ModelBuilder builder)
        {
            var model = builder.Entity<User>();
            model.HasKey(m => m.Id);

            model.HasOne(m => m.Person)
                .WithOne(m => m.User)
                .HasForeignKey<User>(m => m.PersonId);
            model.HasIndex(m => m.PersonId);
        }
    }
}

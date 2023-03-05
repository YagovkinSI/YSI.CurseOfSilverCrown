﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YSI.CurseOfSilverCrown.Core.Helpers;
using YSI.CurseOfSilverCrown.Core.Database.Characters;
using YSI.CurseOfSilverCrown.Core.Database.Commands;
using YSI.CurseOfSilverCrown.Core.Database.Domains;
using YSI.CurseOfSilverCrown.Core.Helpers.StartingDatas;
using YSI.CurseOfSilverCrown.Core.Helpers.Commands.UnitCommands;

namespace YSI.CurseOfSilverCrown.Core.Database.Units
{
    public class Unit : ICommand
    {
        public int Id { get; set; }

        public int DomainId { get; set; }

        [Display(Name = "Казна")]
        public int Coffers { get; set; }

        [Display(Name = "Воины")]
        public int Warriors { get; set; }


        [Display(Name = "Действие")]
        public enUnitCommandType Type { get; set; }

        public int ActionPoints { get; set; }

        [Display(Name = "Цель")]
        public int? TargetDomainId { get; set; }

        [Display(Name = "Дополнительная цель")]
        public int? Target2DomainId { get; set; }

        [Display(Name = "Инициатор приказа")]
        public int InitiatorPersonId { get; set; }

        [Display(Name = "Местоположение")]
        public int? PositionDomainId { get; set; }

        [Display(Name = "Статус")]
        public CommandStatus Status { get; set; }

        public virtual Domain Domain { get; set; }

        public virtual Domain Target { get; set; }

        public virtual Domain Target2 { get; set; }

        public virtual Domain Position { get; set; }

        public virtual Character PersonInitiator { get; set; }

        [NotMapped]
        public int TypeInt { get => (int)Type; set => Type = (enUnitCommandType)value; }

        public override string ToString()
        {
            return $"Отряд владения {Domain?.Name ?? "???"} во владении {Position?.Name ?? "???"}, воинов - {Warriors}";
        }

        internal static void CreateModel(ModelBuilder builder)
        {
            var model = builder.Entity<Unit>();
            model.HasKey(m => m.Id);

            CreateModelRelations(model);

            model.HasIndex(m => m.InitiatorPersonId);
            model.HasIndex(m => m.DomainId);
            model.HasIndex(m => m.PositionDomainId);
            model.HasIndex(m => m.Type);
            model.HasIndex(m => m.TargetDomainId);
            model.HasIndex(m => m.ActionPoints);

            model.HasData(StartingData.Units);
        }

        private static void CreateModelRelations(EntityTypeBuilder<Unit> model)
        {
            model.HasOne(m => m.Domain)
                .WithMany(m => m.Units)
                .HasForeignKey(m => m.DomainId);
            model.HasOne(m => m.PersonInitiator)
                .WithMany(m => m.UnitsWithMyCommands)
                .HasForeignKey(m => m.InitiatorPersonId)
                .OnDelete(DeleteBehavior.Restrict);
            model.HasOne(m => m.Target)
                .WithMany(m => m.ToDomainUnits)
                .HasForeignKey(m => m.TargetDomainId);
            model.HasOne(m => m.Target2)
                .WithMany(m => m.ToDomain2Units)
                .HasForeignKey(m => m.Target2DomainId);
            model.HasOne(m => m.Position)
                .WithMany(m => m.UnitsHere)
                .HasForeignKey(m => m.PositionDomainId);
        }
    }
}

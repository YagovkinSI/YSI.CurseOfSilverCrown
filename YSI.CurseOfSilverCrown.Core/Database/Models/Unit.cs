﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YSI.CurseOfSilverCrown.Core.Database.Enums;
using YSI.CurseOfSilverCrown.Core.Interfaces;

namespace YSI.CurseOfSilverCrown.Core.Database.Models
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
        public enArmyCommandType Type { get; set; }

        [Display(Name = "Цель")]
        public int? TargetDomainId { get; set; }

        [Display(Name = "Дополнительная цель")]
        public int? Target2DomainId { get; set; }

        [Display(Name = "Инициатор приказа")]
        public int InitiatorDomainId { get; set; }

        [Display(Name = "Местоположение")]
        public int? PositionDomainId { get; set; }

        [Display(Name = "Статус")]
        public enCommandStatus Status { get; set; }

        public Domain Domain { get; set; }
        public Domain Target { get; set; }
        public Domain Target2 { get; set; }
        public Domain Position { get; set; }
        public Domain Initiator { get; set; }

        public int TypeInt { get => (int)Type; set => Type = (enArmyCommandType)value; }

        internal bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}
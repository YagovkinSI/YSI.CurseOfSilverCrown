﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSI.CurseOfSilverCrown.Core.Database.Models;
using YSI.CurseOfSilverCrown.Core.Database.Enums;

namespace YSI.CurseOfSilverCrown.Core.Commands
{
    public class WarSupportAttackCommand : BaseCommand
    {
        public WarSupportAttackCommand(Unit command)
            : base(command)
        {
            TypeInt = (int)enArmyCommandType.WarSupportAttack;
        }

        public override string Name => "Помощь в нападении";

        public override string[] Descriptions => new[] 
        {
            "Помощь в нападении - команда помочь одному владению атаковать другое.",
            "Если вы отправляете воинов помогать в нападении, то они не смогут в этом ходу защищать ваше владение."
        };

        public override bool IsSingleCommand => false;

        public override bool NeedTarget => true;

        public override string TargetName => "В нападении на владение";


        public override bool NeedTarget2 => true;

        public override string Target2Name => "Помочь владению";


        public override bool NeedCoffers => false;

        public override bool NeedWarriors => true;
    }
}

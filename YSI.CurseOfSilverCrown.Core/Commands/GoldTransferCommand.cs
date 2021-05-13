﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSI.CurseOfSilverCrown.Core.Database.Models;
using YSI.CurseOfSilverCrown.Core.Database.Enums;
using YSI.CurseOfSilverCrown.Core.Parameters;

namespace YSI.CurseOfSilverCrown.Core.Commands
{
    public class GoldTransferCommand : BaseCommand
    {
        public GoldTransferCommand(Command command)
            : base(command)
        {
            Type = enCommandType.GoldTransfer;
        }

        public override string Name => "Отправка золота";

        public override string[] Descriptions => new[]
        {
            $"Отправка золота - отправка средств из казны в другую провинцию для помощи в развитии или оплаты оговоренных услуг.",
            $"За один сезон вы не можете отправить больше {GoldTransferHelper.MaxGoldTransfer} золотых монет."
        };

        public override bool IsSingleCommand => false;

        public override bool NeedTarget => true;

        public override string TargetName => "Отправить золото в провинцию";

        public override bool NeedTarget2 => false;

        public override string Target2Name => string.Empty;

        public override bool NeedCoffers => true;
        public override int MaxCoffers => GoldTransferHelper.MaxGoldTransfer;

        public override bool NeedWarriors => false;
    }
}
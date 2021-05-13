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
    public class FortificationsCommand : BaseCommand
    {
        public FortificationsCommand(Command command)
            : base(command)
        {
            Type = enCommandType.Fortifications;
        }

        public override string Name => "Вложение средств в постройку укреплений";

        public override string[] Descriptions => new[] 
        {
            "Вложение средства в постройку укреплений - действие позволяющее повысить обороноспособность провинции от захвата.",
            "Вложения подразумевают постройку замка, рва, стен и другое. Выгода от постепенно вложений угасает, " +
            "так постройка огромного замка стоит бонословных средств, но не даёт такой же прирост к защите.",
            "В целом вкладывать деньги в укрепления можно до бесконечности, но с каждым разом они будут всё менее выгодны.",
            "Все войска учавствующие в защите провинции получают бонус от укрплений.",
            "Укрпления требуют содержания, равное 10% стоимости укрплений.",
        };

        public override bool IsSingleCommand => true;

        public override bool NeedTarget => false;

        public override string TargetName => string.Empty;


        public override bool NeedTarget2 => false;

        public override string Target2Name => string.Empty;


        public override bool NeedCoffers => true;

        public override bool NeedWarriors => false;
    }
}

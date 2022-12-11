﻿using System;
using System.Linq;
using YSI.CurseOfSilverCrown.Core.Database.Enums;
using YSI.CurseOfSilverCrown.Core.Database.Helpers;
using YSI.CurseOfSilverCrown.Core.Database.Models;
using YSI.CurseOfSilverCrown.Core.Database.Models.GameWorld;
using YSI.CurseOfSilverCrown.Core.Parameters;
using YSI.CurseOfSilverCrown.Core.Utils;

namespace YSI.CurseOfSilverCrown.Core.Database.PregenDatas
{
    internal static class PregenData
    {
        private static readonly Turn firstTurn = new Turn
        {
            Id = 1,
            Started = DateTime.MinValue,
            IsActive = true
        };

        private static readonly GameSession firstGameSession = new GameSession
        {
            Id = 1,
            EndSeesionTurnId = int.MaxValue,
            StartSeesionTurnId = 1,
            NumberOfGame = 1
        };

        public static Domain[] Organizations =>
            PregenDomainConstants.Array
                .Select(p => new Domain
                {
                    Id = p.Id,
                    Name = p.Name,
                    MoveOrder = p.Size,
                    TurnOfDefeat = int.MinValue,
                    Coffers = RandomHelper.AddRandom(CoffersParameters.StartCount, randomNumber: ((p.Id + 1) * p.Id) % 10 / 10.0, roundRequest: -1),
                    Fortifications = RandomHelper.AddRandom(FortificationsParameters.StartCount, randomNumber: ((p.Id + 2) * p.Id) % 10 / 10.0, roundRequest: -1),
                    PersonId = p.Id
                })
                .ToArray();

        public static Person[] Persons =>
            PregenDomainConstants.Array
                .Select(p => new Person
                {
                    Id = p.Id,
                    Name = "Эйгон " + p.Id.ToString()
                })
                .ToArray();

        public static Unit[] Units =>
            PregenDomainConstants.Array
                .Select(p => new Unit
                {
                    Id = p.Id,
                    DomainId = p.Id,
                    PositionDomainId = p.Id,
                    Warriors = RandomHelper.AddRandom(WarriorParameters.StartCount, randomNumber: (p.Id * p.Id) % 10 / 10.0),
                    Type = enArmyCommandType.WarSupportDefense,
                    TargetDomainId = p.Id,
                    InitiatorPersonId = p.Id,
                    Status = enCommandStatus.ReadyToMove,
                    ActionPoints = WarConstants.ActionPointsFullCount
                })
                .ToArray();

        internal static Turn GetFirstTurn()
        {
            return firstTurn;
        }

        internal static GameSession GetFirstGameSession()
        {
            return firstGameSession;
        }

        internal static Route[] Routes =>
            PregenDomainConstants.Array
                .SelectMany(b => b.BorderingDomainModelIds
                    .Select(r => new Route
                    {
                        FromDomainId = b.Id,
                        ToDomainId = r
                    }))
                .ToArray();
    }
}

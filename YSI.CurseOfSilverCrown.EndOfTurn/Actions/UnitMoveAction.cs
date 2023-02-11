﻿using System;
using System.Collections.Generic;
using YSI.CurseOfSilverCrown.Core.Database.EF;
using YSI.CurseOfSilverCrown.Core.Database.Enums;
using YSI.CurseOfSilverCrown.Core.Database.Models;
using YSI.CurseOfSilverCrown.Core.Game.Map.Routes;
using YSI.CurseOfSilverCrown.EndOfTurn.Event;

namespace YSI.CurseOfSilverCrown.EndOfTurn.Actions
{
    internal class UnitMoveAction : UnitActionBase
    {
        private int MovingTarget { get; set; }
        private bool NeedIntoTarget { get; set; }

        public UnitMoveAction(ApplicationDbContext context, Turn currentTurn, int unitId)
            : base(context, currentTurn, unitId)
        {
        }

        public override bool CheckValidAction()
        {
            var targetExist = SetMoveTarget();

            return targetExist &&
                Unit.PositionDomainId != MovingTarget;
        }

        private bool SetMoveTarget()
        {
            switch (Unit.Type)
            {
                case enArmyCommandType.ForDelete:
                    return false;
                case enArmyCommandType.CollectTax:
                    NeedIntoTarget = true;
                    MovingTarget = Unit.DomainId;
                    return true;
                case enArmyCommandType.War:
                case enArmyCommandType.WarSupportAttack:
                case enArmyCommandType.WarSupportDefense:
                    if (Unit.TargetDomainId == null)
                        return false;
                    NeedIntoTarget = Unit.Type == enArmyCommandType.WarSupportDefense;
                    MovingTarget = Unit.TargetDomainId.Value;
                    return true;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override bool Execute()
        {
            var reasonMovement = Unit.Type switch
            {
                enArmyCommandType.ForDelete => enMovementReason.Retreat,
                enArmyCommandType.War => enMovementReason.Atack,
                enArmyCommandType.CollectTax => enMovementReason.Defense,
                enArmyCommandType.WarSupportDefense => enMovementReason.Defense,
                enArmyCommandType.WarSupportAttack => enMovementReason.SupportAttack,
                _ => throw new NotImplementedException(),
            };
            var routeFindParameters = new RouteFindParameters(Unit, reasonMovement, MovingTarget);
            var route = RouteHelper.FindRoute(Context, routeFindParameters);
            var newPosition = route == null
                ? Unit.PositionDomainId.Value
                : route[1].Id;
            CreateEvent(newPosition);
            Unit.PositionDomainId = newPosition;
            Context.Update(Unit);
            return true;
        }

        private void CreateEvent(int newPostionId)
        {
            var unitMoving = Unit.PositionDomainId != newPostionId;
            var type = unitMoving
                ? enEventResultType.UnitMove
                : enEventResultType.UnitCantMove;
            var eventStoryResult = new EventStoryResult(type);
            eventStoryResult.AddEventOrganization(Unit.Domain.Id, enEventOrganizationType.Main, new List<EventParametrChange>());
            eventStoryResult.AddEventOrganization(Unit.PositionDomainId.Value, enEventOrganizationType.Vasal, new List<EventParametrChange>());
            eventStoryResult.AddEventOrganization(unitMoving ? newPostionId : Unit.TargetDomainId.Value, enEventOrganizationType.Target, new List<EventParametrChange>());
            CreateEventStory(eventStoryResult, new Dictionary<int, int> { { Unit.DomainId, 100 } });
        }
    }
}

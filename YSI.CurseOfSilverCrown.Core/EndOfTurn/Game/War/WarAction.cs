﻿using YSI.CurseOfSilverCrown.Core.Database.Enums;
using YSI.CurseOfSilverCrown.Core.Database.Models;
using YSI.CurseOfSilverCrown.Core.Database.Models.GameWorld;
using YSI.CurseOfSilverCrown.Core.Game.Map.Routes;
using YSI.CurseOfSilverCrown.Core.Helpers;
using YSI.CurseOfSilverCrown.Core.MainModels;
using YSI.CurseOfSilverCrown.EndOfTurn.Actions;

namespace YSI.CurseOfSilverCrown.EndOfTurn.Game.War
{
    internal partial class WarAction : UnitActionBase
    {
        private WarActionParameters _warActionParameters;

        public WarAction(ApplicationDbContext context, Turn currentTurn, int unitId)
                : base(context, currentTurn, unitId)
        {
        }

        public override bool CheckValidAction()
        {
            return Unit.Type == enArmyCommandType.War &&
                Unit.TargetDomainId != null &&
                Unit.Status == enCommandStatus.ReadyToMove &&
                RouteHelper.IsNeighbors(Context, Unit.PositionDomainId.Value, Unit.TargetDomainId.Value) &&
                !KingdomHelper.IsSameKingdoms(Context.Domains, Unit.Domain, Unit.Target);
        }

        protected override bool Execute()
        {
            _warActionParameters = new WarActionParameters(Context, Unit);

            while (!_warActionParameters.WarIsOver)
            {
                RetreatCheck();
                CalcWarActionStage();
            }

            CalsWarResult();

            CreateEvent();

            return true;
        }

        private void RetreatCheck()
        {
            var task = new WarActionRetreatCheckTask(_warActionParameters);
            task.Execute();
        }

        private void CalcWarActionStage()
        {
            var task = new WarActionStageCalcTask(_warActionParameters);
            task.Execute();
        }

        private void CalsWarResult()
        {
            var resultCalcTask = new WarActionResultCalcTask(Context, _warActionParameters, CurrentTurn);
            resultCalcTask.Execute();
        }

        private void CreateEvent()
        {
            var task = new WarEventCreateTask(Context, _warActionParameters);
            task.Execute();
            CreateEventStory(task.EventStoryResult, task.DommainEventStories);
        }
    }
}

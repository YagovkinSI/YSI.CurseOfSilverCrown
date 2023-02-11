﻿using System;
using YSI.CurseOfSilverCrown.Core.Utils;
using YSI.CurseOfSilverCrown.EndOfTurn.Actions;

namespace YSI.CurseOfSilverCrown.EndOfTurn.Game.War
{
    internal class WarActionStageCalcTask
    {
        private readonly WarActionParameters _warActionParameters;

        public WarActionStageCalcTask(WarActionParameters warActionParameters)
        {
            _warActionParameters = warActionParameters;
        }

        internal void Execute()
        {
            var warriorCountByType = _warActionParameters.GetWarriorCountByType();
            _warActionParameters.WarActionStage =
                WarActionHelper.CheckWarActionStage(warriorCountByType, _warActionParameters.WarActionStage);

            switch (_warActionParameters.WarActionStage)
            {
                case enWarActionStage.Siege:
                    RunSiegeStage();
                    break;
                case enWarActionStage.Assault:
                    RunAssaultStage();
                    break;
                case enWarActionStage.Battle:
                    RunBattleStage();
                    break;
                case enWarActionStage.AgressorWin:
                case enWarActionStage.DefenderWin:
                    break;
                default:
                    throw new NotImplementedException($"Неизвестный тип {_warActionParameters.WarActionStage}");
            }
        }

        private void RunSiegeStage()
        {
            var warriorCountByType = _warActionParameters.GetWarriorCountByType();
            var defendersInCastle = warriorCountByType[enTypeOfWarrior.TargetDefense];
            var agressorCount = warriorCountByType.GetAllOnSide(true);
            if (defendersInCastle == 0 || agressorCount == 0)
                return;

            SiegeProcess(agressorCount);

            if (_warActionParameters.CurrentFortifications <= 0)
            {
                _warActionParameters.DayOfWar += new Random().Next(1, 7);
                _warActionParameters.WarActionStage = enWarActionStage.Assault;
                _warActionParameters.IsBreached = true;
            }
            else
            {
                _warActionParameters.CurrentFortifications /= 2;
                _warActionParameters.DayOfWar += 7;
            }
        }

        private void SiegeProcess(int agressorCount)
        {
            var siegeRoll = RandomHelper.Random2d6();
            if (siegeRoll == 0)
            {
                var percent = (int)Math.Ceiling(new Random().NextDouble() * 5);
                _warActionParameters.AddLosses(true, percent, enTypeOfWarrior.Agressor, enTypeOfWarrior.AgressorSupport);
                _warActionParameters.AddLosses(false, new Random().Next(0, 10), enTypeOfWarrior.TargetDefense);
            }
            else if (siegeRoll == 12)
            {
                _warActionParameters.CurrentFortifications = 0;
                _warActionParameters.AddLosses(false, new Random().Next(0, 50), enTypeOfWarrior.Agressor, enTypeOfWarrior.AgressorSupport);
                _warActionParameters.AddLosses(false, new Random().Next(0, 50), enTypeOfWarrior.TargetDefense);
            }
            else
            {
                _warActionParameters.CurrentFortifications -= agressorCount * siegeRoll / 3.5;
                _warActionParameters.AddLosses(false, new Random().Next(0, 30), enTypeOfWarrior.Agressor, enTypeOfWarrior.AgressorSupport);
                _warActionParameters.AddLosses(false, new Random().Next(0, 10), enTypeOfWarrior.TargetDefense);
            }
        }

        private void RunAssaultStage()
        {
            var warriorCountByType = _warActionParameters.GetWarriorCountByType();
            var (defendersCount, agressorCount) = warriorCountByType.GetWarriorCountBySide();
            if (defendersCount == 0 || agressorCount == 0)
                return;

            AssaultProcess(defendersCount, agressorCount);

            var assaultRoll = RandomHelper.Random2d6();
            if (assaultRoll > 8)
                _warActionParameters.WarActionStage = enWarActionStage.Battle;
            _warActionParameters.DayOfWar += new Random().Next(0, 2);
        }

        private void AssaultProcess(int defendersCount, int agressorCount)
        {
            var defendersInCastle = _warActionParameters.GetWarriorCount(enTypeOfWarrior.TargetDefense);
            var defendersOutCastle = defendersCount - defendersInCastle;

            if (defendersInCastle != 0)
            {
                var agressorLosses = Math.Min(defendersInCastle, 200) * 0.3 * RandomHelper.Random2d6() / 7.0;
                var inCastleLosses = Math.Min(agressorCount, 200) * 0.15 * RandomHelper.Random2d6() / 7.0;
                _warActionParameters.AddLosses(false, (int)agressorLosses, enTypeOfWarrior.Agressor, enTypeOfWarrior.AgressorSupport);
                _warActionParameters.AddLosses(false, (int)inCastleLosses, enTypeOfWarrior.TargetDefense);
            }

            if (defendersOutCastle != 0)
            {
                var countInBattle = Math.Max(defendersOutCastle, agressorCount) / 10;
                var agressorLosses = Math.Min(defendersOutCastle, countInBattle) * 0.25 * RandomHelper.Random2d6() / 7;
                var outCastleLosses = Math.Min(agressorCount, countInBattle) * 0.3 * RandomHelper.Random2d6() / 7;
                _warActionParameters.AddLosses(false, (int)agressorLosses, enTypeOfWarrior.Agressor, enTypeOfWarrior.AgressorSupport);
                _warActionParameters.AddLosses(false, (int)outCastleLosses, enTypeOfWarrior.TargetSupport);
            }
        }

        private void RunBattleStage()
        {
            var warriorCountByType = _warActionParameters.GetWarriorCountByType();
            var (defendersCount, agressorCount) = warriorCountByType.GetWarriorCountBySide();
            if (defendersCount == 0 || agressorCount == 0)
                return;

            var countInBattle = Math.Max(defendersCount, agressorCount) / 10;
            var agressorLosses = Math.Min(defendersCount, countInBattle) * 0.3 * RandomHelper.Random2d6() / 7;
            var defendersLosses = Math.Min(agressorCount, countInBattle) * 0.3 * RandomHelper.Random2d6() / 7;
            _warActionParameters.AddLosses(false, (int)agressorLosses, enTypeOfWarrior.Agressor, enTypeOfWarrior.AgressorSupport);
            _warActionParameters.AddLosses(false, (int)defendersLosses, enTypeOfWarrior.TargetDefense, enTypeOfWarrior.TargetDefense);
            _warActionParameters.DayOfWar += new Random().Next(0, 2);
        }
    }
}
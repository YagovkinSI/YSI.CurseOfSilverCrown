﻿using System;
using System.Collections.Generic;
using System.Linq;
using YSI.CurseOfSilverCrown.Core;
using YSI.CurseOfSilverCrown.Core.Database.EF;
using YSI.CurseOfSilverCrown.Core.Database.Enums;
using YSI.CurseOfSilverCrown.Core.Database.Models;
using YSI.CurseOfSilverCrown.Core.Database.Models.GameWorld;
using YSI.CurseOfSilverCrown.Core.Game.Map.Routes;
using YSI.CurseOfSilverCrown.Core.Helpers;
using YSI.CurseOfSilverCrown.Core.Interfaces;
using YSI.CurseOfSilverCrown.Core.Parameters;
using YSI.CurseOfSilverCrown.EndOfTurn.Actions;
using YSI.CurseOfSilverCrown.EndOfTurn.AI;
using YSI.CurseOfSilverCrown.EndOfTurn.Game.War;
using YSI.CurseOfSilverCrown.EndOfTurn.Helpers;

namespace YSI.CurseOfSilverCrown.EndOfTurn
{
    public class EndOfTurnTask
    {
        private readonly ApplicationDbContext Context;
        private Turn CurrentTurn;

        private int eventNumber;
        private const int SubTurnCount = 10;

        public EndOfTurnTask(ApplicationDbContext context)
        {
            Context = context;
        }

        public Response<bool> Execute()
        {
            try
            {
                DeactivateCurrentTurn();
                Prepare();
                AICommandsPrepare();
                RunUnits();
                RunCommands();
                RetrearUnits();
                AINegotiveEvents();
                PrepareForNewTurn();
                CreateNewTurn();
                return new Response<bool>(true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void AINegotiveEvents()
        {
            var persons = Context.Persons.ToList();
            foreach (var person in persons)
            {
                if (person.User != null && person.User.LastActivityTime > DateTime.Now - TimeSpan.FromDays(5))
                    continue;

                var domain = person.Domains.SingleOrDefault();
                if (domain == null)
                    continue;

                if (new Random().NextDouble() > 0.1)
                    continue;

                SetNegativeEvent(domain);
            }
        }

        private void SetNegativeEvent(Domain domain)
        {
            var negativeAction = ChooseNegativeAction(domain);
            if (negativeAction == null)
                return;
            eventNumber = negativeAction.ExecuteAction(eventNumber);
        }

        private DomainActionBase ChooseNegativeAction(Domain domain)
        {
            var townFireAction = new TownFireAction(Context, CurrentTurn, domain);
            var castleFireAction = new CastleFireAction(Context, CurrentTurn, domain);
            var diseaseAction = new DiseaseAction(Context, CurrentTurn, domain);

            var investmentCoef = (double)domain.Investments / InvestmentsHelper.StartInvestment;
            var fortificationCoef = (double)domain.Fortifications / FortificationsParameters.StartCount;
            var unitInDomain = domain.Units.Where(u => u.PositionDomainId == domain.Id).Sum(u => u.Warriors);
            var warrioirInDomainCoef = (double)unitInDomain / WarriorParameters.StartCount;

            var dict = new Dictionary<DomainActionBase, double>()
            {
                { townFireAction, investmentCoef },
                { castleFireAction, fortificationCoef / 3 },
                { diseaseAction, investmentCoef / 2 + warrioirInDomainCoef },
            };
            var action = dict
                .OrderByDescending(p => p.Value)
                .FirstOrDefault(p => p.Key.CheckValidAction())
                .Key;
            return action;
        }

        private void AICommandsPrepare()
        {
            var persons = Context.Persons.ToList();
            foreach (var person in persons)
            {
                if (person.User != null && person.User.LastActivityTime > DateTime.Now - TimeSpan.FromDays(5))
                    continue;

                var isSameInitiator = person.Domains
                    .Single()
                    .Commands
                    .Any(u => u.InitiatorPersonId == person.Id);
                if (!isSameInitiator)
                    continue;

                var userAi = new UserAI(Context, person.Id, CurrentTurn);
                userAi.SetCommands();
            }
        }

        private void UpdateEventNumber()
        {
            eventNumber = 0;
            var eventsOfCurrentTurn = Context.EventStories
                .Where(e => e.TurnId == CurrentTurn.Id);
            if (eventsOfCurrentTurn.Any())
                eventNumber = eventsOfCurrentTurn.Max(e => e.Id) + 1;
        }

        private void RunCommands()
        {
            UpdateEventNumber();

            var runCommands = Context.Commands.ToList();
            var organizations = Context.Domains.ToArray();

            ExecuteCommands(runCommands, organizations);

            Context.RemoveRange(runCommands);

            Context.SaveChanges();
        }

        private void ExecuteCommands(List<Command> runCommands, Domain[] organizations)
        {
            ExecuteRebelionAction(CurrentTurn, runCommands);
            ExecuteVassalTransferAction(CurrentTurn, runCommands);
            ExecuteGoldTransferAction(CurrentTurn, runCommands);
            ExecuteGrowthAction(CurrentTurn, runCommands);
            ExecuteInvestmentsAction(CurrentTurn, runCommands);
            ExecuteFortificationsAction(CurrentTurn, runCommands);
            ExecuteTaxAction(CurrentTurn, organizations);
            ExecuteFortificationsMaintenanceAction(CurrentTurn, organizations);
            ExecuteMaintenanceAction(CurrentTurn, organizations);
            ExecuteMutinyAction(CurrentTurn, organizations);
        }

        private void RunUnits()
        {
            UpdateEventNumber();

            var runUnitIds = Context.Units
                .OrderBy(u => u.Position.MoveOrder + (10000.0 - (double)u.Type / 10000.0))
                .Select(u => u.Id)
                .ToList();

            CheckDefenseOnPosition(runUnitIds);
            for (var subTurn = 0; subTurn < SubTurnCount; subTurn++)
            {
                foreach (var unitId in runUnitIds)
                {
                    var unit = Context.Units.Find(unitId);
                    if (IsCompleted(unit, subTurn))
                        continue;
                    CheckCommand(unit);
                    unit.ActionPoints -= WarConstants.ActionPointForMoveWarriors;
                    Context.Update(unit);
                    Context.SaveChanges();
                }
            }

            SetCompletedCommandForAll(runUnitIds);
            DeleteDestroyedInits();
            Context.SaveChanges();
        }

        private void DeleteDestroyedInits()
        {
            var unitForDelete = Context.Units.Where(c => c.Warriors <= 0 || c.Status == enCommandStatus.Destroyed);
            Context.RemoveRange(unitForDelete);
        }

        private void SetCompletedCommandForAll(List<int> runUnitIds)
        {
            foreach (var unitId in runUnitIds)
            {
                var unit = Context.Units.Find(unitId);
                unit.Status = enCommandStatus.Complited;
                Context.Update(unit);
            }
        }

        private bool IsCompleted(Unit unit, int subTurn)
        {
            return unit.Status == enCommandStatus.Complited ||
                unit.ActionPoints < WarConstants.ActionPointsFullCount - subTurn * WarConstants.ActionPointForMoveWarriors;
        }

        private void CheckCommand(Unit unit)
        {
            switch (unit.Type)
            {
                case enArmyCommandType.CollectTax:
                    CheckCollectTaxCommand(unit);
                    break;
                case enArmyCommandType.War:
                    CheckWarCommand(unit);
                    break;
                case enArmyCommandType.WarSupportAttack:
                    CheckWarSupportAttackCommand(unit);
                    break;
                case enArmyCommandType.WarSupportDefense:
                    CheckWarSupportDefenseCommand(unit);
                    break;
                case enArmyCommandType.ForDelete:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void CheckWarSupportDefenseCommand(Unit unit)
        {
            if (unit.PositionDomainId != unit.TargetDomainId)
            {
                var task = new UnitMoveAction(Context, CurrentTurn, unit.Id);
                eventNumber = task.ExecuteAction(eventNumber);
                Context.SaveChanges();
            }
            if (unit.PositionDomainId == unit.TargetDomainId)
            {
                unit = Context.Units.Find(unit.Id);
                unit.Status = enCommandStatus.Complited;
                Context.Update(unit);
                Context.SaveChanges();
            }
        }

        private void CheckWarSupportAttackCommand(Unit unit)
        {
            if (!RouteHelper.IsNeighbors(Context, unit.PositionDomainId.Value, unit.TargetDomainId.Value))
            {
                var task = new UnitMoveAction(Context, CurrentTurn, unit.Id);
                eventNumber = task.ExecuteAction(eventNumber);
                Context.SaveChanges();
            }
            else
            {
                unit.Status = enCommandStatus.Complited;
                Context.Update(unit);
                Context.SaveChanges();
            }
        }

        private void CheckWarCommand(Unit unit)
        {
            if (!RouteHelper.IsNeighbors(Context, unit.PositionDomainId.Value, unit.TargetDomainId.Value))
            {
                var task = new UnitMoveAction(Context, CurrentTurn, unit.Id);
                eventNumber = task.ExecuteAction(eventNumber);
                Context.SaveChanges();
            }
            else
            {
                var task = new WarAction(Context, CurrentTurn, unit.Id);
                eventNumber = task.ExecuteAction(eventNumber);
                Context.SaveChanges();

                var retreats = Context.Units
                        .Where(u => u.Status == enCommandStatus.Retreat)
                        .ToList();
                foreach (var retreatUnit in retreats)
                {
                    var retreatTask = new RetreatAction(Context, CurrentTurn, retreatUnit.Id);
                    eventNumber = retreatTask.ExecuteAction(eventNumber);
                    Context.SaveChanges();
                }
            }
        }

        private void CheckCollectTaxCommand(Unit unit)
        {
            if (unit.PositionDomainId != unit.DomainId)
            {
                var task = new UnitMoveAction(Context, CurrentTurn, unit.Id);
                eventNumber = task.ExecuteAction(eventNumber);
                Context.SaveChanges();
            }
            if (unit.PositionDomainId == unit.DomainId)
            {
                unit.Status = enCommandStatus.Complited;
                Context.Update(unit);
                Context.SaveChanges();
            }
        }

        private void CheckDefenseOnPosition(List<int> runUnitIds)
        {
            foreach (var unitId in runUnitIds)
            {
                var unit = Context.Units.Find(unitId);
                if (unit.Type == enArmyCommandType.WarSupportDefense &&
                    unit.TargetDomainId == unit.PositionDomainId ||
                    unit.Type == enArmyCommandType.CollectTax &&
                    unit.DomainId == unit.PositionDomainId)
                {
                    unit.Status = enCommandStatus.Complited;
                    Context.Update(unit);
                }
            }
            Context.SaveChanges();
        }

        private void RetrearUnits()
        {
            var runUnitIds = Context.Units
                .OrderBy(u => u.Warriors)
                .Select(u => u.Id)
                .ToList();

            //Отступаем или уничтожаемся
            foreach (var unitId in runUnitIds)
            {
                var unit = Context.Units.Find(unitId);
                if (unit.PositionDomainId == unit.DomainId)
                    continue;

                var unitDomain = Context.Domains.Find(unit.DomainId);
                var unitPosition = Context.Domains.Find(unit.PositionDomainId.Value);
                if (KingdomHelper.IsSameKingdoms(Context.Domains, unitDomain, unitPosition) ||
                    DomainRelationsHelper.HasPermissionOfPassage(Context, unitDomain.Id, unitPosition.Id))
                {
                    continue;
                }

                unit.Status = enCommandStatus.Retreat;
                Context.Update(unit);
                var task = new RetreatAction(Context, CurrentTurn, unit.Id);
                eventNumber = task.ExecuteAction(eventNumber);
                Context.Update(unit);
                Context.SaveChanges();
            }

            var unitForDelete = Context.Units.Where(c => c.Warriors <= 0 || c.Status == enCommandStatus.Destroyed);
            Context.RemoveRange(unitForDelete);

            Context.SaveChanges();
        }

        private void PrepareForNewTurn()
        {
            PrepareUnitsForNewTurn();
            PrepareCommandsForNewTurn();
        }

        private void PrepareCommandsForNewTurn()
        {
            CreatorCommandForNewTurn.CreateNewCommandsForOrganizations(Context);
            Context.SaveChanges();
        }

        private void PrepareUnitsForNewTurn()
        {
            var runUnits = Context.Units.ToList();

            var unitCompleted = runUnits.Where(c => c.Status == enCommandStatus.Complited);
            foreach (var unit in unitCompleted)
            {
                if (unit.Type != enArmyCommandType.CollectTax && unit.Type != enArmyCommandType.WarSupportDefense)
                {
                    unit.Type = enArmyCommandType.WarSupportDefense;
                    unit.Target2DomainId = null;
                    unit.TargetDomainId = unit.DomainId;
                }
                unit.Status = enCommandStatus.ReadyToMove;
                unit.ActionPoints = WarConstants.ActionPointsFullCount;
            }
            Context.UpdateRange(unitCompleted);

            Context.SaveChanges();
        }

        private void Prepare()
        {
            PrepareCommands();
            PrepareUnits();
        }

        private int? GetInitiatorRunIdForPrepareCommands(IEnumerable<IGrouping<int, Command>> groups, Domain domain)
        {
            if (!groups.Any())
            {
                return null;
            }
            else if (groups.Count() > 1)
            {
                var domainIsActive = domain.Person.User != null &&
                                     domain.Person.User.LastActivityTime > DateTime.UtcNow - new TimeSpan(24, 0, 0);
                return domainIsActive || !groups.Any(g => g.Key == domain.Suzerain.PersonId)
                    ? domain.PersonId
                    : domain.Suzerain.PersonId;
            }
            else
            {
                return domain.PersonId;
            }
        }

        private void PrepareCommands()
        {
            var domains = Context.Domains.ToList();
            foreach (var domain in domains)
            {
                GameErrorHelper.CheckAndFix(Context, domain.Id, domain.PersonId);

                var groups = domain.Commands
                    .GroupBy(c => c.InitiatorPersonId);
                var initiatorRunId = GetInitiatorRunIdForPrepareCommands(groups, domain);
                if (initiatorRunId == null)
                    continue;

                var groupsForDelete = groups.Where(g => g.Key != initiatorRunId);
                foreach (var group in groupsForDelete)
                    Context.RemoveRange(group.ToList());

                var groupForRun = groups.Single(g => g.Key == initiatorRunId);
                foreach (var command in groupForRun)
                    command.InitiatorPersonId = domain.PersonId;
                Context.UpdateRange(groupForRun);
                Context.SaveChanges();
            }
        }

        private void PrepareUnits()
        {
            var domains = Context.Domains.ToList();
            foreach (var domain in domains)
            {
                var initiatorRunId = 0;
                var groups = domain.Units
                    .GroupBy(c => c.InitiatorPersonId);
                if (groups.Count() > 1)
                {
                    var domainIsActive = domain.Person.User != null &&
                                         domain.Person.User.LastActivityTime > DateTime.UtcNow - new TimeSpan(24, 0, 0);
                    initiatorRunId = domainIsActive
                        ? domain.PersonId
                        : domain.Suzerain.PersonId;
                }
                else
                {
                    initiatorRunId = domain.PersonId;
                }

                var groupsForDelete = groups.Where(g => g.Key != initiatorRunId);
                foreach (var group in groupsForDelete)
                    Context.RemoveRange(group.ToList());

                var groupForRun = groups.SingleOrDefault(g => g.Key == initiatorRunId);
                if (groupForRun != null)
                {
                    foreach (var unit in groupForRun)
                        unit.InitiatorPersonId = domain.PersonId;
                    Context.UpdateRange(groupForRun);
                }
                Context.SaveChanges();
            }
        }

        private void DeactivateCurrentTurn()
        {
            CurrentTurn = Context.Turns.
                  SingleOrDefault(t => t.IsActive);

            if (CurrentTurn != null)
            {
                CurrentTurn.IsActive = false;
                Context.Update(CurrentTurn);
                Context.SaveChanges();
            }
            else
            {
                CurrentTurn = Context.Turns
                    .OrderByDescending(t => t.Id)
                    .First();
            }
        }

        private void ExecuteGoldTransferAction(Turn currentTurn, IEnumerable<Command> currentCommands)
        {
            var commandList = currentCommands.Where(c => c.Type == enCommandType.GoldTransfer);
            foreach (var command in commandList)
            {
                var task = new GoldTransferAction(Context, currentTurn, command);
                eventNumber = task.ExecuteAction(eventNumber);
            }
        }

        private void ExecuteGrowthAction(Turn currentTurn, IEnumerable<Command> currentCommands)
        {
            var commandList = currentCommands.Where(c => c.Type == enCommandType.Growth);
            foreach (var command in commandList)
            {
                var task = new GrowthAction(Context, currentTurn, command);
                eventNumber = task.ExecuteAction(eventNumber);
            }
        }

        private void ExecuteInvestmentsAction(Turn currentTurn, IEnumerable<Command> currentCommands)
        {
            var commandList = currentCommands.Where(c => c.Type == enCommandType.Investments);
            foreach (var command in commandList)
            {
                var task = new InvestmentsAction(Context, currentTurn, command);
                eventNumber = task.ExecuteAction(eventNumber);
            }
        }

        private void ExecuteFortificationsAction(Turn currentTurn, IEnumerable<Command> currentCommands)
        {
            var commandList = currentCommands.Where(c => c.Type == enCommandType.Fortifications);
            foreach (var command in commandList)
            {
                var task = new FortificationsAction(Context, currentTurn, command);
                eventNumber = task.ExecuteAction(eventNumber);
            }
        }

        private void ExecuteRebelionAction(Turn currentTurn, IEnumerable<ICommand> currentCommands)
        {
            var commandList = currentCommands.Where(c => c.TypeInt == (int)enCommandType.Rebellion);
            foreach (var command in commandList)
            {
                var task = new RebelionAction(Context, currentTurn, command as Command);
                eventNumber = task.ExecuteAction(eventNumber);
            }
        }

        private void ExecuteVassalTransferAction(Turn currentTurn, IEnumerable<ICommand> currentCommands)
        {
            var commandList = currentCommands.Where(c => c.TypeInt == (int)enCommandType.VassalTransfer);
            foreach (var command in commandList)
            {
                var task = new VassalTransferAction(Context, currentTurn, command as Command);
                eventNumber = task.ExecuteAction(eventNumber);
            }
        }

        private void ExecuteTaxAction(Turn currentTurn, params Domain[] organizations)
        {
            foreach (var organization in organizations)
            {
                var task = new TaxAction(Context, currentTurn, organization);
                eventNumber = task.ExecuteAction(eventNumber);
            }
        }

        private void ExecuteFortificationsMaintenanceAction(Turn currentTurn, params Domain[] organizations)
        {
            foreach (var organization in organizations)
            {
                var task = new FortificationsMaintenanceAction(Context, currentTurn, organization);
                eventNumber = task.ExecuteAction(eventNumber);
            }
        }

        private void ExecuteMaintenanceAction(Turn currentTurn, params Domain[] organizations)
        {
            foreach (var organization in organizations)
            {
                var task = new MaintenanceAction(Context, currentTurn, organization);
                eventNumber = task.ExecuteAction(eventNumber);
            }
        }

        private void ExecuteMutinyAction(Turn currentTurn, params Domain[] organizations)
        {
            foreach (var organization in organizations)
            {
                var task = new MutinyAction(Context, currentTurn, organization);
                eventNumber = task.ExecuteAction(eventNumber);
            }
        }

        private void CreateNewTurn()
        {
            var newTurn = new Turn
            {
                IsActive = true,
                Started = DateTime.UtcNow
            };
            Context.Add(newTurn);
            Context.SaveChanges();
        }
    }
}

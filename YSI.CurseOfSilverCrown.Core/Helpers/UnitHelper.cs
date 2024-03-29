﻿using System.Threading.Tasks;
using YSI.CurseOfSilverCrown.Core.Database;
using YSI.CurseOfSilverCrown.Core.Database.Units;

namespace YSI.CurseOfSilverCrown.Core.Helpers
{
    public static class UnitHelper
    {
        public static async Task<bool> TryUnion(this Unit unitTo, Unit unitFrom, ApplicationDbContext context)
        {
            if (unitTo.PositionDomainId != unitFrom.PositionDomainId ||
                unitTo.DomainId != unitFrom.DomainId)
            {
                return false;
            }

            unitTo.Warriors += unitFrom.Warriors;
            unitTo.Gold += unitFrom.Gold;
            context.Remove(unitFrom);
            context.Update(unitTo);
            await context.SaveChangesAsync();
            return true;
        }

        public static async Task<(bool, Unit)> TrySeparate(this Unit unit, int separateCount, ApplicationDbContext context)
        {
            if (unit == null || unit.Warriors <= separateCount)
                return (false, null);

            var newUnit = new Unit
            {
                Warriors = separateCount,
                Gold = 0,
                DomainId = unit.DomainId,
                PositionDomainId = unit.PositionDomainId,
                Status = unit.Status,
                Type = UnitCommandType.WarSupportDefense,
                TypeInt = (int)UnitCommandType.WarSupportDefense,
                TargetDomainId = unit.DomainId,
            };
            unit.Warriors -= separateCount;

            context.Update(unit);
            context.Add(newUnit);
            await context.SaveChangesAsync();
            return (true, newUnit);
        }

        public static async Task<bool> TryWar(this Unit unit, int targetDomainId, ApplicationDbContext context)
        {
            unit.TargetDomainId = targetDomainId;
            unit.Type = UnitCommandType.War;
            context.Update(unit);
            await context.SaveChangesAsync();
            return true;
        }
    }
}

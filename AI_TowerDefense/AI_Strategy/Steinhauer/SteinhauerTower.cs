using GameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_TowerDefense.Steinhauer
{
    public class SteinhauerTower : Tower
    {
        List<Unit> _enemyUnits = new();
        protected override bool TryGetTarget(out Unit unit)
        {
            _enemyUnits.Clear();

            for (int x = posX - range; x <= posX + range; x++)
            {
                for (int y = posY - range; y <= posY + range; y++)
                {
                    Cell cell = lane.GetCellAt(x, y);
                    if (cell == null)
                        continue;

                    unit = cell.Unit;
                    if (unit != null && unit.Type != type && unit.Health > 0)
                        _enemyUnits.Add(unit);
                }
            }
            unit = null;

            int lowestHealth = int.MaxValue;
            int highestPos = int.MinValue;
            foreach (Unit enemy in _enemyUnits)
            {
                // prioritise lower health
                if (enemy.Health < lowestHealth)
                {
                    unit = enemy;
                    lowestHealth = enemy.Health;
                    continue;
                }
                if (enemy.Health == lowestHealth)
                {
                    if (enemy.PosY > unit.PosY)
                        unit = enemy;
                }

                // prioritise position
                //if (enemy.PosY > highestPos)
                //{
                //    unit = enemy;
                //    highestPos = enemy.PosY;
                //    continue;
                //}
                //if (enemy.PosY == highestPos && enemy.Health < unit.Health)
                //    unit = enemy;
            }

            return unit != null;
        }
    }
}

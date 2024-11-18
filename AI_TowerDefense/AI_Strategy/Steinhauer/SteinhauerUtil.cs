using AI_Strategy;
using GameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_TowerDefense.Steinhauer
{
    internal class CellPosition
    {
        public int X;
        public int Y;
        internal CellPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class SteinhauerUtil
    {
        internal static List<Unit> GetAllEnemies(PlayerLane lane, string enemyType)
        {
            List<Unit> enemies = new List<Unit>();

            for (int x = 0; x < PlayerLane.WIDTH; x++)
                for (int y = 0; y < PlayerLane.HEIGHT; y++)
                {
                    Unit unit = lane.GetCellAt(x, y).Unit;
                    if (unit != null && unit.Type == enemyType)
                        enemies.Add(unit);
                }
            return enemies;
        }

        internal static Unit GetEnemySoldierClosestToDestination(List<Unit> enemies)
        {
            int highestY = int.MinValue;
            Unit furthestUnit = null;
            foreach(Unit enemy in enemies)
            {
                if (enemy.PosY > highestY)
                {
                    highestY = enemy.PosY;
                    furthestUnit = enemy;
                }
                else if (enemy.PosY == highestY)
                {
                    if (enemy.Health < furthestUnit.Health)
                        furthestUnit = enemy;
                }
            }
            return furthestUnit;
        }

        internal static int FindClosestCellToPositionIndex(int x, int y, List<CellPosition> cellPositions)
        {
            int index = -1;
            float closestDistance = float.MaxValue;
            for (int i = 0; i < cellPositions.Count; i++)
            {
                float distance = (Math.Abs(x - cellPositions[i].X) + Math.Abs(y - cellPositions[i].Y)) / 2f;
                if (distance > closestDistance)
                    continue;
                if (distance == closestDistance && index >= 0)
                {
                    if (cellPositions[i].Y < cellPositions[index].Y)
                        continue;
                    if ((cellPositions[i].X == 0 || cellPositions[i].X == PlayerLane.WIDTH - 1))
                        continue;
                }
                index = i;
                closestDistance = distance;
            }

            return index;
        }
    }
}

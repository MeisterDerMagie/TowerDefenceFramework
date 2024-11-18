using AI_Strategy;
using GameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameFramework.Player;

namespace AI_TowerDefense.Steinhauer
{
    internal class SteinhauerStrategy : AbstractStrategy
    {
        private static Random random = new Random();

        private UtilityAction _deployTowerAction;
        private UtilityAction _idleAction;
        private readonly List<UtilityAction> _towerPlacementActions;
        private readonly List<UtilityAction> _soldierPlacementActions;
        private Context _context = new();
        private List<CellPosition> _validCells = new();

        private int _turns = 0;

        public SteinhauerStrategy(Player player) : base(player)
        {
            _towerPlacementActions = new List<UtilityAction>();
            _soldierPlacementActions = new List<UtilityAction>();

            Consideration towerCost = new FloatBasedConsideration("cost", 1f, 2, 15, CurveType.Exponential, true, 2f);
            Consideration playerGoldAfterTowerPurchase = new FloatBasedConsideration("goldAfterPurchase", 1f, 14, 45, CurveType.Logarithmic, false, 1f);
            Consideration enemySoldiers = new FloatBasedConsideration("enemySoldiers", 2f, 6, 14, CurveType.Exponential, false, 5f);
            Consideration ownTowers = new FloatBasedConsideration("ownTowers", 0.7f, 0, 10, CurveType.Exponential, true, 3f);
            Consideration enoughGold = new CompareTwoValuesConsideration(1f, "cost", "gold", 0.1f, -1f, 1f);

            Consideration compositeTowerConsideration = new CompositeConsideration(new List<Consideration>() { towerCost, playerGoldAfterTowerPurchase, enemySoldiers, enoughGold }, true);
            _deployTowerAction = new UtilityAction(compositeTowerConsideration);

            Consideration idle = new ConstantValueConsideration(0.2f);
            _idleAction = new UtilityAction(idle);
        }

        public override void DeployTowers()
        {
            _turns++;

            if (_turns < 10)
                return;

            if (Tower.GetNextTowerCosts(player.HomeLane) >= player.Gold)
                return;
            
            if (_turns + PlayerLane.HEIGHT == 1000)
            {
                if (_deployTowerAction.Consideration is CompositeConsideration consideration)
                {
                    foreach (Consideration c in consideration.Considerations)
                    {
                        if (c.ContextKey == "goldAfterPurchase" && c is FloatBasedConsideration goldConsideration)
                            goldConsideration.MinValue = 3f;
                        else if (c.ContextKey == "ownTowers" && c is FloatBasedConsideration towerConsideration)
                            towerConsideration.MaxValue = 100;
                        else if (c.ContextKey == "cost" && c is FloatBasedConsideration costConsideration)
                            costConsideration.MaxValue = 1000;
                    }
                }
            }
            
            if (player.Gold < 8)
                return;
            
            bool idle = false;
            while (!idle)
            {
                UpdateContext();

                float idleValue = _idleAction.CalculateUtility(_context);
                float towerValue = _deployTowerAction.CalculateUtility(_context);

                if (idleValue > towerValue){
                    idle = true;
                    break;
                }
                if (!TryPlaceTower())
                    idle = true;
            }
        }

        private bool TryPlaceTower()
        {
            _validCells.Clear();
            for (int x = 0; x < PlayerLane.WIDTH; x++)
                for (int y = 0; y < PlayerLane.HEIGHT; y++)
                    if (IsTowerPlacementValid(x, y))
                        _validCells.Add(new CellPosition(x, y));
            if (_validCells.Count == 0)
                return false;

            Unit furthestSoldier = SteinhauerUtil.GetEnemySoldierClosestToDestination(SteinhauerUtil.GetAllEnemies(player.HomeLane, "S"));
            int index = furthestSoldier == null ? -1 : SteinhauerUtil.FindClosestCellToPositionIndex(furthestSoldier.PosX, furthestSoldier.PosY, _validCells);
            if (index == -1)
                index = random.Next(0, _validCells.Count);
            TowerPlacementResult result = player.TryBuyTower<SteinhauerTower>(_validCells[index].X, _validCells[index].Y);
            return result == TowerPlacementResult.Success;
        }

        private bool IsTowerPlacementValid(int x, int y)
        {
            if (y < PlayerLane.HEIGHT_OF_SAFETY_ZONE
                || y >= PlayerLane.HEIGHT
                || x < 0
                || x >= PlayerLane.WIDTH)
                return false;

            if (player.HomeLane.GetCellAt(x, y).Unit != null)
                return false;

            if (player.CheckAdjacentCellsAny(player.HomeLane, x, y, cell => cell.Unit is Tower))
                return false;
            return true;
        }

        private void UpdateContext()
        {
            _context.ConsiderationValues["cost"] = Tower.GetNextTowerCosts(player.HomeLane);
            _context.ConsiderationValues["gold"] = player.Gold;
            _context.ConsiderationValues["enemySoldiers"] = player.HomeLane.SoldierCount();
            _context.ConsiderationValues["ownSoldiers"] = player.EnemyLane.SoldierCount();
            _context.ConsiderationValues["enemyTowers"] = player.EnemyLane.TowerCount();
            _context.ConsiderationValues["ownTowers"] = player.HomeLane.TowerCount();
            _context.ConsiderationValues["goldAfterPurchase"] = _context.ConsiderationValues["gold"] - _context.ConsiderationValues["cost"];
        }

        public override void DeploySoldiers()
        {
            int amountToSpawn = player.Gold / 2;
            
            if (player.Gold - amountToSpawn * 2 < 3)
                amountToSpawn--;

            if (amountToSpawn <= 0)
                return;

            List<int> validPositions = new List<int>();
            for (int x = 0; x < PlayerLane.WIDTH; x++)
            {
                if (player.EnemyLane.GetCellAt(x, 0).Unit == null)
                    validPositions.Add(x);
            }

            for (int i = 0; i < amountToSpawn; i++)
            {
                int index = random.Next(0, validPositions.Count);
                player.TryBuySoldier<SteinhauerSoldier>(validPositions[index]);
                validPositions.RemoveAt(index);
                if (validPositions.Count == 0)
                    break;
            }
        }

        public override List<Soldier> SortedSoldierArray(List<Soldier> unsortedList)
        {
            return unsortedList.OrderByDescending(u => u.PosY)
            .ThenByDescending(u => u.Health)
            .ThenBy(u => u.PosX)
            .ToList();
        }

        public override List<Tower> SortedTowerArray(List<Tower> unsortedList)
        {
            return unsortedList.OrderByDescending(u => u.PosY)
            .ThenByDescending(u => u.Health)
            .ThenBy(u => u.PosX)
            .ToList();
        }
    }
}

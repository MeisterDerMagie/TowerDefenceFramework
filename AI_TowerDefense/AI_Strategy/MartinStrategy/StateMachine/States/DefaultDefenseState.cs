using System;
using System.Collections.Generic;
using System.Linq;
using AI_Strategy;
using GameFramework;

namespace Wichtel.StateMachine.States {
public class DefaultDefenseState : IState
{
    public DefaultDefenseState(Player player, StateMachine machine, string name, int maxTowerLines = 100)
    {
        Player = player;
        Machine = machine;
        Name = name;
        _maxTowerLines = maxTowerLines;
    }

    public string Name { get; }
    public Player Player { get; }
    public StateMachine Machine { get; }

    private int _turn = 0;
    private const int TowerFrontLine = 10;
    private int _maxTowerLines;

    public void OnEnter()
    {
        DebugLogger.Log(Name);
    }

    public void OnExit()
    {
        
    }

    //initial tactic: build up
    public void DeployTowers()
    {
        _turn++;

        //don't build towers if there are no soldiers attacking
        if (Player.HomeLane.GetSoldiers().Count == 0)
            return;
        
        List<(int row, int soldierAmount)> soldiersPerRow = new ();
        for (int i = 0; i < PlayerLane.WIDTH; i++)
        {
            soldiersPerRow.Add((i, Player.HomeLane.GetSoldiers().Count(soldier => soldier.PosX == i)));
        }
        
        soldiersPerRow = soldiersPerRow.OrderByDescending(x => x.soldierAmount).ToList();

        for (int i = 0; i < PlayerLane.WIDTH; i++)
        {
            int row = soldiersPerRow[i].row;
            int soldiers = soldiersPerRow[i].soldierAmount;

            int depth = soldiers + 1; //how many lines of towers we want to build. Always one more than soldiers on this row
            if (depth > _maxTowerLines) depth = _maxTowerLines;
            
            //don't build towers for empty lanes
            if (soldiers == 0)
                continue;

            row = row.RoundToEven();

            List<(int x, int y)> placements = new List<(int x, int y)>() { (0, 1), (-2, 1), (2, 1), (-1, 2), (1, 2), (0, 3), (-2, 3), (2, 3), (-1, 4), (1, 4), (0, 5), (-2, 5), (2, 5) };
            
            foreach ((int x, int y) offset in placements)
            {
                //don't place more towers than necessary
                if (offset.y > depth)
                    continue;
                
                int retreat = 0; //how many lines do we need to back up because the cell we want to build on is occupied by an enemy soldier

                while(true)
                {
                    int x = row + offset.x;
                    int y = TowerFrontLine + retreat + offset.y;
                
                    //if we retreated so far that we're out of bounds, we can't build a tower. The enemies are flooding us :(
                    if (y >= PlayerLane.HEIGHT)
                    {
                        break;
                    }
                    
                    Unit unitOnCell = Player.HomeLane.GetCellAt(x, y)?.Unit;
                    if (unitOnCell is Soldier)
                    {
                        retreat += 2;
                        continue;
                    }
                
                    Player.TowerPlacementResult result = Player.TryBuyTower<Tower>(x, y);
                    break;
                }
            }
        }
    }

    public void DeploySoldiers()
    {
        
    }

    public List<Soldier> SortedSoldierArray(List<Soldier> unsortedList)
    {
        return unsortedList;
    }

    public List<Tower> SortedTowerArray(List<Tower> unsortedList)
    {
        
        var furtherBackTowersFirst = unsortedList.OrderByDescending(tower => tower.PosY).ToList();
        string log = "";
        foreach (Tower tower in furtherBackTowersFirst)
        {
            log += tower.PosY;
            log += ", ";
        }
        //DebugLogger.Log(log);
        return furtherBackTowersFirst;
    }
}
}
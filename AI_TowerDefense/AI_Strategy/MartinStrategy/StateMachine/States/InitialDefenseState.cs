//(c) copyright by Martin M. Klöckener

using System;
using System.Collections.Generic;
using System.Linq;
using AI_Strategy;
using GameFramework;

namespace Wichtel.StateMachine.States {
public class InitialDefenseState : IState
{
    public InitialDefenseState(Player player, StateMachine machine)
    {
        Player = player;
        Machine = machine;
    }

    public Player Player { get; }
    public StateMachine Machine { get; }

    private int _turn = 0;

    public void OnEnter()
    {
        
    }

    public void OnExit()
    {
        
    }

    //initial tactic: build up
    public void DeployTowers()
    {
        _turn++;
        //we want to place initial towers where the most enemies arrive
        if (_turn < 14)
            return;

        int[] soldiersPerRow = new int[PlayerLane.WIDTH];
        for (int i = 0; i < PlayerLane.WIDTH; i++)
        {
            soldiersPerRow[i] = Player.HomeLane.Soldiers.Count(soldier => soldier.PosX == i);
        }

        for (int i = 0; i < soldiersPerRow.Length; i++)
        {
            int amount = soldiersPerRow[i];
            if (amount != soldiersPerRow.Max())
                continue;

            int j = i.Clamp(2, PlayerLane.WIDTH - 2);
            j = j.RoundToEven();
            
            var res1 = Player.TryBuyTower(j, PlayerLane.HEIGHT - 1, out Tower tower1);
            var res2 = Player.TryBuyTower(j - 2, PlayerLane.HEIGHT - 1, out Tower tower2);
            var res3 = Player.TryBuyTower(j + 2, PlayerLane.HEIGHT - 1, out Tower tower3);
            var res4 = Player.TryBuyTower(j - 1, PlayerLane.HEIGHT - 2, out Tower tower4);
            var res5 = Player.TryBuyTower(j + 1, PlayerLane.HEIGHT - 2, out Tower tower5);
            var res6 = Player.TryBuyTower(j, PlayerLane.HEIGHT - 3, out Tower tower6);
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
        DebugLogger.Log(log);
        return furtherBackTowersFirst;
    }
}
}
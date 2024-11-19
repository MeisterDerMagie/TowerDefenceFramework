using System;
using System.Collections.Generic;
using System.Linq;
using AI_Strategy;
using GameFramework;

namespace Wichtel.StateMachine.States {
public class InitialAttack : IState
{
    public InitialAttack(Player player, StateMachine machine, string name)
    {
        Player = player;
        Machine = machine;
        Name = name;
    }

    public string Name { get; }
    public Player Player { get; }
    public StateMachine Machine { get; }

    public bool LeaveState;

    public void OnEnter()
    {
        DebugLogger.Log(Name);

        //let all idle soldiers start walking
        var ownSoldiers = Player.EnemyLane.GetSoldiers();
        foreach (Soldier soldier in ownSoldiers)
        {
            if (soldier is MartinSoldier martinSoldier)
                martinSoldier.ShouldWalk = true;
        }
    }

    public void OnExit()
    {
        LeaveState = false;
    }

    //initial tactic: build up
    public void DeployTowers()
    {
        //not called in offense
    }

    public void DeploySoldiers()
    {
        LeaveState = true;
        MartinStrategy.LeftInitialOffense?.Invoke();
        return;
        
        //////
        
        if (Player.Gold >= 14)
        {
            for (int i = 0; i < PlayerLane.WIDTH - 0; i++)
            {
                Player.TryBuySoldier<Soldier>(i);
            }
        }

        LeaveState = true;
        MartinStrategy.LeftInitialOffense?.Invoke();
    }

    public List<Soldier> SortedSoldierArray(List<Soldier> unsortedList)
    {
        var foremostSoldiersFirst = unsortedList.OrderByDescending(soldier => soldier.PosY).ToList();
        return foremostSoldiersFirst;
    }

    public List<Tower> SortedTowerArray(List<Tower> unsortedList)
    {
        //not called in offense
        return unsortedList;
    }
}
}
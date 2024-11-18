//(c) copyright by Martin M. Klöckener

using System;
using System.Collections.Generic;
using System.Linq;
using AI_Strategy;
using GameFramework;

namespace Wichtel.StateMachine.States {
public class MassRecruitementState : IState
{
    public MassRecruitementState(Player player, StateMachine machine)
    {
        Player = player;
        Machine = machine;
    }

    public Player Player { get; }
    public StateMachine Machine { get; }

    public void OnEnter()
    {
        
    }

    public void OnExit()
    {
        
    }

    //initial tactic: build up
    public void DeployTowers()
    {
        //not called in offense
    }

    public void DeploySoldiers()
    {
        if(Player.Gold < 78)
            EnterSaveMoneyState();

        for (int i = 0; i < PlayerLane.WIDTH; i++)
        {
            if (Player.Gold < 78)
            {
                EnterSaveMoneyState();
                return;
            }
            
            Player.TryBuySoldier<Soldier>(i);
        }
    }

    private void EnterSaveMoneyState()
    {
        Machine.SetState(new SaveMoney(Player, Machine));
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
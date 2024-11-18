//(c) copyright by Martin M. Klöckener

using System;
using System.Collections.Generic;
using System.Linq;
using AI_Strategy;
using GameFramework;

namespace Wichtel.StateMachine.States {
public class SaveMoney : IState
{
    public SaveMoney(Player player, StateMachine machine)
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
        if (Player.Gold < 277)
            return;
        
        //only start placing soldiers as soon as we have enough gold for a mass recruitement
        Machine.SetState(new MassRecruitementState(Player, Machine));
    }

    public List<Soldier> SortedSoldierArray(List<Soldier> unsortedList)
    {
        return unsortedList;
    }

    public List<Tower> SortedTowerArray(List<Tower> unsortedList)
    {
        //not called in offense
        return unsortedList;
    }
}
}
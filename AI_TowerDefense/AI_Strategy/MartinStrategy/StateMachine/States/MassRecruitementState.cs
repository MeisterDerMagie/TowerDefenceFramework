﻿using System;
using System.Collections.Generic;
using System.Linq;
using AI_Strategy;
using GameFramework;

namespace Wichtel.StateMachine.States {
public class MassRecruitementState : IState
{
    public MassRecruitementState(Player player, StateMachine machine, int goldBuffer, string name)
    {
        Player = player;
        Machine = machine;
        _goldBuffer = goldBuffer;
        Name = name;
    }

    public string Name { get; }
    public Player Player { get; }
    public StateMachine Machine { get; }

    private readonly int _goldBuffer; //how much gold do we want to keep minimum as a backup
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
        if (Player.Gold < _goldBuffer)
        {
            EnterSaveMoneyState();
            return;
        }

        for (int i = 0; i < PlayerLane.WIDTH - 0 ; i++)
        {
            if (Player.Gold < _goldBuffer)
            {
                EnterSaveMoneyState();
                return;
            }
            
            Player.TryBuySoldier(i, out MartinSoldier soldier);
            if (soldier != null) soldier.ShouldWalk = true;
        }
    }

    private void EnterSaveMoneyState()
    {
        LeaveState = true;
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
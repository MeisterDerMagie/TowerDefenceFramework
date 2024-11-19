using System;
using System.Collections.Generic;
using GameFramework;
using Wichtel.StateMachine;

namespace AI_Strategy
{
internal class InitialDefense : IState
{
    private readonly IState _nextState;

    public InitialDefense(Player player, StateMachine machine, string name, IState nextState)
    {
        _nextState = nextState;
        Player = player;
        Machine = machine;
        Name = name;
    }

    public string Name { get; }
    public Player Player { get; }
    public StateMachine Machine { get; }

    private int _turn;
    
    public void OnEnter()
    {
        DebugLogger.Log(Name);
    }

    public void OnExit()
    {
        
    }

    public void DeployTowers()
    {
        _turn++;
        
        for (int i = 0; i < PlayerLane.WIDTH; i++)
        {
            Player.TryBuyTower<Tower>(i, PlayerLane.HEIGHT - 1);
            Player.TryBuyTower<Tower>(i + 1, PlayerLane.HEIGHT - 2);
        }
        
        if(_turn > 16)
            Machine.SetState(_nextState);
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
        return unsortedList;
    }
}
}
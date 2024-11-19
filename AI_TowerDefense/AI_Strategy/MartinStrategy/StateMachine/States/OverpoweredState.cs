//(c) copyright by Martin M. Klöckener

using System.Collections.Generic;
using GameFramework;

namespace Wichtel.StateMachine.States {
public class OverpoweredState : IState
{
    public string Name { get; }
    public Player Player { get; }
    public StateMachine Machine { get; }
    
    private int _turn = 0;
    private bool _buyNextTurn;
    
    public OverpoweredState(Player player, StateMachine machine, string name)
    {
        Player = player;
        Machine = machine;
        Name = name;
    }
    
    public void OnEnter()
    {
        
    }

    public void OnExit()
    {
        
    }

    public void DeployTowers()
    {
        _turn ++;
    }

    public void DeploySoldiers()
    {
        _turn ++;

        if (Player.Gold >= 28 || _buyNextTurn)
        {
            if (Player.Gold >= 28)
                _buyNextTurn = true;
            else
                _buyNextTurn = false;
            
            for (int i = 0; i < PlayerLane.WIDTH - 0; i++)
            {
                Player.TryBuySoldier<Soldier>(i);
            }
        }
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
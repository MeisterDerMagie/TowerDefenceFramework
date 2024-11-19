using System.Collections.Generic;
using GameFramework;

namespace Wichtel.StateMachine.States {
public class Idle : IState
{
    public string Name { get; }
    public Player Player { get; }
    public StateMachine Machine { get; }
    
    public Idle(Player player, StateMachine machine, string name)
    {
        Player = player;
        Machine = machine;
        Name = name;
    }
    
    public void OnEnter()
    {
        DebugLogger.Log(Name);
    }

    public void OnExit()
    {
        
    }

    public void DeployTowers()
    {
        
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
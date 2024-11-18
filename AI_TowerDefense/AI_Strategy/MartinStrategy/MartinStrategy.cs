using System.Collections.Generic;
using GameFramework;
using Wichtel.StateMachine;
using Wichtel.StateMachine.States;

namespace AI_Strategy {
public class MartinStrategy : AbstractStrategy
{
    private StateMachine _defense;
    private StateMachine _offense;
    
    public MartinStrategy(Player player) : base(player)
    {
        BuildDefense();
        BuildOffense();
    }

    public override void DeployTowers()
    {
        _defense.DeployTowers();
    }

    public override void DeploySoldiers()
    {
        _offense.DeploySoldiers();
    }
    
    /// <summary>
    /// Called by the game play environment. The order in which the array is returned here is
    /// the order in which soldiers will plan and perform their movement.
    /// </summary>
    public override List<Soldier> SortedSoldierArray(List<Soldier> unsortedList)
    {
        return _offense.SortedSoldierArray(unsortedList);
    }

    /// <summary>
    /// Called by the game play environment. The order in which the array is returned here is
    /// the order in which towers will plan and perform their action.
    /// </summary>
    public override List<Tower> SortedTowerArray(List<Tower> unsortedList)
    {
        DebugLogger.Log(unsortedList.Count);
        return _defense.SortedTowerArray(unsortedList);
    }

    private void BuildDefense()
    {
        _defense = new StateMachine();

        var initialState = new InitialDefenseState(player, _defense);
        
        //_stateMachine.AddAnyTransition();

        _defense.SetState(initialState);
    }
    
    private void BuildOffense()
    {
        _offense = new StateMachine();

        var initialState = new SaveMoney(player, _offense);
        
        //_stateMachine.AddAnyTransition();

        _offense.SetState(initialState);
    }
}
}
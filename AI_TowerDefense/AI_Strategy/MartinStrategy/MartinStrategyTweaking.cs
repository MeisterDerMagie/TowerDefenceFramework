using System.Collections.Generic;
using GameFramework;
using Wichtel.StateMachine;
using Wichtel.StateMachine.States;

namespace AI_Strategy {
public class MartinStrategyTweaking : AbstractStrategy
{
    private StateMachine _defense;
    private StateMachine _offense;

    private int _turn = 0;
    private bool _defensiveStrategyWorked = false;
    //private bool CurrentStrategyWorks => !(_turn is > 500 and < 940 && player.Score < 50);
    
    public MartinStrategyTweaking(Player player) : base(player)
    {
        BuildDefense();
        BuildOffense();
    }

    public override void DeployTowers()
    {
        _turn++;

        if (_turn == 500)
        {
            _defensiveStrategyWorked = player.Score > 50;
        }
        
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
        return _defense.SortedTowerArray(unsortedList);
    }

    private void BuildDefense()
    {
        _defense = new StateMachine();

        var initialState = new DefaultDefenseState(player, _defense, "Tweak D: default Defense", 3);
        var minimalDefense = new DefaultDefenseState(player, _defense,"Tweak D: minimal Defense", 2);
        var idleState = new Idle(player, _defense, "Tweak D: idle Defense");
        
        //apparently our defensive strategy can't outperform the opponent, switch to minimal defense mode instead
        _defense.AddAnyTransition(minimalDefense, () => _turn > 500 && !_defensiveStrategyWorked);
        //all or nothing mode
        _defense.AddAnyTransition(idleState, () => _turn > 940);

        _defense.SetState(initialState);
    }
    
    private void BuildOffense()
    {
        _offense = new StateMachine();

        var saveMoney = new SaveMoney(player, _offense, "Tweak O: save Money");
        var defensiveMassRecruitement = new MassRecruitementState(player, _offense, 148, "Tweak O: defensive Offensive");
        var offensiveMassRecruitement = new MassRecruitementState(player, _offense, 40, "Tweak O: Offensive");
        var allOrNothing = new MassRecruitementState(player, _offense, 0, "Tweak O: AllOrNothing");
        
        _offense.AddAnyTransition(allOrNothing, () => _turn > 940);
        _offense.AddTransition(saveMoney, offensiveMassRecruitement, () => player.Gold > 70 && !_defensiveStrategyWorked && _turn < 940);
        _offense.AddTransition(saveMoney, defensiveMassRecruitement, () => player.Gold > 367 && _defensiveStrategyWorked && _turn < 940);
        _offense.AddTransition(defensiveMassRecruitement, saveMoney, () => defensiveMassRecruitement.LeaveState && _turn < 940);
        _offense.AddTransition(offensiveMassRecruitement, saveMoney, () => offensiveMassRecruitement.LeaveState && _turn < 940);

        _offense.SetState(saveMoney);
    }
}
}
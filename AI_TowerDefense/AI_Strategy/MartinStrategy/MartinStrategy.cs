using System;
using System.Collections.Generic;
using GameFramework;
using Wichtel.StateMachine;
using Wichtel.StateMachine.States;

namespace AI_Strategy {
public class MartinStrategy : AbstractStrategy
{
    private StateMachine _defense;
    private StateMachine _offense;

    private int _turn = 0;
    private bool _defensiveStrategyWorked = false;
    public static Action LeftInitialOffense = delegate {  };
    
    public MartinStrategy(Player player) : base(player)
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

        var defaultDefense = new DefaultDefenseState(player, _defense, "Martin D: default Defense", 5);
        var minimalDefense = new DefaultDefenseState(player, _defense, "Martin D: minimal Defense", 2);
        var idleState = new Idle(player, _defense, "Martin D: Idle Defense");
        var initialDefense = new InitialDefense(player, _defense, "Martin D: initialDefense", defaultDefense);
        
        _defense.AddTransition(idleState, defaultDefense, ref LeftInitialOffense);
        _defense.AddAnyTransition(idleState, () => _turn > 940);
        _defense.AddAnyTransition(idleState, () => IsDangerousAttackIncoming() && _offense.CurrentState is not InitialAttack && _turn > 30);
        _defense.AddTransition(idleState, defaultDefense, () => !IsDangerousAttackIncoming());
        _defense.AddAnyTransition(minimalDefense, () => _turn > 500 && !_defensiveStrategyWorked);

        _defense.SetState(idleState);
        //_defense.SetState(idleState);
    }
    
    private void BuildOffense()
    {
        _offense = new StateMachine();

        var initialAttack = new InitialAttack(player, _offense, "Martin O: Initial Attack");
        var saveMoney = new SaveMoney(player, _offense, "Martin O: Save Money");
        var offensiveMassRecruitement = new MassRecruitementState(player, _offense, 40, "Martin O: Offensive");
        var counterAttack = new MassRecruitementState(player, _offense, 0, "Martin O: Counter Attack");
        var allOrNothing = new MassRecruitementState(player, _offense, 0, "Martin O: AllOrNothing");
        
        _offense.AddTransition(initialAttack, saveMoney, () => initialAttack.LeaveState);
        _offense.AddAnyTransition(allOrNothing, () => _turn > 940);
        _offense.AddAnyTransition(counterAttack, () => IsDangerousAttackIncoming() && player.Gold >= 28);
        _offense.AddTransition(counterAttack, saveMoney, () => player.Gold < 3);
        _offense.AddTransition(saveMoney, offensiveMassRecruitement, () => player.Gold > 90 && _turn < 940); //160
        _offense.AddTransition(offensiveMassRecruitement, saveMoney, () => offensiveMassRecruitement.LeaveState && _turn < 940);

        _offense.SetState(initialAttack);
    }

    private bool IsDangerousAttackIncoming()
    {
        int linesWithThreeOrMoreSoldiers = 0;
        var soldiers = new List<Soldier>();
        
        for (int h = 0; h < PlayerLane.HEIGHT; h++)
        {
            for (int w = 0; w < PlayerLane.WIDTH; w++)
            {
                Cell cell = player.HomeLane.GetCellAt(w, h);
                if(cell.Unit is Soldier soldier) soldiers.Add(soldier);
            }

            if (soldiers.Count >= 3) linesWithThreeOrMoreSoldiers++;
            soldiers.Clear();
        }

        return linesWithThreeOrMoreSoldiers >= 2;
    }
}
}
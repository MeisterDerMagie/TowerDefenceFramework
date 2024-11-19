using System;
using System.Collections.Generic;
using AI_Strategy;
using GameFramework;

namespace Wichtel.StateMachine.States {
public class SaveMoney : IState
{
    public SaveMoney(Player player, StateMachine machine, string name)
    {
        Player = player;
        Machine = machine;
        Name = name;
    }

    public string Name { get; }
    public Player Player { get; }
    public StateMachine Machine { get; }

    
    private int GoldTendency
    {
        get
        {
            int tendency1 = _goldHistory[9] - _goldHistory[0];
            int tendency2 = _goldHistory[10] - _goldHistory[1];
            int tendency3 = _goldHistory[11] - _goldHistory[2];

            return (tendency1 + tendency2 + tendency3) / 3;
        }
    }

    private List<int> _goldHistory = new() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

    public void OnEnter()
    {
        DebugLogger.Log(Name);
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
        UpdateGoldHistory();
        
        //potentially cheese player with idle soldiers
        if (Player.Gold < 50) return;
        
        Player.TryBuySoldier<MartinSoldier>(0);
        Player.TryBuySoldier<MartinSoldier>(PlayerLane.WIDTH - 1);
    }

    private void UpdateGoldHistory()
    {
        _goldHistory.Add(Player.Gold);
        if (_goldHistory.Count > 13) _goldHistory.RemoveAt(0);
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
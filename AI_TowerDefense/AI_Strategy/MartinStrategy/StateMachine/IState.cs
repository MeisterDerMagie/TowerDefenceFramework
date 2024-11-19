//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using GameFramework;

namespace Wichtel.StateMachine {
public interface IState
{
    public string Name { get; }
    public Player Player { get; }
    public StateMachine Machine { get; }
    public void OnEnter();
    public void OnExit();
    public void DeployTowers();
    public void DeploySoldiers();
    public List<Soldier> SortedSoldierArray(List<Soldier> unsortedList);
    public List<Tower> SortedTowerArray(List<Tower> unsortedList);
}
}
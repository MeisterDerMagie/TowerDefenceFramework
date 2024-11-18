using System;
using System.Collections.Generic;
using GameFramework;

namespace Wichtel.StateMachine {
public class StateMachine
{
    public IState CurrentState { get; private set; }
    public string CurrentStateName => CurrentState == null ? "NONE" : CurrentState.GetType().Name;
   
    private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type,List<Transition>>();
    private List<Transition> _currentTransitions = new List<Transition>();
    private readonly List<Transition> _anyTransitions = new List<Transition>();
   
    private static readonly List<Transition> EmptyTransitions = new List<Transition>(0);

    private bool _isAlive;
    
    public StateMachine()
    {
        _isAlive = true;
    }

    private void EvaluateTransitions()
    {
        var transition = GetTransition();
        if (transition != null) SetState(transition.To);
    }

    public void DeployTowers()
    {
        EvaluateTransitions();
        CurrentState.DeployTowers();
    }

    public void DeploySoldiers()
    {
        EvaluateTransitions();
        CurrentState.DeploySoldiers();
    }
    
    public List<Soldier> SortedSoldierArray(List<Soldier> unsortedList)
    {
        EvaluateTransitions();
        return CurrentState.SortedSoldierArray(unsortedList);
    }
    
    public List<Tower> SortedTowerArray(List<Tower> unsortedList)
    {
        EvaluateTransitions();
        return CurrentState.SortedTowerArray(unsortedList);
    }

    public void Kill()
    {
        //exit current state
        SetState(null);
        
        //then set isAlive to false
        _isAlive = false;
    }

    public void SetState(IState state)
    {
        if (!_isAlive)
        {
            throw new InvalidProgramException("This state machine has been killed. Can't set state.");
        }
        
        if (state == CurrentState)
            return;
      
        CurrentState?.OnExit();
        CurrentState = state;
        
        //if state was set to null, do nothing else (this happens when the state machine is killed)
        if (state == null)
            return;
      
        _transitions.TryGetValue(CurrentState.GetType(), out _currentTransitions);
        if (_currentTransitions == null)
            _currentTransitions = EmptyTransitions;
      
        CurrentState.OnEnter();
    }

    public void AddTransition(IState @from, IState to, Func<bool> predicate)
    {
        if (!_isAlive)
        {
            throw new InvalidProgramException("This state machine has been killed. Can't add transition.");
        }
        
        if (_transitions.TryGetValue(@from.GetType(), out var transitions) == false)
        {
            transitions = new List<Transition>();
            _transitions[@from.GetType()] = transitions;
        }
      
        transitions.Add(new Transition(to, predicate));
    }
    
    public void AddTransition(IState @from, IState to, ref Action trigger)
    {
        if (!_isAlive)
        {
            throw new InvalidProgramException("This state machine has been killed. Can't add transition.");
        }
        
        if (_transitions.TryGetValue(@from.GetType(), out var transitions) == false)
        {
            transitions = new List<Transition>();
            _transitions[@from.GetType()] = transitions;
        }
        
        transitions.Add(new Transition(to, ref trigger));
    }

    public void AddAnyTransition(IState state, Func<bool> predicate)
    {
        if (!_isAlive)
        {
            throw new InvalidProgramException("This state machine has been killed. Can't add transition.");
            return;
        }
        
        _anyTransitions.Add(new Transition(state, predicate));
    }
    
    public void AddAnyTransition(IState state, ref Action trigger)
    {
        if (!_isAlive)
        {
            throw new InvalidProgramException("This state machine has been killed. Can't add transition.");
        }
        
        _anyTransitions.Add(new Transition(state, ref trigger));
    }

    private class Transition
    {
        public Func<bool> Condition {get; }
        public IState To { get; }

        private bool Triggered() => _triggered;
        private bool _triggered = false;
        public void ResetTrigger() => _triggered = false;
        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }
        
        public Transition(IState to, ref Action trigger)
        {
            To = to;
            trigger += OnTrigger;
            Condition = Triggered;
        }

        private void OnTrigger()
        {
            _triggered = true;
        }
    }

    private Transition GetTransition()
    {
        foreach (var transition in _anyTransitions)
        {
            if (transition.Condition())
            {
                transition.ResetTrigger();
                return transition;
            }
        }
        
        foreach (var transition in _currentTransitions)
        {
            if (transition.Condition())
            {
                transition.ResetTrigger();
                return transition;
            }
        }

        return null;
    }
}
}
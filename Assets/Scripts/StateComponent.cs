using System;
using System.Collections;
using System.Linq;
using Game.Data;
using UnityEngine;

public class StateComponent : StateListenerBase 
{
    #region SERIALIZE FIELDS

    [SerializeField, Tooltip("Array of states to view")]
    private GameState[] _states;

    #endregion

    #region implemented abstract members of StateListenerBase

    protected override void OnStateChangedHandler(GameState arg1, GameState arg2)
    {
        if (_states == null || _states.Length == 0) throw new Exception("States not set");

        var states = from i in _states where (i == arg1) select i;

        bool condition = states == null || states.Count() == 0;

        gameObject.SetActive(!condition);
    }

    #endregion
}

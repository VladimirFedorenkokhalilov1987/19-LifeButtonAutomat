using System;
using System.Collections;
using Game.Data;
using UnityEngine;

public abstract class StateListenerBase : MonoBehaviour 
{
    #region UNITY EVENTS
    protected virtual void Awake()
    {
        if(StateManager.Instance)
			StateManager.Instance.OnStateChanged += OnStateChangedHandler;
        else
			throw new NullReferenceException("StateManager.Instance is null.");
    }

    protected virtual void OnDestroy()
    {
        if(StateManager.Instance)
			StateManager.Instance.OnStateChanged -= OnStateChangedHandler;
    }
    #endregion

    #region PROTECTED ABSTRACT METHOD
    protected abstract void OnStateChangedHandler(GameState arg1, GameState arg2);
    #endregion
}

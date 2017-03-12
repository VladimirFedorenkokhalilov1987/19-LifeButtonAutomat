using System;
using System.Collections;
using Game.Data;
using UnityEngine;



public class StateManager : MonoBehaviour 
{
    #region SINGLETONE SECTION

    private static StateManager _instance;

    public static StateManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)  _instance = this;
        else DestroyImmediate(gameObject);
    }

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField]
    private GameState _currentState;

    [SerializeField]
    private GameState _previousState;

    #endregion

    public GameState CurrentState
    {
        get
        {
            return _currentState;
        }
    }

    #region EVENTS AND HANDLERS

    public event Action<GameState,GameState> OnStateChanged;

    private void OnStateChangedHandler(GameState current, GameState previous)
    {
        if (OnStateChanged != null)  OnStateChanged(current, previous);
    }

    #endregion

    #region PUBLIC METHODS

    public void SetState(GameState newState)
    {
        _previousState = _currentState;
        _currentState = newState;
        OnStateChangedHandler(_currentState, _previousState);
    }

    #endregion

//    private void OnGUI()
//    {
//        if(GUI.Button(new Rect(10,10,100,50), "Play")) SetState(GameState.Play);
//        if(GUI.Button(new Rect(10,60,100,50), "Pause")) SetState(GameState.Pause);
//        if(GUI.Button(new Rect(10,110,100,50), "Resume")) SetState(GameState.Resume);
//        if(GUI.Button(new Rect(10,160,100,50), "GameOver")) SetState(GameState.GameOver);
//    }
}

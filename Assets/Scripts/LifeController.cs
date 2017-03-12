using UnityEngine;
using System.Collections;
using Game.Data;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Game.Extentions;

public class LifeController : MonoBehaviour 
{
    #region SERIALIZE FIELDS
    [SerializeField]
    private Item _itemPrefab;

    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup;

    [SerializeField]
    private Text _generationIndexText;

	[SerializeField]
	private Text _maxGenerationIndexText;

    [SerializeField]
    private Toggle _autoMoveToggle;

    [SerializeField]
    private Slider _generationSlider;
    #endregion

    #region PRIVATE FIELDS
    private List<Item> _field = new List<Item>();
    private bool[,] _logicalField;
    private bool[,] _logicalFieldGeneration;
    private int _row;
    private int _column;
    private int _fullCount;
    private int _generationIndex;
    private List<bool[,]> _generations = new List<bool[,]>();
    #endregion

    #region UNITY EVENTS
    private void Start()
    {
		_autoMoveToggle.gameObject.SetActive (false);

        if(_generationSlider)
			_generationSlider.gameObject.SetActive(false);
		
        SetState(GameState.Select);
        GenerateField();
    }
    #endregion

    #region PRIVATE METHODS
    private void GenerateField()
    {
        if (!_gridLayoutGroup) 
			return;
		
        while (_fullCount < GameConfig.ItemCount)
        {
            _gridLayoutGroup.CalculateLayoutInputHorizontal();
            _gridLayoutGroup.CalculateLayoutInputVertical();
            _gridLayoutGroup.SetLayoutHorizontal();
            _gridLayoutGroup.SetLayoutVertical();

            _row = (int)(_gridLayoutGroup.preferredHeight / _gridLayoutGroup.cellSize.y);
            float value = GameConfig.ItemCount / (float)_row;
            bool condition = (value - (int)value) > 0;
            _column = condition ? ((int)value) + 1 : (int)value; 
            _fullCount = _row * _column;

            for (int i = 0; i < Mathf.Abs(_fullCount - GameConfig.ItemCount); i++)
            {
                var temp = _itemPrefab.GetInstance();
                _field.Add(temp);
                temp.transform.SetParent(_gridLayoutGroup.transform, false);
                temp.gameObject.name = i.ToString();
            }
        }
        Debug.LogFormat("<color=olive><size=15><b>row:{0}, column:{1}</b></size></color>", _row, _column);
    }

    private bool[,] GetNextGeneration(bool[,] field)
    {
        if(field == null || field.Length == 0) 
			return null;

        var temp = new bool[field.GetLength(0),field.GetLength(1)];

        for (int i = 0; i < _row; i++)
        {
            for (int j = 0; j < _column; j++) 
            {
                temp[i,j] = CheckCell(field, i, j);
            }
        }
        return temp;
    }

    private bool CheckCell(bool[,] field, int row, int column)
    {
        if(field == null || field.Length == 0)
			return false;

        int count = 0;

        for (int i = row - 1; i <= row + 1; i++)
        {
            for (int j = column - 1; j <= column + 1; j++) 
            {
                if(i == row && j == column) 
					continue;

                int _row = i;
                int _column = j;

                if(i < 0) 
					_row = field.GetLength(0) - 1;
                if(j < 0)
					_column = field.GetLength(1) - 1;
                if(i >= field.GetLength(0))
					_row = 0;
                if(j >= field.GetLength(1))
					_column = 0;

                count += field[_row,_column] ? 1 : 0;
            }
        }

        bool condition = !field[row,column] && count == 3;
        bool condition1 = field[row,column] && (count == 2 || count == 3);

        return condition || condition1;
    }

    private bool[,] GetLogicalField()
    {
        var temp = new bool[_row,_column];

        for (int i = 0; i < _row; i++)
        {
            for (int j = 0; j < _column; j++) 
            {
                int index = i * _column + j;

                if(_field[index])
					temp[i,j] = _field[index].IsSelected; 
            }
        }
        return temp;
    }

    private void SetLogicalField(bool[,] field)
    {
        if(field == null || field.Length == 0) 
			return;

        for (int i = 0; i < field.GetLength(0); i++)
        {
            for (int j = 0; j < field.GetLength(1); j++) 
            {
                int index = i * _column + j;

                if(_field[index])
					_field[index].IsSelected = field[i,j];
            }
        } 
    } 

    private void SetState(GameState state)
    {
        if(StateManager.Instance)
			StateManager.Instance.SetState(state);
    }

    private bool IsFieldAllreadyAdded(bool[,] array, out int index)
    {
        index = 0;

        foreach (var item in _generations)
        {
            if(item == null) 
				continue;
			
            if(CompareArrays(item, array)) 
            {
                return true;
            }

            index++;
        }
        return false;
    }

    private bool CompareArrays(bool[,] a, bool[,] b)
    {
        if(a == null || b == null)
			return false;

        for (int i = 0; i < a.GetLength(0); i++)
        {
            for (int j = 0; j < a.GetLength(1); j++)
            {
                if(a[i,j] != b[i,j])
					return false;
            }   
        }
        return true;
    }

    private void StopAutoMove()
    {
        if(_autoMoveToggle && _autoMoveToggle.isOn)
			_autoMoveToggle.isOn = false;
		
        OnAutoMoveValueChanged();
		_maxGenerationIndexText.gameObject.SetActive (true);
    }

    private void SetGenerationSlider()
    {
        if(!_generationSlider) 
			return;

        _generationSlider.gameObject.SetActive(true);
        _generationSlider.maxValue = _generations.Count - 1;
        _generationSlider.value = _generationIndex;
    }
    #endregion

    #region PUBLIC METHODS
    public void StartLife()
    {
		_autoMoveToggle.gameObject.SetActive (true);
		_maxGenerationIndexText.gameObject.SetActive (false);
        SetState(GameState.Life);
        _logicalField = GetLogicalField();
        _logicalFieldGeneration = _logicalField;
        _generations.Add(_logicalField);

        if(_generationSlider) 
			_generationSlider.gameObject.SetActive(false);
    }

    public void MoveForward()
    {
        if(_logicalFieldGeneration.IsAllFalse()) 
        {
            StopAutoMove();
            SetGenerationSlider();
            return;
        }

        _generationIndex++;

        if(_generationIndex >= _generations.Count)
			_logicalFieldGeneration = GetNextGeneration(_logicalFieldGeneration);
        else
			_logicalFieldGeneration = _generations[_generationIndex];

        int index;

        if(IsFieldAllreadyAdded(_logicalFieldGeneration, out index))
        {
            _generationIndex = index;
            _logicalFieldGeneration = _generations[_generationIndex];
            StopAutoMove();
            SetGenerationSlider();
        }
        else
        {
            _generations.Add(_logicalFieldGeneration);
        } 

		if (_generationIndexText && _maxGenerationIndexText) 
		{
			_generationIndexText.text = _generationIndex.ToString ();
			_maxGenerationIndexText.text = _generationSlider.maxValue.ToString ();
		}
		
        if(_generationSlider) 
			_generationSlider.value = _generationIndex;
		
		SetLogicalField(_logicalFieldGeneration);
    }

    public void MoveBackward()
    {
        _generationIndex--;
        _generationIndex = Mathf.Max(0, _generationIndex);

        if (_generationIndexText)
			_generationIndexText.text = _generationIndex.ToString();
		
        if(_generationSlider) 
			_generationSlider.value = _generationIndex;

        _logicalFieldGeneration = _generations[_generationIndex];
        SetLogicalField(_logicalFieldGeneration);
    }

    public void OnAutoMoveValueChanged()
    {
        if(!_autoMoveToggle) 
			return;
		
        CancelInvoke();

        if(_autoMoveToggle.isOn)
			InvokeRepeating("MoveForward", 0, GameConfig.MoveDelay);
    }

    public void OnRebootButtonClick()
    {
		_autoMoveToggle.gameObject.SetActive (false);

        SetState(GameState.Select);
        _generationIndex = 0;
        
		if (_generationIndexText) 
			_generationIndexText.text = _generationIndex.ToString();
		
        _generations.Clear();
        CancelInvoke();
        
		if(_generationSlider)
			_generationSlider.gameObject.SetActive(false);
    }

    public void OnClearAll()
    {
        SetLogicalField(new bool[_row,_column]);
    }

    public void OnGenerationSliderValueChanged()
    {
        if(!_generationSlider) 
			return;

        _generationIndex = (int)_generationSlider.value;
        _logicalFieldGeneration = _generations[_generationIndex];

        if (_generationIndexText)
			_generationIndexText.text = _generationIndex.ToString();
		
        SetLogicalField(_logicalFieldGeneration);
    }
    #endregion
}

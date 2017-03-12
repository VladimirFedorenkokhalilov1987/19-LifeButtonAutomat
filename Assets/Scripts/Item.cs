using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Item : MonoBehaviour 
{
    [SerializeField]
    private Color _selectedColor;

    private Color _unselectedColor;

    private Image _imageVar;
    private Image _image
    {
        get
        {
            if(!_imageVar)
            {
                _imageVar = GetComponent<Image>();
                _unselectedColor = _imageVar.color;
            }

            return _imageVar;
        }
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get
        {
            return _isSelected;
        }

        set
        {
            _isSelected = value;
            if(_image)
				_image.color = (value ? _selectedColor : _unselectedColor);
        }
    }

    public void OnMouseDown()
    {
        if(!StateManager.Instance || StateManager.Instance.CurrentState != Game.Data.GameState.Select)
			return;
		
        IsSelected = !IsSelected;
    }
}

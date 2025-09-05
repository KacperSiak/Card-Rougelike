using System.Collections.Generic;
using UnityEngine;

//mb separate saving properties and card logic
public class PlayerCardLogic : MonoBehaviour
{
    [SerializeField] private List<Vector3> _hoverVectors;
    [SerializeField] private List<Vector3> _pressedVectors;

    private Vector3 _hoverOffset = new();
    private Vector3 _pressedOffset = new();

    private Vector3 _defaultPosition;
    private Quaternion _defaultRotation;
    private Vector3 _defaultSize;
    private bool _isSelected = false;
    private bool _isHovered = false;
    private bool _isAvailable = false;

    public bool IsUsed { get; set; } = false;
    public bool IsNew { get; private set; } = true;

    public CardsSOTest Card { get; private set; }

    public void RestoreSelectedPosition()
    { 
        if(_isSelected)
            transform.position += _pressedOffset;
        else if(_isHovered)
            transform.position += _hoverOffset;
    }

    //? 
    private void OnViewChanged()
    { 
        _hoverOffset = CamMovementManager.Instance.IsFirstSet ? _hoverVectors[0] : _hoverVectors[1];
        Debug.Log(_hoverOffset + "camera changed" + CamMovementManager.Instance.IsFirstSet);
        _pressedOffset = CamMovementManager.Instance.IsFirstSet ? _pressedVectors[0] : _pressedVectors[1];
        Debug.Log(_pressedOffset + "camera changed" + CamMovementManager.Instance.IsFirstSet);
    }

    public void SaveDefaultProperties()
    {
        _defaultPosition = transform.position;
        _defaultSize = transform.localScale;
        _defaultRotation = transform.rotation;

        if (IsNew)
            IsNew = false;
    }

    public void SetCardProperties(CardsSOTest card)
    {
        Card = card;
        _isAvailable = true;
        OnViewChanged();
        RoundManager.OnRoundEnd += MakeReusable;
        PlayerCardManager.OnViewChanged += OnViewChanged;
    }

    public void SetAnimationPlacement(Vector3 newPosition, Quaternion newRotation)
    {
        transform.position += newPosition;
        transform.rotation = newRotation;
    }

    public void ResetLastPlacement(Vector3 newPosition)
    {
        transform.position -= newPosition;
        transform.rotation = _defaultRotation;
    }

    private void OnMouseEnter()
    {
        if (_isSelected || !_isAvailable || DiscardManager.Instance.InUI) return;
        transform.position += _hoverOffset;
        Debug.Log(_hoverOffset);
        _isHovered = true;
    }

    private void OnMouseExit()
    {
        if (_isSelected || !_isAvailable) return;

        if (_isHovered)
        {
            _isHovered = false;
            transform.position -= _hoverOffset;
        } 

    }
    private void OnMouseDown()
    {
        if (DiscardManager.Instance.InUI) return;

        if (!_isSelected && _isAvailable)
        {
            if (PlayerCardManager.Instance.SelectedCards.Count == 3)
                return; // Prevent selecting more than 3 cards

            transform.position += _pressedOffset;
            Debug.Log(_pressedOffset);
            PlayerCardManager.Instance.AddSelectedCards(gameObject);
            _isSelected = true;

            if(!CamMovementManager.Instance.IsFirstSet)
            CamMovementManager.Instance.ChangeCameraPlacementOnSelected();
        }

        else if(!IsUsed && _isAvailable)
        {
            transform.position -= _pressedOffset;
            PlayerCardManager.Instance.RemoveSelectedCards(gameObject);
            _isSelected = false;
        }

    }

    //probably i won't need that itf
    public void MakeReusable()
    {
        _isSelected = false;
        IsUsed = false;
        IsNew = true;
    }

    public void ResetCard()
    {
        transform.position = Vector3.zero;
        transform.localScale = _defaultSize;
        transform.rotation = Quaternion.identity;
        Card = null; 
        _isAvailable = false;
        MakeReusable();
        RoundManager.OnRoundEnd -= MakeReusable;
        PlayerCardManager.OnViewChanged -= OnViewChanged;
    }
}

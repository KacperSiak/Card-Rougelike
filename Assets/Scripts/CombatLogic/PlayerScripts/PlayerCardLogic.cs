using UnityEngine;

//mb separate saving properties and card logic
public class PlayerCardLogic : MonoBehaviour
{
    private Vector3 _hoverOffset = new (0f, 0.0125f, -0.00088f);
    private Vector3 _pressedOffset = new (0f, 0.015f, 0f);

    private Vector3 _defaultPosition;
    private Vector3 _lastPosition;
    private Quaternion _defaultRotation;
    private Vector3 _defaultSize;
    private bool _isSelected = false;

    public bool IsUsed { get; set; } = false;
    private bool _isAvailable = false;

    public CardsSOTest Card { get; private set; }

    private void OnEnable() 
    {
        RoundManager.OnRoundEnd += MakeReusable;
    }


    private void OnDisable()
    { 
        RoundManager.OnRoundEnd -= MakeReusable;
    }

    public void SaveDefaultProperties()
    {
        _defaultPosition = transform.position;
        _defaultSize = transform.localScale;
        _defaultRotation = transform.rotation;
        SaveCurrentProperties();
    }

    public void IncreaseDefaultProperties(Vector3 v)
    { 
        _defaultPosition += v;
    }

    public void DecreaseDefaultProperties(Vector3 v)
    {
        _defaultPosition -= v;
    }

    public void SaveCurrentProperties()
    {
        _lastPosition = transform.localPosition;
        _defaultRotation = transform.localRotation;
    }

    public void SetCardProperties(CardsSOTest card)
    {
        Card = card;
        _isAvailable = true;
    }

    public void SetAnimationPlacement(Vector3 newPosition, Quaternion newRotation)
    {
        transform.SetLocalPositionAndRotation(newPosition, newRotation);
    }

    public void ResetLastPlacement()
    {
        transform.SetLocalPositionAndRotation(_defaultPosition, _defaultRotation);
    }

    private void OnMouseEnter()
    {
        if (_isSelected || !_isAvailable) return;
        transform.position += _hoverOffset;
        IncreaseDefaultProperties(_hoverOffset);
        SaveCurrentProperties();
    }

    private void OnMouseExit()
    {
        if (_isSelected || !_isAvailable) return; 
        transform.position = _defaultPosition; // Reset position when not hovered
        SaveCurrentProperties();
    }

    private void OnMouseDown()
    {
        if (!_isSelected && _isAvailable)
        {
            if (PlayerCardManager.Instance.SelectedCards.Count == 3)
                return; // Prevent selecting more than 3 cards

            transform.position = _defaultPosition + new Vector3(0f, 0.015f, 0f);
            SaveCurrentProperties();
            PlayerCardManager.Instance.AddSelectedCards(gameObject);
            _isSelected = true;
        }

        else if(!IsUsed && _isAvailable)
        {
            transform.position = _defaultPosition;
            SaveCurrentProperties();
            PlayerCardManager.Instance.RemoveSelectedCards(gameObject);
            _isSelected = false;
        }

    }
    public void MakeReusable()
    {
        _isSelected = false;
        IsUsed = false;
    }

    public void ResetCard()
    {
        transform.position = Vector3.zero;
        transform.localScale = _defaultSize;
        transform.localRotation = Quaternion.identity;
        Card = null; 
        _isAvailable = false;
        MakeReusable();
        SaveCurrentProperties();
    }
}

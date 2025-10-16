using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicUIDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _iconPrefab;
    [SerializeField] private GameObject _fullIconPrefab;

    [SerializeField] private RectTransform _iconDisplay;
    [SerializeField] private RectTransform _fullIconDisplay;

    [SerializeField] private Button _showFullDisplay;
    [SerializeField] private Button _hideFullDisplay;

    [SerializeField] private Image _display;
    [SerializeField] private Image _fullDisplay;

    private void Start()
    {
        RefreshDisplay();
    }

    private void OnEnable()
    {
        _showFullDisplay.onClick.AddListener(ShowFullDisplay);
        _hideFullDisplay.onClick.AddListener(HideFullDisplay);
        _fullDisplay.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _showFullDisplay.onClick.RemoveAllListeners();
        _hideFullDisplay.onClick.RemoveAllListeners();
    }
    private void RefreshDisplay()
    {
        for (int i = _iconDisplay.childCount -1; i >= 0; i--)
            Destroy(_iconDisplay.GetChild(i).gameObject );

        foreach(var relic in ConsumablesAndRelicMenager.Instance.currentRelics)
            AddIcon(relic);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_iconDisplay);
    }

    public void AddIcon(RelicData relic)
    {
        var icon = Instantiate(_iconPrefab, _iconDisplay);

        var rt = icon.transform as RectTransform;
        rt.localScale = Vector3.one;

        var img = icon.GetComponent<Image>();
        if (img != null) img.sprite = relic.icon;
        else Debug.LogWarning("There is no image on icon prefab");

        var iconUI = icon.GetComponent<RelicIconButtonsUI>();
        if (iconUI != null)
        {
            iconUI.Init(relic);

            var btn = icon.GetComponent<Button>();
            if (btn) btn.onClick.AddListener(() => iconUI.ToggleRemoveButton());
        }
    }
    private void ShowFullDisplay()
    {
        _display.gameObject.SetActive(false);
        _fullDisplay.gameObject.SetActive(true);
        foreach(var relic in ConsumablesAndRelicMenager.Instance.currentRelics)
        {
            var fullIcon = Instantiate(_fullIconPrefab, _fullIconDisplay);

            var img = fullIcon.GetComponentInChildren<Image>();
            var text = fullIcon.GetComponentInChildren<TextMeshProUGUI>();
            if (img != null) img.sprite = relic.icon;
            else Debug.LogWarning("There is no image on icon prefab");

            if (text != null) text.text = relic.description;
            else Debug.LogWarning("There is no description for this relic");
        }
    }

    private void HideFullDisplay()
    {
        _display.gameObject.SetActive(true);
        _fullDisplay.gameObject.SetActive(false);

        for (int i = _fullIconDisplay.childCount - 1; i >= 0; i--)
            Destroy(_fullIconDisplay.GetChild(i).gameObject);
    }
}

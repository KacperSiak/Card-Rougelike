using UnityEngine;
using UnityEngine.UI;

public class RelicUIDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _iconPrefab;
    [SerializeField] private Transform _iconDisplay;

    [SerializeField] private Button _showFullDisplay;
    private void Start()
    {
        RefreshDisplay();
    }

    private void OnEnable()
    {
        _showFullDisplay.onClick.AddListener(ShowFullDisplay);
    }

    private void OnDisable()
    {
        _showFullDisplay.onClick.RemoveAllListeners();
    }
    private void RefreshDisplay()
    {
        foreach (var relic in ConsumablesAndRelicMenager.Instance.currentRelics)
        {
            AddIcon(relic);
        }
    }

    public void AddIcon(RelicData relic)
    {
        GameObject icon = Instantiate(_iconPrefab, _iconDisplay);
        Image img = icon.GetComponent<Image>();
        //warnings
        if (img != null)
            img.sprite = relic.icon;

        RelicIconButtonsUI iconUI = icon.GetComponent<RelicIconButtonsUI>();
        if (iconUI != null)
        {
            iconUI.Init(relic);

            icon.GetComponent<Button>().onClick.AddListener(()=>
            {
                iconUI.ToggleRemoveButton();
            });
        }
    }

    private void ShowFullDisplay()
    {
        return;
    }
}

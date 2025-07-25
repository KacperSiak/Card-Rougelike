using UnityEngine;
using UnityEngine.UI;

public class RelicUIDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _iconPrefab;
    [SerializeField] private Transform _iconDisplay;

    private void Start()
    {
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        foreach (var relic in ConsumablesMenager.instance.currentRelics)
        {
            AddIcon(relic);
        }
    }

    public void AddIcon(RelicData relic)
    {
        GameObject icon = Instantiate(_iconPrefab, _iconDisplay);
        Image img = icon.GetComponent<Image>();
        if (img != null)
            img.sprite = relic.icon;

        TooltipTrigger trigger = icon.GetComponent<TooltipTrigger>();
        if (trigger != null)
            trigger.relicData = relic;

        IconButtonUI iconUI = icon.GetComponent<IconButtonUI>();
        if (iconUI != null)
        {
            iconUI.Init(relic);


            icon.GetComponent<Button>().onClick.AddListener(()=>
            {
                iconUI.ShowRemoveButton(true);
            });
        }
    }
}

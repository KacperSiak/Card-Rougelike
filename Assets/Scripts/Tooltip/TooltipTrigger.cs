using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RelicData relicData;
    public ConsumablesData consumable;

    public void OnPointerEnter(PointerEventData eventData)
    {
        string name = "";
        string desc = "";

        if (relicData != null)
        {
            name = relicData.name;
            desc = relicData.description;
        }
        else if (consumable != null)
        {
            name = consumable.name;
            desc = consumable.cardDescription;
        }

        ToolTipManager.Instance.ShowToolTip(name, desc, Input.mousePosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipManager.Instance.HideToolTip();
    }
}

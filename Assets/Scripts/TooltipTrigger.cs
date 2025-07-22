using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RelicData relicData;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTipManager.Instance.ShowToolTip(relicData.description, Input.mousePosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipManager.Instance.HideToolTip();
    }
}

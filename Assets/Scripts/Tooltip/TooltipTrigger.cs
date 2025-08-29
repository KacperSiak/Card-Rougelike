using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string _description;

    public void Setup(string description)
    {
        _description = description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!string.IsNullOrEmpty(_description)) ToolTipControler.Instance.Show(_description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipControler.Instance.Hide();
    }
}

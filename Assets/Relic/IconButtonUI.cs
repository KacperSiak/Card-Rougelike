using UnityEngine;
using UnityEngine.UI;

public class IconButtonUI : MonoBehaviour
{
    public RelicData relic;
    public Button removeButton;
    public void Init(RelicData relic)
    {
        this.relic = relic;
        removeButton.gameObject.SetActive(false);
        removeButton.onClick.AddListener(RemoveRelic);
    }

    public void ShowRemoveButton(bool show)
    {
        removeButton.gameObject.SetActive(show);
    }

    public void RemoveRelic()
    {
        ToolTipManager.Instance.HideToolTip();
        RelicEventsSystem.RelicRemoved(relic);
        Destroy(gameObject);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class IconButtonUI : MonoBehaviour
{
    [SerializeField] private RelicData _relic;
    [SerializeField] private Button _removeButton;
    public void Init(RelicData relic)
    {
        _relic = relic;
        _removeButton.gameObject.SetActive(false);
        _removeButton.onClick.AddListener(RemoveRelic);
    }

    public void ShowRemoveButton(bool show)
    {
        _removeButton.gameObject.SetActive(show);
    }

    public void RemoveRelic()
    {
        ToolTipManager.Instance.HideToolTip();
        RelicEventsSystem.RelicRemoved(_relic);
        Destroy(gameObject);
    }
}

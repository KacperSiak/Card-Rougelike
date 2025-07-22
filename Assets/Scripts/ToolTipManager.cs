using UnityEngine;
using TMPro;
public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager Instance;

    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipText;

    private RectTransform panelRect;
    private void Awake()
    {
        Instance = this;
        panelRect = tooltipPanel.GetComponent<RectTransform>();
        tooltipPanel.SetActive(false);

    }
    public void ShowToolTip(string description, Vector2 position)
    {
        tooltipPanel.SetActive(true);
        tooltipText.text = description;

        panelRect.position = position;
    }

    public void HideToolTip()
    {
        tooltipPanel.SetActive(false);
    }
}

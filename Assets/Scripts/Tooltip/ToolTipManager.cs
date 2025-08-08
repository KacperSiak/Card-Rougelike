using UnityEngine;
using TMPro;

//[RequireComponent(typeof(RectTransform))]
public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager Instance;

    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipText;

	[SerializeField] private RectTransform panelRect;

	private void OnValidate()
	{
		if(!panelRect)
            panelRect = tooltipPanel.GetComponent<RectTransform>();
	}

	private void Awake()
    {
        Instance = this;
        tooltipPanel.SetActive(false);
    }
    public void ShowToolTip(string name, string description, Vector2 position)
    {
        tooltipPanel.SetActive(true);
        tooltipText.text = name + "\n\n" + description;

        panelRect.position = position;
    }

    public void HideToolTip()
    {
        tooltipPanel.SetActive(false);
    }
}

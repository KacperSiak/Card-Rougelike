using UnityEngine;
using TMPro;

public class ToolTipControler : PersistenSingleton<ToolTipControler>
{
    [SerializeField] private GameObject _tooltipObject;
    [SerializeField] private TextMeshProUGUI _tooltipText;
    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    private void Update()
    {
        if (_tooltipObject.activeSelf)
        {
            Vector2 pos = Input.mousePosition;
            _tooltipObject.transform.position = pos + new Vector2(15f, -15f);
        }
    }

    public void Show(string text)
    {
        _tooltipText.text = text;
        _tooltipObject.SetActive(true);
    }

    public void Hide()
    {
        _tooltipObject?.SetActive(false);
    }
}

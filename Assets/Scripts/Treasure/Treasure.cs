using UnityEngine;
using UnityEngine.UI;

public class Treasure : MonoBehaviour
{
    [SerializeField] private Button TreasureButton;
    [SerializeField] private int gold;

    private void OnEnable()
    {
        TreasureButton.onClick.AddListener(GetTreasure);
    }

    private void OnDisable()
    {
        TreasureButton.onClick.RemoveAllListeners();
    }
    private void GetTreasure()
    {
        PlayerManager.Instance.GoldChange(gold, true);
    }
}

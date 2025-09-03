using UnityEngine;
using UnityEngine.UI;

public class BuffAndDebuffUI : MonoBehaviour
{
    [SerializeField] private GameObject _iconPrefab;
    [SerializeField] private Transform _buffAndDebuffPanel;
    
    public void AddBuffIcon(PotionData buff)
    {
        GameObject icon = Instantiate(_iconPrefab, _buffAndDebuffPanel);
        Image img = icon.GetComponent<Image>();
        if (img != null)
            img.sprite = buff.potionIcon;

        BuffAndDebuffIconUI ui = icon.GetComponent<BuffAndDebuffIconUI>();
        if (ui != null)
            ui.BuffIconSetup(buff);
    }

    public void AddDebuffIcon(DebuffData debuff)
    {
        GameObject icon = Instantiate(_iconPrefab, _buffAndDebuffPanel);
        Image img = icon.GetComponent<Image>();
        if (img != null)
            img.sprite = debuff.debuffIcon;

        BuffAndDebuffIconUI ui = icon.GetComponent<BuffAndDebuffIconUI>();
        if (ui != null)
            ui.DebuffIconSetup(debuff);
    }
}

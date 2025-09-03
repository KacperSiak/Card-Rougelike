using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "VaseRelicUpgrade", menuName = "Shop/VaseRelicUpgrade")]
public class VaseRelicUpgrade : ShopItem
{
    public int price;
    public Sprite icon;

    public static UnityEvent OnVaseAcquired = new UnityEvent();

    public void GotVase()
    {
        OnVaseAcquired.Invoke();
        RelicIconButtonsUI.vaseEnabled = true;
        ShopMenager.Instance.allItems.Remove(this);
    }
}
using UnityEngine;

public class CardSetupManager : MonoBehaviour
{
    public static CardSetupManager Instance { get; private set; }

    private MaterialPropertyBlock _block;

    void Awake()
    {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);

        _block = new();
    }

    public void SetupCardVisual(MeshRenderer card, int col, int row)
    {
        _block.SetFloat("_ColIndex", col);
        _block.SetFloat("_RowIndex", row);

        card.SetPropertyBlock(_block);

        card.SetPropertyBlock(_block, 0);
    }
}

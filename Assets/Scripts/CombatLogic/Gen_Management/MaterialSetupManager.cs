using UnityEngine;

public class MaterialSetupManager : PersistenSingleton<MaterialSetupManager>
{
    private MaterialPropertyBlock _block;

    void Start()
    {
        _block = new();
    }

    public void SetupVisual(MeshRenderer card, int col, int row)
    {
        _block.SetFloat("_ColIndex", col);
        _block.SetFloat("_RowIndex", row);

        card.SetPropertyBlock(_block);

        card.SetPropertyBlock(_block, 0);
    }
}
